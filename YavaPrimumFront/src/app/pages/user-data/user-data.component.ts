import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Chart } from 'chart.js/auto';
import { TaskService } from '../../services/task/task.service';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { DateTime } from 'luxon';
import { Candidate } from '../../data/interface/Candidate.interface';
import { UserService } from '../../services/user/user.service';
import { User } from '../../data/interface/User.interfase';

@Component({
  selector: 'app-user-data',
  imports: [CommonModule, HttpClientModule],
  templateUrl: './user-data.component.html',
  styleUrl: './user-data.component.scss',
  providers: [TaskService]
})
export class UserDataComponent implements OnInit {
  allTasks: Tasks[] = [];
  chart: Chart | undefined;
  formData! : User; 

  constructor(private taskService: TaskService, private userService: UserService) {}

  ngOnInit(): void 
  {
    this.taskService.getAllTasks();
    this.taskService.allTasks$.subscribe(tasks => {

      this.allTasks = this.taskService.filterAndSortTasks(tasks);

      this.loadChart();
    });

    this.userService.getUserDate().subscribe(user => {
      this.formData = user;
    });
  }

  loadChart() {
    const taskData = this.getTasksCountByDay();
    console.log("Task Data:", taskData); // Проверка преобразованных данных
    const labels = taskData.map(data => data.date);
    const data = taskData.map(data => data.count);

    const ctx = document.getElementById('tasksChart') as HTMLCanvasElement;

    if (this.chart) {
      this.chart.destroy(); // Уничтожение предыдущей диаграммы
    }

    console.log("Создание диаграммы...");
    this.chart = new Chart(ctx, {
      type: 'line',
      data: {
        labels,
        datasets: [{
          label: 'Количество выполненных задач',
          data,
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          y: {
            beginAtZero: true
          }
        }
      }
    });
    console.log("Диаграмма создана.");
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
  
}
