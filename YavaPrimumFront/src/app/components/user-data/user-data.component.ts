import { Component, OnInit } from '@angular/core';
import { Chart, ChartData, ChartOptions } from 'chart.js/auto';
import { TaskService } from '../../services/task/task.service';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { DateTime } from 'luxon';
import { UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';
import { User } from '../../data/interface/User.interface';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-user-data',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './user-data.component.html',
  styleUrl: './user-data.component.scss',
})
export class UserDataComponent implements OnInit {
  totalInterviewPassed = 0;
  totalTestDeadline = 0;
  totalInterviewScheduled = 0;
  totalTestCompleted = 0;

  allTasks: Tasks[] = [];
  isAdmin: boolean = (localStorage.getItem('isAdmin') === 'true'? true : false);
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
  formData! : User; 

  constructor(private taskService: TaskService, private userService: UserService, private router: Router) {}

  activeMonth: Date = new Date(); // Инициализация с текущей датой

  ngOnInit(): void 
  {
    this.taskService.loadAllTasks().subscribe({
      next: data => {
        let allTasks = data.map(task => ({
          ...task,
          dateTime: DateTime.fromISO(task.dateTime as unknown as string)
        }));
        console.log(allTasks);
        this.taskService.setAllTasks(allTasks);
        this.renderTaskNowChart()

        this.calculateTaskStatistics()

        this.userService.getUserData().subscribe(user => {
          this.formData = user;
        });
    
      }
    });

  }


    private calculateTaskStatistics(): void {
    this.totalInterviewPassed = this.allTasks.filter(t => t.status === 'Собеседование пройдено' || t.status === 'Тестовое задание выполнено').length;
    this.totalTestDeadline = this.allTasks.filter(t => t.status === 'Срок тестового задания').length;
    this.totalInterviewScheduled = this.allTasks.filter(t => t.status === 'Собеседование назначено').length;
    this.totalTestCompleted = this.allTasks.filter(t => t.status === 'Тестовое задание выполнено').length;
  }
  renderTaskNowChart(): void {
    this.allTasks = this.taskService.getAllTasksOfUser();

  // Группировка задач по дате и подсчёт количества задач и задач со статусом 2
  const counts: { [date: string]: { tasks: number, tasksWithStatus2: number } } = this.allTasks.reduce((acc, task) => {
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
          label: 'Количество выполненных задач',
          data: tasksWithStatus2Counts,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1,
        },
        {
          label: 'Количество задач',
          data: taskCounts,
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
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

  changeMonth(num: number): void {
    const newDate = new Date(this.activeMonth);
    newDate.setMonth(newDate.getMonth() + num);
    this.activeMonth = new Date(newDate.getFullYear(), newDate.getMonth(), 1);
    this.renderTaskNowChart();
  }

  getTasksCountByDay(): { date: string, count: number }[] {
    const taskCountByDay: { [key: string]: number } = {};
  
    // Получаем все дни текущего месяца
    const now = DateTime.now();
    const daysInMonth = now.daysInMonth;
    const year = now.year;
    const month = now.month;
  
    // Инициализируем массив taskCountByDay для всех дней текущего месяца
    for (let day = 1; day <= daysInMonth; day++) {
      const date = DateTime.fromObject({ year, month, day }).toISODate();

      if (date) {
        taskCountByDay[date] = 0;
      }
    }
    console.log(taskCountByDay);
  
    // Заполняем данные задачами
    this.allTasks.forEach(task => {
      const date = task.dateTime.toISODate(); // Получаем дату в формате YYYY-MM-DD
      if (date) 
      {
        taskCountByDay[date]++;
        console.log(taskCountByDay[date]);
      }
    });
  
    return Object.keys(taskCountByDay).map(date => ({
      date,
      count: taskCountByDay[date]
    }));
  }

  logOut()
  {
    document.cookie = `token-cookies=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/`;
    this.router.navigate(['/']);
  }
}
