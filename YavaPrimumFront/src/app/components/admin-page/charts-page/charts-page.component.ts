import { Component, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ChartData, ChartOptions } from 'chart.js';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import * as ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { User } from '../../../data/interface/User.interface';
import { TaskService } from '../../../services/task/task.service';
import { DateTime } from 'luxon';

// Типы и константы
type RecruitmentStatus = 
  | 'Собеседование пройдено'
  | 'Собеседование не пройдено'
  | 'Приём пройден'
  | 'Приём не пройден'
  | 'Собеседование назначено'
  | 'Прием назначен'
  | 'Отказ кандидата';

type ChartPeriod = 'day' | 'month' | 'year' | 'week' | '3months' | 'custom';

interface StatusGroup {
  label: string;
  filter: (t: Tasks) => boolean;
  color: string;
}

const CHART_CONFIG = {
  colors: {
    hr: '#4A89DC',
    recruiter: '#D770AD',
    interviewSuccess: '#63B598',
    interviewFail: '#E9573F',
    hireSuccess: '#3BAFDA',
    hireFail: '#F6BB42',
    events: '#3BAFDA'
  },
  excel: {
    header: {
      fill: { type: 'pattern', pattern: 'solid', fgColor: { argb: '4472C4' } },
      font: { bold: true, color: { argb: 'FFFFFF' } }
    },
    dataHeader: {
      fill: { type: 'pattern', pattern: 'solid', fgColor: { argb: 'E9E9E9' } },
      font: { bold: true }
    }
  }
};

const STATUS_GROUPS: StatusGroup[] = [
  { 
    label: 'Успешное собеседование', 
    filter: t => t.status === 'Собеседование пройдено' && t.typeStatus === 2,
    color: CHART_CONFIG.colors.interviewSuccess
  },
  { 
    label: 'Неудачное собеседование', 
    filter: t => t.status === 'Собеседование не пройдено',
    color: CHART_CONFIG.colors.interviewFail
  },
  { 
    label: 'Успешный приём', 
    filter: t => t.status === 'Приём пройден' && t.typeStatus === 2,
    color: CHART_CONFIG.colors.hireSuccess
  },
  { 
    label: 'Неудачный приём', 
    filter: t => t.status === 'Приём не пройден',
    color: CHART_CONFIG.colors.hireFail
  }
];

const EVENT_STATUSES = ['Собеседование назначено', 'Прием назначен'];

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, ReactiveFormsModule],
  templateUrl: './charts-page.component.html',
  styleUrls: ['./charts-page.component.scss']
})
export class ChartsPageComponent implements OnInit {
  // Form Controls
  startDateControl = new FormControl<string>('');
  endDateControl = new FormControl<string>('');
  activePeriod: ChartPeriod = 'week';
  
  // UI Config
  periods = [
    { label: 'Неделя', value: 'week' as ChartPeriod },
    { label: 'Месяц', value: 'month' as ChartPeriod },
    { label: '3 месяца', value: '3months' as ChartPeriod },
    { label: 'Год', value: 'year' as ChartPeriod },
    { label: 'Вручную', value: 'custom' as ChartPeriod }
  ];

  // Stats
  totalHired = 0;
  totalRejections = 0;
  totalEvents = 0;
  totalActiveTasks = 0;

  // Charts
  hrPerformanceChart: ChartData<'bar'> = { labels: [], datasets: [] };
  recruitmentChart: ChartData<'line'> = { labels: [], datasets: [] };
  eventsChart: ChartData<'bar'> = { labels: [], datasets: [] };

  chartOptions: ChartOptions = {
    responsive: true,
    scales: {
      x: { title: { display: true, text: 'Период' } },
      y: { 
        title: { display: true, text: 'Количество' }, 
        beginAtZero: true,
        ticks: {
          precision: 0 // Обеспечиваем целые числа на оси Y
        }
      }
    },
    plugins: {
      title: { display: true, position: 'top' },
      legend: { position: 'bottom' }
    }
  };

  private allTasks: Tasks[] = [];

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.setPeriod('week');
    this.loadTasks();
    
