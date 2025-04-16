import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Chart, ChartData, ChartOptions, registerables } from 'chart.js';
import { User } from '../../../data/interface/User.interface';
import { UserService } from '../../../services/user/user.service';
import { TaskService } from '../../../services/task/task.service';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { OptionalDataService } from '../../../services/optional-data/optional-data.service';
import { BaseChartDirective } from 'ng2-charts';
import { catchError, of } from 'rxjs';
import { trigger, transition, style, animate } from '@angular/animations';

Chart.register(...registerables);

interface UserFilters {
  surname: string;
  firstName: string;
  patronymic: string;
  company: string;
  email: string;
  post: string;
  phone: string;
  [key: string]: string;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})
export class AdminComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  currentUser: User | null = null;
  isEditMode = false;
  isLoad = false;
  isChartVisible = false;

  filters: UserFilters = {
    surname: '',
    firstName: '',
    patronymic: '',
    company: '',
    email: '',
    post: '',
    phone: '',
  };

  selectedUserTasks: Tasks[] = [];
  allTasks: Tasks[] = [];
  companies: string[] = [];
  activeMonth: Date = new Date();
  taskChartData: ChartData<'line', number[], string> = {
    labels: [],
    datasets: []
  };
  taskChartOptions: ChartOptions<'line'> = {
    responsive: true,
    scales: {
      y: {
        beginAtZero: true
      }
    }
  };

  constructor(
    private userService: UserService,
    private taskService: TaskService,
    private optional: OptionalDataService
  ) {}

  ngOnInit(): void {
    this.optional.getPostsAndCountry().pipe(
      catchError(err => {
        console.error('Ошибка загрузки компаний:', err);
        return of({ companies: [] });
      })
    ).subscribe(data => {
      this.companies = data.companies || [];
    });

    this.loadUsers();
    this.loadAllTasks();
  }

  private loadUsers(): void {
    this.userService.getAllUsersData().pipe(
      catchError(err => {
        console.error('Ошибка загрузки пользователей:', err);
        return of([]);
      })
    ).subscribe(users => {
      this.users = users;
      this.filteredUsers = [...users];
    });
  }

  private loadAllTasks(): void {
    this.taskService.getAllTasks().pipe(
      catchError(err => {
        console.error('Ошибка загрузки задач:', err);
        return of([]);
      })
    ).subscribe(tasks => {
      this.allTasks = tasks;
    });
  }

  openEditModal(user: User): void {
    console.log("openEditModal")
    this.isEditMode = true;
    this.currentUser = { ...user };
    this.isLoad = true;
  }

  deleteUser(user: User): void {
    if (confirm(`Вы уверены, что хотите удалить пользователя ${user.surname}?`)) {
      this.userService.deleteUser(user.userId).subscribe({
        next: () => this.loadUsers(),
        error: (err) => console.error('Ошибка удаления пользователя:', err)
      });
    }
  }

  openModal(): void {
    this.isEditMode = false;
    this.currentUser = {
      userId: 'E9DA72EF-9045-4E95-AAFE-0295192A9064',
      surname: '',
      firstName: '',
      patronymic: '',
      imgUrl: 'default.jpg',
      phone: '',
      email: '',
      company: this.companies[0] || '',
      post: 'HR'
    };
    this.isLoad = true;
  }

  closeModal(): void {
    this.isEditMode = false;
    this.currentUser = null;
  }

  onSubmit(form: NgForm): void {
    if (!this.currentUser || form.invalid) return;

    const operation = this.isEditMode 
      ? this.userService.updateUser(this.currentUser)
      : this.userService.addUser(this.currentUser);

    operation.subscribe({
      next: () => {
        this.loadUsers();
        this.closeModal();
      },
      error: (err) => console.error('Ошибка сохранения пользователя:', err)
    });
  }

  applyFilters(): void {
    this.filteredUsers = this.users.filter(user => 
      Object.keys(this.filters).every(key =>
        !this.filters[key] || String(user[key as keyof User]).toLowerCase().includes(this.filters[key].toLowerCase())
      )
    );
  }

  getFilterPlaceholder(key: string): string {
    const placeholders: Record<string, string> = {
      surname: 'Фамилия',
      firstName: 'Имя',
      patronymic: 'Отчество',
      company: 'Компания',
      email: 'Email',
      post: 'Должность',
      phone: 'Телефон'
    };
    return placeholders[key] || '';
  }

  getAllTasksOfUser(user: User): void {
    if (!user?.userId) {
      console.error('Некорректный объект пользователя');
      return;
    }

    this.currentUser = user;
    this.selectedUserTasks = this.allTasks.filter(task =>
      task.user?.userId === user.userId
    );
    this.renderTaskNowChart();
  }

  changeMonth(num: number): void {
    const newDate = new Date(this.activeMonth);
    newDate.setMonth(newDate.getMonth() + num);
    this.activeMonth = new Date(newDate.getFullYear(), newDate.getMonth(), 1);
    this.renderTaskNowChart();
  }

  private renderTaskNowChart(): void {
    if (!this.currentUser || this.selectedUserTasks.length === 0) {
      this.taskChartData = { labels: [], datasets: [] };
      return;
    }

    const allDates = this.getAllDatesOfActiveMonth();
    const dateCounts = this.countTasksByDate(allDates);

    this.taskChartData = {
      labels: allDates.map(date => this.formatDateForDisplay(date)),
      datasets: [
        {
          label: 'Выполненные задачи',
          data: allDates.map(date => dateCounts[date]?.completed || 0),
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1,
        },
        {
          label: 'Все задачи',
          data: allDates.map(date => dateCounts[date]?.total || 0),
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1,
        },
      ],
    };
  }

  private countTasksByDate(dates: string[]): Record<string, { total: number, completed: number }> {
    const counts: Record<string, { total: number, completed: number }> = {};

    this.selectedUserTasks.forEach(task => {
      const date = task.dateTime.toString().split('T')[0];
      if (!dates.includes(date)) return;

      if (!counts[date]) {
        counts[date] = { total: 0, completed: 0 };
      }
      counts[date].total++;
      if (task.typeStatus === 2) {
        counts[date].completed++;
      }
    });

    return counts;
  }

  private getAllDatesOfActiveMonth(): string[] {
    const year = this.activeMonth.getFullYear();
    const month = this.activeMonth.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    return Array.from({ length: daysInMonth }, (_, i) => {
      const date = new Date(year, month, i + 1);
      return date.toISOString().split('T')[0];
    });
  }

  private formatDateForDisplay(isoDate: string): string {
    const date = new Date(isoDate);
    return date.getDate().toString();
  }

  get activeMonthToString(): string {
    return this.activeMonth.toLocaleString('ru-RU', { month: 'long' });
  }


  public toggleChartDisplay(): void
  {
    this.isChartVisible = !this.isChartVisible;
  }
}
