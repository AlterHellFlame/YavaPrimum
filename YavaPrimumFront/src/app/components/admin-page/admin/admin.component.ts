import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartData, ChartOptions, registerables } from 'chart.js';
import { User } from '../../../data/interface/User.interface';
import { UserService } from '../../../services/user/user.service';
import { TaskService } from '../../../services/task/task.service';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { DateTime } from 'luxon';
import { OptionalDataService } from '../../../services/optional-data/optional-data.service';
import { BaseChartDirective } from 'ng2-charts';

Chart.register(...registerables);

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
  selectedUserTasks: Tasks[] = [];
  allTasks: Tasks[] = [];

  // Фильтры
  filters = {
    surname: '',
    firstName: '',
    patronymic: '',
    company: '',
    email: '',
    post: '',
    phone: '',
  };

  // Текущий пользователь для редактирования/добавления
  currentUser: User = {} as User;
  isEditMode: boolean = false;

  // График
  taskChart: Chart | null = null;
  activeMonth: Date = new Date();
  companies: string[] = [];

  constructor(private userService: UserService, private taskService: TaskService, private optional: OptionalDataService) {}

  ngOnInit(): void {
    this.optional.getPostsAndCountry().subscribe(data =>{
      this.companies = data.companies;
      console.log(this.companies)
    })

    this.loadUsers();
    this.taskService.getAllTasks().subscribe((tasks) => {
      this.allTasks = tasks;
    });

  }

  // Загрузка пользователей
  loadUsers(): void {
    this.userService.getAllUsersData().subscribe((users) => {
      this.users = users;
      this.filteredUsers = users;
    });
  }

  // Открытие модального окна для редактирования
  openEditModal(user: User): void {
    console.log("openEditModal")
    this.isEditMode = true;
    this.currentUser = { ...user };

    this.isLoad = true;
  }

  // Отправка формы
  onSubmit(): void {
    console.log("onSubmit")
    if (this.isEditMode) 
      {
        console.log("updateUser");
      this.userService.updateUser(this.currentUser).subscribe(() => {
        this.loadUsers();
        this.closeModal();
        this.isEditMode = false;
      });
    } else 
    {
      console.log("addUser");
      this.userService.addUser(this.currentUser).subscribe({
        next: () => {
            this.loadUsers();
            this.closeModal();
            this.isEditMode = false;
        },
        error: (error) => {
            console.error('Ошибка при добавлении пользователя:', error);
        }
        
    });
    }
  }


  isLoad: boolean = false;
  // Удаление пользователя
  deleteUser(user: User): void {
    if (confirm(`Вы уверены, что хотите удалить пользователя ${user.surname}?`)) {
      this.userService.deleteUser(user.userId).subscribe(() => {
        this.loadUsers();
      });
    }
  }

  // Закрытие модального окна
  closeModal(): void {
    console.log("closeModal")
    const modal = document.getElementById('userModal');
    if (modal) {
      modal.classList.remove('show');
      modal.setAttribute('aria-hidden', 'true');
      modal.setAttribute('style', 'display: none');
      const modalBackdrop = document.querySelector('.modal-backdrop');
      if (modalBackdrop) {
        modalBackdrop.remove();
      }
    }
  }

  openModal(): void {
    this.isLoad = false;
    this.currentUser = {
        userId: 'E9DA72EF-9045-4E95-AAFE-0295192A9064', // Установите значение по умолчанию или оставьте пустым
        surname: '',
        firstName: '',
        patronymic: '',
        imgUrl: 'default.jpg', // Установите значение по умолчанию
        phone: '',
        email: '',
        company: this.companies[0],
        post: 'HR'
    };
    this.isLoad = true;
  }



  // Применение фильтров
  applyFilters(): void {
    console.log('Фильтры применены:', this.filters);

    this.filteredUsers = this.users.filter((user) => {
      return (
        (this.filters.surname === '' ||
          user.surname.toLowerCase().includes(this.filters.surname.toLowerCase())) &&
        (this.filters.firstName === '' ||
          user.firstName.toLowerCase().includes(this.filters.firstName.toLowerCase())) &&
        (this.filters.patronymic === '' ||
          user.patronymic.toLowerCase().includes(this.filters.patronymic.toLowerCase())) &&
        (this.filters.company === '' ||
          user.company.toLowerCase().includes(this.filters.company.toLowerCase())) &&
        (this.filters.email === '' ||
          user.email.toLowerCase().includes(this.filters.email.toLowerCase())) &&
        (this.filters.post === '' ||
          user.post.toLowerCase().includes(this.filters.post.toLowerCase())) &&
        (this.filters.phone === '' || user.phone.includes(this.filters.phone))
      );
    });
  }

  getAllTasksOfUser(user: User): void {
    console.log("getAllTasksOfUser")
    console.log('Выбранный пользователь:', user); // Отладка
  
    if (!user || !user.userId) {
      console.error('Некорректный объект пользователя 1:', user);
      return;
    }
  
    if (!user.userId) {
      console.error('Некорректный объект пользователя 2:', user);
      return;
    }

    if (!this.allTasks || this.allTasks.length === 0) {
      console.error('Данные задач еще не загружены.');
      return;
    }
  
    this.selectedUserTasks = this.allTasks.filter((task) => task.user.userId === user.userId);
    this.isLoad = true;
    this.isEditMode = true;
    console.log('Задачи выбранного пользователя:', this.selectedUserTasks); // Отладка
    this.renderTaskNowChart(); // Рендер графика задач
  }

  changeMonth(num: number): void {
    const newDate = new Date(this.activeMonth);
    newDate.setMonth(newDate.getMonth() + num);
    this.activeMonth = new Date(newDate.getFullYear(), newDate.getMonth(), 1);
    this.renderTaskNowChart();
  }

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

  // Рендер графика задач
  renderTaskNowChart(): void {

  // Группировка задач по дате и подсчёт количества задач и задач со статусом 2
  const counts: { [date: string]: { tasks: number, tasksWithStatus2: number } } = this.selectedUserTasks.reduce((acc, task) => {
    const date = task.dateTime.toString().split('T')[0]; // Преобразование даты
    if (!acc[date]) {
      acc[date] = { tasks: 0, tasksWithStatus2: 0 };
    }
    acc[date].tasks += 1; // Общее количество задач
    if (task.typeStatus === 2) {
      acc[date].tasksWithStatus2 += 1; // Количество задач со статусом 2
    }
    return acc;
  }, {} as { [date: string]: { tasks: number, tasksWithStatus2: number } });

  // Получение всех дат текущего месяца
  const allDates = this.getAllDatesOfActiveMonth();
  const taskCounts = allDates.map(date => counts[date]?.tasks || 0);
  const tasksWithStatus2Counts = allDates.map(date => counts[date]?.tasksWithStatus2 || 0);

    // Обновление данных графика
    this.taskChartData = {
      labels: allDates,
      datasets: [
        {
          label: 'Количество задач',
          data: taskCounts,
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1,
        },
        {
          label: 'Количество выполненных задач',
          data: tasksWithStatus2Counts,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1,
        },
      ],
    };
  }


  // Получение всех дат активного месяца
  getAllDatesOfActiveMonth(): string[] {
    const year = this.activeMonth.getFullYear();
    const month = this.activeMonth.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const allDates = [];
    for (let day = 1; day <= daysInMonth; day++) {
        const date = new Date(year, month, day);
        allDates.push(date.toISOString().split('T')[0]);
    }
    return allDates;
  }


  get activeMonthToString(): string {
    return this.activeMonth.toLocaleString('ru-RU', { month: 'long' });
  }
 
}
