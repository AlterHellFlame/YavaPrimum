// admin.component.ts
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Chart, ChartData, ChartOptions, registerables } from 'chart.js';
import { defaultUser, User } from '../../../data/interface/User.interface';
import { UserService } from '../../../services/user/user.service';
import { TaskService } from '../../../services/task/task.service';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { OptionalDataService } from '../../../services/optional-data/optional-data.service';
import { BaseChartDirective } from 'ng2-charts';
import { catchError, of } from 'rxjs';
import { NotifyService } from '../../../services/notify/notify.service';
Chart.register(...registerables);

interface UserFilters {
  [key: string]: string;
  surname: string;
  firstName: string;
  patronymic: string;
  company: string;
  email: string;
  post: string;
  phone: string;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})
export class AdminComponent {
  users: User[] = [];
  filteredUsers: User[] = [];
  currentUser: User = this.getDefaultUser();
  isEditMode = false;
  isChartVisible = false;
  modalTitle = '';
  isFormSubmitted = false;
  emailExists = false;
  phoneExists = false;

  filters: UserFilters = {
    surname: '',
    firstName: '',
    patronymic: '',
    company: '',
    email: '',
    post: '',
    phone: '',
  };

  tableHeaders = [
    { key: 'surname', title: 'Фамилия' },
    { key: 'firstName', title: 'Имя' },
    { key: 'patronymic', title: 'Отчество' },
    { key: 'company', title: 'Компания' },
    { key: 'email', title: 'Email' },
    { key: 'post', title: 'Должность' },
    { key: 'phone', title: 'Телефон' }
  ];

  selectedUserTasks: Tasks[] = [];
  allTasks: Tasks[] = [];
  companies: string[] = [];
  activeMonth: Date = new Date();
  sortColumn: string = 'surname';
  sortDirection: 'asc' | 'desc' = 'asc';

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
    private optional: OptionalDataService,
    private notify: NotifyService
  ) {
    this.loadInitialData();
  }

  private getDefaultUser(): User {
    return defaultUser;
  }

  private loadInitialData(): void {
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
      this.applyFilters();
      if (users.length > 0 && !this.currentUser.userId) {
        this.currentUser = users[0];
        this.getAllTasksOfUser(users[0]);
      }
    });
  }

  private loadAllTasks(): void {
    this.taskService.getAllTasks().pipe(
      catchError(err => {
        console.error('Ошибка загрузки задач:', err);
        return of([]);
      })
    ).subscribe(tasks => {
        this.allTasks = tasks.filter(task => {
          return (task.typeStatus === 0 || task.typeStatus === 2);
        });
    });
  }

  sortTable(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.applySorting();
  }

  private applySorting(): void {
    this.filteredUsers.sort((a, b) => {
      const valueA = a[this.sortColumn as keyof User];
      const valueB = b[this.sortColumn as keyof User];
      
      if (typeof valueA === 'string' && typeof valueB === 'string') {
        return this.sortDirection === 'asc' 
          ? valueA.localeCompare(valueB) 
          : valueB.localeCompare(valueA);
      }
      
      if (valueA > valueB) return this.sortDirection === 'asc' ? 1 : -1;
      if (valueA < valueB) return this.sortDirection === 'asc' ? -1 : 1;
      return 0;
    });
  }

  applyFilters(): void {
    this.filteredUsers = this.users.filter(user => 
      Object.keys(this.filters).every(key =>
        !this.filters[key] || String(user[key as keyof User]).toLowerCase().includes(this.filters[key].toLowerCase())
    ));
    this.applySorting();
  }

  openAddModal(): void {
    this.isEditMode = false;
    this.modalTitle = 'Добавить пользователя';
    this.currentUser = this.getDefaultUser();
    this.isFormSubmitted = false;
    this.emailExists = false;
    this.phoneExists = false;
  }

  openEditModal(user: User): void {
    this.isEditMode = true;
    this.modalTitle = 'Редактировать пользователя';
    this.currentUser = { ...user };
    this.isFormSubmitted = false;
    this.emailExists = false;
    this.phoneExists = false;
  }

  closeModal(): void {
    this.currentUser = this.getDefaultUser();
    this.isFormSubmitted = false;
  }

  onSubmit(form: NgForm): void {
    this.isFormSubmitted = true;
    if (form.invalid || this.emailExists || this.phoneExists) return;

    const operation = this.isEditMode 
      ? this.userService.updateUser(this.currentUser)
      : this.userService.addUser(this.currentUser);

    operation.subscribe({
      next: () => {
        this.loadUsers();
        this.closeModal();
        this.notify.showSuccess(
          this.isEditMode 
            ? 'Пользователь успешно обновлен' 
            : 'Пользователь успешно зарегистрирован'
        );
      },
      error: (err) => {
        console.error('Ошибка сохранения пользователя:', err);
        this.notify.showError(
          err.status === 409 
            ? err.response.data?.Message 
            : 'Произошла ошибка при сохранении пользователя'
        );
      }
    });
  }

  deleteUser(user: User): void {
    if (confirm(`Вы уверены, что хотите удалить пользователя ${user.surname}?`)) {
      this.userService.deleteUser(user.userId).subscribe({
        next: () => this.loadUsers(),
        error: (err) => console.error('Ошибка удаления пользователя:', err)
      });
    }
  }

  validateEmail(): void {
    this.emailExists = this.users.some(user => 
      user.email === this.currentUser.email && 
      (!this.isEditMode || user.userId !== this.currentUser.userId)
    );
  }

  validatePhone(): void {
    this.phoneExists = this.users.some(user => 
      user.phone === this.currentUser.phone && 
      (!this.isEditMode || user.userId !== this.currentUser.userId)
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

  // Остальные методы без изменений
  getAllTasksOfUser(user: User): void {
    if (!user?.userId) return;
    
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
    if (!this.currentUser?.userId || this.selectedUserTasks.length === 0) {
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
    return new Date(isoDate).getDate().toString();
  }

  get activeMonthToString(): string {
    return this.activeMonth.toLocaleString('ru-RU', { month: 'long' });
  }

  toggleChartDisplay(): void {
    this.isChartVisible = !this.isChartVisible;
  }
}