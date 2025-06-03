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

// Types and constants
type ChartPeriod = 'week' | 'month' | 'quarter' | 'year' | 'custom';

interface StatusGroup {
  label: string;
  filter: (t: Tasks) => boolean;
  color: string;
}

const CHART_CONFIG = {
  colors: {
    hr: '#4A89DC',
    recruiter: '#D770AD',
    success: '#63B598',
    failure: '#E9573F',
    pending: '#F6BB42',
    events: '#3BAFDA',
    hires: '#37BC9B',
    rejections: '#DA4453'
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

const HR_STATUSES = [
  'Подтверждение даты',
  'Подтверждение времени',
  'Дата подтверждена',
  'Время подтверждено',
  'Дата была подтверждена',
  'Время отказано',
  'Дата отказана',
  'Запрос на смену времени',
  'Запрос на смену даты',
  'Назначено собеседование',
  'Выполнено тестовое задание',
  'Не выполнено тестовое задание'
];

const RECRUITER_STATUSES = [
  'Ожидается подтверждение даты',
  'Ожидается подтверждение времени',
  'Назначен приём',
  'Срок тестового задания',
];

const RESULT_STATUSES = [
  'Собеседование пройдено',
  'Собеседование не пройдено',
  'Не пришел',
  'Пришел',
  'Выполнено тестовое задание',
];

@Component({
  selector: 'app-hr-analytics',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, ReactiveFormsModule],
  templateUrl: './charts-page.component.html',
  styleUrls: ['./charts-page.component.scss']
})
export class ChartsPageComponent implements OnInit {
  // Form Controls
  startDateControl = new FormControl<string>('');
  endDateControl = new FormControl<string>('');
  activePeriod: ChartPeriod = 'month';
  
  // UI Config
  periods = [
    { label: 'Неделя', value: 'week' as ChartPeriod },
    { label: 'Месяц', value: 'month' as ChartPeriod },
    { label: 'Квартал', value: 'quarter' as ChartPeriod },
    { label: 'Год', value: 'year' as ChartPeriod },
    { label: 'Вручную', value: 'custom' as ChartPeriod }
  ];

  // Stats
  totalHires = 0;
  totalRejections = 0;
  totalPending = 0;
  totalCompletedTasks = 0;
  totalActiveTasks = 0;
  hrEfficiency = 0;
  recruiterEfficiency = 0;

  // Charts
  hiresChart: ChartData<'line'> = { labels: [], datasets: [] };
  rejectionsChart: ChartData<'line'> = { labels: [], datasets: [] };
  hrWorkloadChart: ChartData<'bar'> = { labels: [], datasets: [] };
  recruiterWorkloadChart: ChartData<'bar'> = { labels: [], datasets: [] };
  eventsPlanChart: ChartData<'bar'> = { labels: [], datasets: [] };

  chartOptions: ChartOptions = {
    responsive: true,
    scales: {
      x: { title: { display: true, text: 'Период' } },
      y: { 
        title: { display: true, text: 'Количество' }, 
        beginAtZero: true,
        ticks: { precision: 0 }
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
    this.setPeriod('month');
    this.loadTasks();
    
    this.startDateControl.valueChanges.subscribe(() => this.onDateChange());
    this.endDateControl.valueChanges.subscribe(() => this.onDateChange());
  }

  setPeriod(period: ChartPeriod): void {
    this.activePeriod = period;
    let startDate: DateTime;
    const endDate = DateTime.now();

    switch (period) {
      case 'week':
        startDate = endDate.minus({ weeks: 1 });
        break;
      case 'month':
        startDate = endDate.minus({ months: 1 });
        break;
      case 'quarter':
        startDate = endDate.minus({ months: 3 });
        break;
      case 'year':
        startDate = endDate.minus({ years: 1 });
        break;
      case 'custom':
        return;
      default:
        startDate = endDate.minus({ months: 1 });
    }

    this.startDateControl.setValue(startDate.toFormat('yyyy-MM-dd'));
    this.endDateControl.setValue(endDate.toFormat('yyyy-MM-dd'));
    this.applyDateFilter();
  }

  async exportToExcel(): Promise<void> {
    const workbook = new ExcelJS.Workbook();
    workbook.creator = 'HR Analytics System';
    workbook.created = new Date();

    this.addSummarySheet(workbook);
    this.addHiresAnalysisSheet(workbook);
    this.addRejectionsAnalysisSheet(workbook);
    this.addHrPerformanceSheet(workbook);
    this.addRecruiterPerformanceSheet(workbook);
    this.addEventsPlanSheet(workbook);
    this.addPeriodInfoSheet(workbook);

    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), `HR_Analytics_${DateTime.now().toFormat('yyyy-MM-dd')}.xlsx`);
  }

  private loadTasks(): void {
    const start = this.startDateControl.value ? DateTime.fromISO(this.startDateControl.value).startOf('day') : null;
    const end = this.endDateControl.value ? DateTime.fromISO(this.endDateControl.value).endOf('day') : null;
    
    if (!start || !end) return;
    
    this.taskService.getAllTasks().subscribe({
      next: tasks => {
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
    this.generateHiresChart();
    this.generateRejectionsChart();
    this.generateHrWorkloadChart();
    this.generateRecruiterWorkloadChart();
    this.generateEventsPlanChart();
  }

  private generateHiresChart(): void {
    const hiresData = this.groupByPeriod(
      this.allTasks.filter(t => t.status === 'Взят кандидат' || t.status === 'Собеседование пройдено'),
      this.getGroupBy(),
      this.getTaskDate
    );

    this.hiresChart = {
      labels: hiresData.labels,
      datasets: [
        this.createChartDataset('Принятые сотрудники', hiresData.values, CHART_CONFIG.colors.hires)
      ]
    };
  }

  private generateRejectionsChart(): void {
    const rejectionsData = this.groupByPeriod(
      this.allTasks.filter(t => 
        t.status === 'Собеседование не пройдено' || 
        t.status === 'Не пришел' ||
        t.status === 'Не выполнено тестовое задание'
      ),
      this.getGroupBy(),
      this.getTaskDate
    );

    this.rejectionsChart = {
      labels: rejectionsData.labels,
      datasets: [
        this.createChartDataset('Отказы', rejectionsData.values, CHART_CONFIG.colors.rejections)
      ]
    };
  }

  private generateHrWorkloadChart(): void {
    const hrTasks = this.allTasks.filter(t => this.isHRTask(t));
    const statusGroups = this.groupHrTasksByStatus(hrTasks);

    this.hrWorkloadChart = {
      labels: Object.keys(statusGroups),
      datasets: [
        this.createChartDataset('Задачи HR', Object.values(statusGroups), CHART_CONFIG.colors.hr)
      ]
    };
  }

  
  private generateRecruiterWorkloadChart(): void {
    const recruiterTasks = this.allTasks.filter(t => this.isRecruiterTask(t));
    const statusGroups = this.groupRecruiterTasksByStatus(recruiterTasks);

    this.recruiterWorkloadChart = {
      labels: Object.keys(statusGroups),
      datasets: [
        this.createChartDataset('Задачи рекрутеров', Object.values(statusGroups), CHART_CONFIG.colors.recruiter)
      ]
    };
  }
private generateEventsPlanChart(): void {
  // Фильтруем задачи по статусам мероприятий
  const interviewTasks = this.allTasks.filter(t => t.status === 'Назначено собеседование');
  const receptionTasks = this.allTasks.filter(t => t.status === 'Назначен приём');
  const testTasks = this.allTasks.filter(t => t.status === 'Срок тестового задания');

  // Сначала получаем все возможные периоды
  const allEvents = [...interviewTasks, ...receptionTasks, ...testTasks];
  const allPeriodsData = this.groupByPeriod(allEvents, this.getGroupBy(), this.getTaskDate);
  const allLabels = allPeriodsData.labels;

  // Функция для заполнения данных с учетом всех периодов
  const getValuesForAllPeriods = (tasks: Tasks[]) => {
    const groupedData = this.groupByPeriod(tasks, this.getGroupBy(), this.getTaskDate);
    const valuesMap = new Map<string, number>();
    
    groupedData.labels.forEach((label, index) => {
      valuesMap.set(label, groupedData.values[index]);
    });

    // Заполняем значения для всех периодов (если нет данных - 0)
    return allLabels.map(label => valuesMap.get(label) || 0);
  };

  // Создаем наборы данных для каждого типа мероприятий
  const datasets = [
    this.createChartDataset('Собеседования', getValuesForAllPeriods(interviewTasks), CHART_CONFIG.colors.hr),
    this.createChartDataset('Приёмы', getValuesForAllPeriods(receptionTasks), CHART_CONFIG.colors.recruiter),
    this.createChartDataset('Тестовые задания', getValuesForAllPeriods(testTasks), CHART_CONFIG.colors.rejections)
  ];

  this.eventsPlanChart = {
    labels: allLabels,
    datasets: datasets
  };
}


  private calculateSummary(): void {
    // Hires
    this.totalHires = this.allTasks.filter(t => 
     t.status === 'Пришел'
    ).length;

    // Rejections
    this.totalRejections = this.allTasks.filter(t => 
      t.status === 'Собеседование не пройдено' || 
      t.status === 'Не пришел' ||
      t.status === 'Не выполнено тестовое задание'
    ).length;

    // Pending
    this.totalPending = this.allTasks.filter(t => 
      t.status.includes('Ожидается') || 
      t.status.includes('Подтверждение') ||
      t.status === 'Перенесено собеседование'
    ).length;

    // Tasks
    const hrTasks = this.allTasks.filter(t => this.isHRTask(t));
    const recruiterTasks = this.allTasks.filter(t => this.isRecruiterTask(t));

    this.totalCompletedTasks = [
      ...hrTasks.filter(t => t.typeStatus === 2),
      ...recruiterTasks.filter(t => t.typeStatus === 2)
    ].length;

    this.totalActiveTasks = [
      ...hrTasks.filter(t => t.typeStatus !== 2),
      ...recruiterTasks.filter(t => t.typeStatus !== 2)
    ].length;

    // Efficiency
    const totalHrTasks = hrTasks.length;
    const completedHrTasks = hrTasks.filter(t => t.typeStatus === 2).length;
    this.hrEfficiency = totalHrTasks > 0 ? Math.round((completedHrTasks / totalHrTasks) * 100) : 0;

    const totalRecruiterTasks = recruiterTasks.length;
    const completedRecruiterTasks = recruiterTasks.filter(t => t.typeStatus === 2).length;
    this.recruiterEfficiency = totalRecruiterTasks > 0 ? Math.round((completedRecruiterTasks / totalRecruiterTasks) * 100) : 0;
  }

  private getTaskDate(task: Tasks): DateTime {
    return typeof task.dateTime === 'string' 
      ? DateTime.fromISO(task.dateTime) 
      : task.dateTime;
  }

  private getGroupBy(): 'day' | 'week' | 'month' {
    switch (this.activePeriod) {
      case 'week': return 'day';
      case 'month': return 'week';
      case 'quarter': return 'month';
      case 'year': return 'month';
      default: return 'day';
    }
  }

  private groupByPeriod<T>(
    items: T[],
    period: 'day' | 'week' | 'month',
    getDate: (item: T) => DateTime
  ): { labels: string[]; values: number[] } {
    let format: string;
    switch (period) {
      case 'day': 
        format = 'dd.MM.yyyy';
        break;
      case 'week':
        format = 'ww (LLL) yyyy';
        break;
      case 'month':
        format = 'LLL yyyy';
        break;
      default:
        format = 'dd.MM.yyyy';
    }

    const grouped = new Map<string, number>();

    items.forEach(item => {
      const date = getDate(item);
      let key: string;
      
      if (period === 'week') {
        const weekNumber = date.weekNumber;
        const month = date.toFormat('LLL');
        key = `${weekNumber} (${month}) ${date.year}`;
      } else {
        key = date.toFormat(format);
      }
      
      grouped.set(key, (grouped.get(key) || 0) + 1);
    });

    const sorted = Array.from(grouped.entries()).sort(([a], [b]) => {
      return this.sortDates(a, b, period);
    });

    return {
      labels: sorted.map(([label]) => label),
      values: sorted.map(([, count]) => count)
    };
  }

  private groupHrTasksByStatus(tasks: Tasks[]): Record<string, number> {
    const groups: Record<string, number> = {
      'Назначенные собеседования': 0,
      'Завершенные успешно': 0,
      'Завершенные провально': 0
    };

    tasks.forEach(task => {
      if (task.status === 'Назначено собеседование') {
        groups['Назначенные собеседования']++;
      }  else if (task.status.includes('Собеседование не пройдено') || task.status === 'Не выполнено тестовое задание'){
        groups['Завершенные провально']++;
      } else if (task.status.includes('Собеседование пройдено') || task.status === 'Выполнено тестовое задание'){
        groups['Завершенные успешно']++;
      }
    });

    return groups;
  }

  private groupRecruiterTasksByStatus(tasks: Tasks[]): Record<string, number> {
    const groups: Record<string, number> = {
      'Назначенные приёмы': 0,
      'Завершенные успешно': 0,
      'Завершенные провально': 0
    };

    tasks.forEach(task => {
      if (task.status === 'Назначен приём') {
        groups['Назначенные приёмы']++;
      }  else if (task.status.includes('Не пришел')){
        groups['Завершенные провально']++;
      } else if (task.status.includes('Пришел')){
        groups['Завершенные успешно']++;
      }
    });

    return groups;
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

  private sortDates(a: string, b: string, period: 'day' | 'week' | 'month' = 'day'): number {
    if (period === 'week') {
      const parseWeek = (s: string) => {
        const match = s.match(/(\d+) \((.+)\) (\d+)/);
        if (!match) return 0;
        const [_, week, month, year] = match;
        return DateTime.fromFormat(`${week} ${month} ${year}`, 'W LLL yyyy').toMillis();
      };
      return parseWeek(a) - parseWeek(b);
    } else if (period === 'month') {
      return DateTime.fromFormat(a, 'LLL yyyy').toMillis() - DateTime.fromFormat(b, 'LLL yyyy').toMillis();
    }
    return DateTime.fromFormat(a, 'dd.MM.yyyy').toMillis() - DateTime.fromFormat(b, 'dd.MM.yyyy').toMillis();
  }

  private isHRTask(task: Tasks): boolean {
    return HR_STATUSES.includes(task.status) || 
           (task.user && task.user.post.toLowerCase().includes('hr'));
  }

  private isRecruiterTask(task: Tasks): boolean {
    return RECRUITER_STATUSES.includes(task.status) || 
           RESULT_STATUSES.includes(task.status) ||
           (task.user && task.user.post.toLowerCase().includes('рекрутер'));
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

  // Excel Export Helpers
  private addSummarySheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Сводка');
    
    // Header
    sheet.addRow(['Ключевые показатели HR аналитики']).font = { bold: true, size: 14 };
    sheet.addRow([]);
    
    // Period info
    sheet.addRow(['Период:', `${this.startDateControl.value} - ${this.endDateControl.value}`]);
    sheet.addRow([]);
    
    // Stats
    sheet.addRow(['Метрика', 'Значение']);
    sheet.addRow(['Всего принято на работу', this.totalHires]);
    sheet.addRow(['Всего отказов', this.totalRejections]);
    sheet.addRow(['Ожидающие подтверждения', this.totalPending]);
    sheet.addRow(['Завершенные задачи', this.totalCompletedTasks]);
    sheet.addRow(['Активные задачи', this.totalActiveTasks]);
    sheet.addRow(['Эффективность HR (%)', this.hrEfficiency]);
    sheet.addRow(['Эффективность рекрутеров (%)', this.recruiterEfficiency]);
    
    // Formatting
    sheet.getColumn(1).width = 30;
    sheet.getColumn(2).width = 20;
  }

  private addHiresAnalysisSheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Анализ принятых');
    sheet.addRow(['Анализ принятых сотрудников по периодам']).font = { bold: true, size: 14 };
    sheet.addRow(['Период', 'Количество']);
    
    this.hiresChart.labels!.forEach((label, i) => {
      sheet.addRow([label, this.hiresChart.datasets[0].data[i]]);
    });
    
    sheet.getColumn(1).width = 20;
    sheet.getColumn(2).width = 15;
  }

  private addRejectionsAnalysisSheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Анализ отказов');
    sheet.addRow(['Анализ отказов по периодам']).font = { bold: true, size: 14 };
    sheet.addRow(['Период', 'Количество']);
    
    this.rejectionsChart.labels!.forEach((label, i) => {
      sheet.addRow([label, this.rejectionsChart.datasets[0].data[i]]);
    });
    
    sheet.getColumn(1).width = 20;
    sheet.getColumn(2).width = 15;
  }

  private addHrPerformanceSheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Работа HR');
    sheet.addRow(['Анализ работы HR отдела']).font = { bold: true, size: 14 };
    sheet.addRow(['Тип задачи', 'Количество']);
    
    Object.keys(this.hrWorkloadChart.labels!).forEach((label, i) => {
      sheet.addRow([label, this.hrWorkloadChart.datasets[0].data[i]]);
    });
    
    sheet.getColumn(1).width = 25;
    sheet.getColumn(2).width = 15;
  }

  private addRecruiterPerformanceSheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Работа рекрутеров');
    sheet.addRow(['Анализ работы рекрутеров']).font = { bold: true, size: 14 };
    sheet.addRow(['Тип задачи', 'Количество']);
    
    Object.keys(this.recruiterWorkloadChart.labels!).forEach((label, i) => {
      sheet.addRow([label, this.recruiterWorkloadChart.datasets[0].data[i]]);
    });
    
    sheet.getColumn(1).width = 25;
    sheet.getColumn(2).width = 15;
  }

  private addEventsPlanSheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('План мероприятий');
    sheet.addRow(['План мероприятий по периодам']).font = { bold: true, size: 14 };
    sheet.addRow(['Период', 'Количество']);
    
    this.eventsPlanChart.labels!.forEach((label, i) => {
      sheet.addRow([label, this.eventsPlanChart.datasets[0].data[i]]);
    });
    
    sheet.getColumn(1).width = 20;
    sheet.getColumn(2).width = 15;
  }

  private addPeriodInfoSheet(workbook: ExcelJS.Workbook): void {
    const sheet = workbook.addWorksheet('Информация о периоде');
    sheet.addRow(['Начало периода', this.startDateControl.value]);
    sheet.addRow(['Конец периода', this.endDateControl.value]);
    sheet.addRow(['Тип периода', this.periods.find(p => p.value === this.activePeriod)?.label]);
  }
}