    this.startDateControl.valueChanges.subscribe(() => this.onDateChange());
    this.endDateControl.valueChanges.subscribe(() => this.onDateChange());
  }

  setPeriod(period: ChartPeriod): void {
    this.activePeriod = period;
    let startDate: DateTime;
    let endDate = DateTime.now();

    switch (period) {
      case 'week':
        startDate = endDate.minus({ weeks: 1 });
        break;
      case 'month':
        startDate = endDate.minus({ months: 1 });
        break;
      case '3months':
        startDate = endDate.minus({ months: 3 });
        break;
      case 'year':
        startDate = endDate.minus({ years: 1 });
        break;
      case 'custom':
        // Для ручного ввода не устанавливаем даты
        return;
      default:
        startDate = endDate.minus({ days: 1 });
    }

    this.startDateControl.setValue(startDate.toFormat('yyyy-MM-dd'));
    this.endDateControl.setValue(endDate.toFormat('yyyy-MM-dd'));
    this.applyDateFilter();
  }

  async exportToExcel(): Promise<void> {
    const workbook = new ExcelJS.Workbook();
    workbook.creator = 'HR System';
    workbook.created = new Date();

    this.addSummarySheet(workbook);
    this.addPeriodInfoSheet(workbook);

    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), `HR_Отчеты_${DateTime.now().toFormat('yyyy-MM-dd')}.xlsx`);
  }

  private loadTasks(): void {
    const start = this.startDateControl.value ? DateTime.fromISO(this.startDateControl.value).startOf('day') : null;
    const end = this.endDateControl.value ? DateTime.fromISO(this.endDateControl.value).endOf('day') : null;
    
    if (!start || !end) return;
    
    this.taskService.getAllTasks().subscribe({
      next: tasks => {
        // Фильтруем задачи по выбранному периоду
        this.allTasks = tasks.filter(task => {
          const taskDate = this.getTaskDate(task);
          return taskDate >= start && taskDate <= end;
        });
        
        this.calculateSummary();
        this.generateCharts();
      },
      error: err => console.error('Ошибка загрузки задач:', err)
    });
  }

  private generateCharts(): void {
    this.generateHrPerformanceChart();
    this.generateCombinedRecruitmentChart();
    this.generateEventsChart();
  }

  private generateCombinedRecruitmentChart(): void {
    const groupBy = this.activePeriod === 'year' ? 'month' : 'day';
    const allDates = new Set<string>();
    const datasets = [];

    for (const group of STATUS_GROUPS) {
      const tasks = this.allTasks.filter(group.filter);
      const data = this.groupByPeriod(tasks, groupBy, this.getTaskDate);
      
      data.labels.forEach(label => allDates.add(label));
      datasets.push(this.createChartDataset(group.label, data.values, group.color));
    }

    this.recruitmentChart = {
      labels: Array.from(allDates).sort(this.sortDates),
      datasets
    };
  }

  private generateHrPerformanceChart(): void {
    const hrTasks = this.allTasks.filter(t => this.isHR(t.user));
    const recruiterTasks = this.allTasks.filter(t => this.isRecruiter(t.user));

    this.hrPerformanceChart = {
      labels: ['Выполненные задачи', 'Задачи в работе'],
      datasets: [
        this.createChartDataset('HR', [
          hrTasks.filter(t => t.typeStatus === 2).length,
          hrTasks.filter(t => t.typeStatus !== 2).length
        ], CHART_CONFIG.colors.hr),
        this.createChartDataset('Рекрутеры', [
          recruiterTasks.filter(t => t.typeStatus === 2).length,
          recruiterTasks.filter(t => t.typeStatus !== 2).length
        ], CHART_CONFIG.colors.recruiter)
      ]
    };
  }

  private generateEventsChart(): void {
    const eventTasks = this.allTasks.filter(t => EVENT_STATUSES.includes(t.status));
    const grouped = this.groupByStatus(eventTasks);

    this.eventsChart = {
      labels: Object.keys(grouped),
      datasets: [
        this.createChartDataset('Мероприятия', Object.values(grouped), CHART_CONFIG.colors.events)
      ]
    };
  }

  private calculateSummary(): void {
    this.totalHired = this.allTasks.filter(t => 
      t.status === 'Собеседование пройдено' && t.typeStatus === 2
    ).length;

    this.totalRejections = this.allTasks.filter(t => 
      ['Собеседование не пройдено', 'Отказ кандидата'].includes(t.status)
    ).length;

    this.totalEvents = this.allTasks.filter(t => 
      EVENT_STATUSES.includes(t.status)
    ).length;

    this.totalActiveTasks = this.allTasks.filter(t => t.typeStatus !== 2).length;
  }

  private getTaskDate(task: Tasks): DateTime {
    return typeof task.dateTime === 'string' 
      ? DateTime.fromISO(task.dateTime) 
      : task.dateTime;
  }

  private groupByPeriod<T>(
    items: T[],
    period: 'day' | 'month',
    getDate: (item: T) => DateTime
  ): { labels: string[]; values: number[] } {
    const format = period === 'month' ? 'LLL yyyy' : 'dd.MM.yyyy';
    const grouped = new Map<string, number>();

    items.forEach(item => {
      const date = getDate(item);
      const key = date.toFormat(format);
      grouped.set(key, (grouped.get(key) || 0) + 1);
    });

    const sorted = Array.from(grouped.entries()).sort(([a], [b]) => {
      return DateTime.fromFormat(a, format).toMillis() - DateTime.fromFormat(b, format).toMillis();
    });

    return {
      labels: sorted.map(([label]) => label),
      values: sorted.map(([, count]) => count)
    };
  }

  private groupByStatus(tasks: Tasks[]): Record<string, number> {
    return tasks.reduce((acc, task) => {
      const eventType = this.translateStatus(task.status);
      acc[eventType] = (acc[eventType] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);
  }

  private translateStatus(status: string): string {
    const translations: Record<string, string> = {
      'Собеседование назначено': 'Техническое собеседование',
      'Прием назначен': 'Первичный прием',
      'Собеседование пройдено': 'Успешное собеседование',
      'Собеседование не пройдено': 'Неудачное собеседование',
      'Приём пройден': 'Успешный приём',
      'Приём не пройден': 'Неудачный приём',
      'Отказ кандидата': 'Отказ кандидата'
    };
    return translations[status] || status;
  }

  private createChartDataset(label: string, data: number[], color: string) {
    return {
      label,
      data,
      backgroundColor: color,
      borderColor: color,
      borderWidth: 1,
      fill: false
    };
  }

  private sortDates(a: string, b: string): number {
    // Для формата "месяц год" (LLL yyyy)
    if (a.match(/[а-яА-Я]+ \d{4}/) && b.match(/[а-яА-Я]+ \d{4}/)) {
      return DateTime.fromFormat(a, 'LLL yyyy').toMillis() - DateTime.fromFormat(b, 'LLL yyyy').toMillis();
    }
    // Для формата "день.месяц.год" (dd.MM.yyyy)
    return DateTime.fromFormat(a, 'dd.MM.yyyy').toMillis() - DateTime.fromFormat(b, 'dd.MM.yyyy').toMillis();
  }

  private isHR(user: User): boolean {
    return user.post.toLowerCase().includes('hr');
  }

  private isRecruiter(user: User): boolean {
    return user.post.toLowerCase().includes('рекрутер');
  }

  private onDateChange(): void {
    if (this.startDateControl.value && this.endDateControl.value) {
      this.activePeriod = 'custom';
    }
  }

  public applyDateFilter(): void {
    if (this.startDateControl.value && this.endDateControl.value) {
      this.loadTasks();
    }
  }

  private addSummarySheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Сводка');
    sheet.addRow(['Метрика', 'Значение']);
    sheet.addRow(['Всего принято', this.totalHired]);
    sheet.addRow(['Всего отказов', this.totalRejections]);
    sheet.addRow(['Запланировано мероприятий', this.totalEvents]);
    sheet.addRow(['Активных задач', this.totalActiveTasks]);
  }

  private addPeriodInfoSheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Период отчета');
    sheet.addRow(['Начало периода', this.startDateControl.value]);
    sheet.addRow(['Конец периода', this.endDateControl.value]);
  }
}