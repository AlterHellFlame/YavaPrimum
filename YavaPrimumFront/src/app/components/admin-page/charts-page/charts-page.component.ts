import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../../services/task/task.service';
import { ChartData, ChartOptions } from 'chart.js';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-charts-page',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './charts-page.component.html',
  styleUrls: ['./charts-page.component.scss']
})
export class ChartsPageComponent implements OnInit {
  allTasks: Tasks[] = [];

  // Данные для диаграмм
  hrPerformanceChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: []
  };
  recruitmentStatsChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: []
  };
  rejectionStatsChartData: ChartData<'pie', number[], string> = {
    labels: [],
    datasets: []
  };
  eventPlanChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: []
  };

  // Опции для диаграмм
  chartOptions: ChartOptions = {
    responsive: true,
    plugins: {
      legend: {
        display: true,
        position: 'top'
      }
    }
  };

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.getAllTasks().subscribe((tasks) => {
      this.allTasks = tasks;
      this.generateReports();
    });
  }

  generateReports(): void {
    this.generateHRPerformanceReport();
    this.generateRecruitmentStatsReport();
    this.generateRejectionStatsReport();
    this.generateEventPlanReport();
  }

  generateHRPerformanceReport(): void {
    // Фильтруем задачи для HR и рекрутеров
    const hrTasks = this.allTasks.filter(task => task.user.post === 'HR');
    const recruiterTasks = this.allTasks.filter(task => task.user.post === 'Кадровик');

    const labels = ['HR', 'Кадровик'];
    const hrProgress = hrTasks.filter(task => task.typeStatus === 2).length;
    const recruiterProgress = recruiterTasks.filter(task => task.typeStatus === 2).length;

    this.hrPerformanceChartData = {
      labels: labels,
      datasets: [
        {
          label: 'Прогресс задач',
          data: [hrProgress, recruiterProgress],
          backgroundColor: ['rgba(75, 192, 192, 0.2)', 'rgba(153, 102, 255, 0.2)'],
          borderColor: ['rgba(75, 192, 192, 1)', 'rgba(153, 102, 255, 1)'],
          borderWidth: 1
        }
      ]
    };
  }

  generateRecruitmentStatsReport(): void {
    // Фильтруем задачи с статусом "Пришел"
    const recruitmentTasks = this.allTasks.filter(task => task.status === 'Пришел');
    console.log(recruitmentTasks)
    const groupedByDate = this.groupTasksByDate(recruitmentTasks);

    this.recruitmentStatsChartData = {
      labels: groupedByDate.labels,
      datasets: [
        {
          label: 'Принятые сотрудники',
          data: groupedByDate.data,
          backgroundColor: [
            '#b4ff63', // Красный для отказов
          ],
          borderColor: [
            '#b4ff63', // Красный для отказов
          ],
        }
      ]
    };
  }

  generateRejectionStatsReport(): void {
    const rejectionTasks = this.allTasks.filter(task => task.status === 'Собеседование не пройдено' || task.status === 'Не пришел');
    const groupedByDate = this.groupTasksByDate(rejectionTasks);

    this.rejectionStatsChartData = {
      labels: groupedByDate.labels,
      datasets: [
        {
          label: 'Отказы',
          data: groupedByDate.data,
          backgroundColor: [
            'rgba(255, 99, 132, 0.6)', // Красный для отказов
            'rgba(75, 192, 192, 0.6)'  // Зеленый для принятых
          ],
          borderColor: [
            'rgba(255, 99, 132, 1)', // Красный для отказов
            'rgba(75, 192, 192, 1)'  // Зеленый для принятых
          ],
          borderWidth: 2,
          hoverOffset: 10,
          borderRadius: 5,
          spacing: 5
        }
      ]
    };
  }

  generateEventPlanReport(): void {
    // Фильтруем задачи, связанные с мероприятиями
    const receptionTasks = this.allTasks.filter(task => task.status === 'Назначен приём');
    const interviewTasks = this.allTasks.filter(task => task.status === 'Назначено собеседование');

    // Группируем задачи по дате
    const groupedReceptionTasks = this.groupTasksByDate(receptionTasks);
    const groupedInterviewTasks = this.groupTasksByDate(interviewTasks);

    // Уникальные даты для всех мероприятий
    const allDates = [...new Set([...groupedReceptionTasks.labels, ...groupedInterviewTasks.labels])];

    // Данные для диаграммы
    this.eventPlanChartData = {
      labels: allDates,
      datasets: [
        {
          label: 'Назначен приём',
          data: allDates.map(date => groupedReceptionTasks.data[groupedReceptionTasks.labels.indexOf(date)] || 0),
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          borderColor: 'rgba(75, 192, 192, 1)',
          borderWidth: 1
        },
        {
          label: 'Назначено собеседование',
          data: allDates.map(date => groupedInterviewTasks.data[groupedInterviewTasks.labels.indexOf(date)] || 0),
          backgroundColor: 'rgba(255, 159, 64, 0.2)',
          borderColor: 'rgba(255, 159, 64, 1)',
          borderWidth: 1
        }
      ]
    };
  }

  groupTasksByDate(tasks: Tasks[]): { labels: string[], data: number[] } {
    const grouped: { [key: string]: number } = {};

    tasks.forEach(task => {
      const date = task.dateTime.toString().split('T')[0];
      if (grouped[date]) {
        grouped[date]++;
      } else {
        grouped[date] = 1;
      }
    });

    const sortedEntries = Object.entries(grouped).sort((a, b) => {
      return new Date(a[0]).getTime() - new Date(b[0]).getTime();
    });

    const labels = sortedEntries.map(entry => entry[0]); // string[]
    const data = sortedEntries.map(entry => entry[1]);   // number[]

    return { labels, data };
  }

  exportToExcel(): void {
    // Создаем массив данных для экспорта
    const data = [];

    // Добавляем данные для каждой диаграммы
    if (this.rejectionStatsChartData.labels && this.rejectionStatsChartData.datasets[0].data) {
      data.push(['Статистика отказов', ...this.rejectionStatsChartData.labels]);
      data.push(['Количество', ...this.rejectionStatsChartData.datasets[0].data]);
      data.push([]); // Пустая строка для разделения
    }

    if (this.eventPlanChartData.labels && this.eventPlanChartData.datasets[0].data) {
      data.push(['План мероприятий', ...this.eventPlanChartData.labels]);
      data.push(['Количество', ...this.eventPlanChartData.datasets[0].data]);
      data.push([]); // Пустая строка для разделения
    }

    if (this.recruitmentStatsChartData.labels && this.recruitmentStatsChartData.datasets[0].data) {
      data.push(['Статистика по найму', ...this.recruitmentStatsChartData.labels]);
      data.push(['Количество', ...this.recruitmentStatsChartData.datasets[0].data]);
      data.push([]); // Пустая строка для разделения
    }

    if (this.hrPerformanceChartData.labels && this.hrPerformanceChartData.datasets[0].data) {
      data.push(['Мониторинг работы HR и рекрутеров', ...this.hrPerformanceChartData.labels]);
      data.push(['Количество', ...this.hrPerformanceChartData.datasets[0].data]);
    }

    // Создаем рабочий лист
    const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(data);

    // Создаем книгу
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Отчеты');

    // Сохраняем файл
    XLSX.writeFile(wb, 'Отчеты_диаграммы.xlsx');
  }
}