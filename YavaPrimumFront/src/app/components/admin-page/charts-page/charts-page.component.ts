import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../../services/task/task.service';
import { ChartData, ChartOptions } from 'chart.js';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import * as XLSX from 'xlsx';

// Константы для повторяющихся значений
const CHART_COLORS = {
  HR: {
    bg: 'rgba(75, 192, 192, 0.2)',
    border: 'rgba(75, 192, 192, 1)'
  },
  RECRUITER: {
    bg: 'rgba(153, 102, 255, 0.2)',
    border: 'rgba(153, 102, 255, 1)'
  },
  REJECTION: {
    bg: 'rgba(255, 99, 132, 0.6)',
    border: 'rgba(255, 99, 132, 1)'
  },
  ACCEPTED: {
    bg: 'rgba(75, 192, 192, 0.6)',
    border: 'rgba(75, 192, 192, 1)'
  },
  RECEPTION: {
    bg: 'rgba(75, 192, 192, 0.2)',
    border: 'rgba(75, 192, 192, 1)'
  },
  INTERVIEW: {
    bg: 'rgba(255, 159, 64, 0.2)',
    border: 'rgba(255, 159, 64, 1)'
  }
};

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
  readonly chartOptions: ChartOptions = {
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
    this.loadTasks();
  }

  private loadTasks(): void {
    this.taskService.getAllArchiveTasks().subscribe((tasks) => {
      this.allTasks = tasks;
      this.generateReports();
    });
  }

  private generateReports(): void {
    this.generateHRPerformanceReport();
    this.generateRecruitmentStatsReport();
    this.generateRejectionStatsReport();
    this.generateEventPlanReport();
  }

  private generateHRPerformanceReport(): void {
    const hrTasks = this.filterTasksByPost('HR');
    const recruiterTasks = this.filterTasksByPost('Кадровик');

    const hrProgress = this.countCompletedTasks(hrTasks);
    const recruiterProgress = this.countCompletedTasks(recruiterTasks);

    this.hrPerformanceChartData = {
      labels: ['HR', 'Кадровик'],
      datasets: [{
        label: 'Прогресс задач',
        data: [hrProgress, recruiterProgress],
        backgroundColor: [CHART_COLORS.HR.bg, CHART_COLORS.RECRUITER.bg],
        borderColor: [CHART_COLORS.HR.border, CHART_COLORS.RECRUITER.border],
        borderWidth: 1
      }]
    };
  }

  private generateRecruitmentStatsReport(): void {
    const recruitmentTasks = this.filterTasksByStatus('Пришел');
    const groupedData = this.groupTasksByDate(recruitmentTasks);

    this.recruitmentStatsChartData = {
      labels: groupedData.labels,
      datasets: [{
        label: 'Принятые сотрудники',
        data: groupedData.data,
        backgroundColor: '#b4ff63',
        borderColor: '#b4ff63'
      }]
    };
  }

  private generateRejectionStatsReport(): void {
    const rejectionTasks = this.allTasks.filter(task => 
      task.status === 'Собеседование не пройдено' || task.status === 'Не пришел'
    );
    const groupedData = this.groupTasksByDate(rejectionTasks);

    this.rejectionStatsChartData = {
      labels: groupedData.labels,
      datasets: [{
        label: 'Отказы',
        data: groupedData.data,
        backgroundColor: CHART_COLORS.REJECTION.bg,
        borderColor: CHART_COLORS.REJECTION.border,
        borderWidth: 2,
        hoverOffset: 10,
        borderRadius: 5,
        spacing: 5
      }]
    };
  }

  private generateEventPlanReport(): void {
    const receptionTasks = this.filterTasksByStatus('Назначен приём');
    const interviewTasks = this.filterTasksByStatus('Назначено собеседование');
    
    const receptionData = this.groupTasksByDate(receptionTasks);
    const interviewData = this.groupTasksByDate(interviewTasks);
    
    const allDates = this.getUniqueDates([...receptionData.labels, ...interviewData.labels]);

    this.eventPlanChartData = {
      labels: allDates,
      datasets: [
        this.createDataset('Назначен приём', receptionData, allDates, CHART_COLORS.RECEPTION),
        this.createDataset('Назначено собеседование', interviewData, allDates, CHART_COLORS.INTERVIEW)
      ]
    };
  }

  private filterTasksByPost(post: string): Tasks[] {
    return this.allTasks.filter(task => task.user.post === post);
  }

  private filterTasksByStatus(status: string): Tasks[] {
    return this.allTasks.filter(task => task.status === status);
  }

  private countCompletedTasks(tasks: Tasks[]): number {
    return tasks.filter(task => task.typeStatus === 2).length;
  }

  private groupTasksByDate(tasks: Tasks[]): { labels: string[]; data: number[] } {
    const grouped = tasks.reduce((acc, task) => {
      const date = task.dateTime.toString().split('T')[0];
      acc[date] = (acc[date] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);

    const sortedEntries = Object.entries(grouped).sort((a, b) => 
      new Date(a[0]).getTime() - new Date(b[0]).getTime()
    );

    return {
      labels: sortedEntries.map(([date]) => date),
      data: sortedEntries.map(([_, count]) => count)
    };
  }

  private getUniqueDates(dates: string[]): string[] {
    return [...new Set(dates)].sort((a, b) => 
      new Date(a).getTime() - new Date(b).getTime()
    );
  }

  private createDataset(
    label: string, 
    groupedData: { labels: string[]; data: number[] }, 
    allDates: string[], 
    colors: { bg: string; border: string }
  ) {
    return {
      label,
      data: allDates.map(date => 
        groupedData.data[groupedData.labels.indexOf(date)] || 0
      ),
      backgroundColor: colors.bg,
      borderColor: colors.border,
      borderWidth: 1
    };
  }



  exportToExcel(): void {
    // Определяем стили для Excel
    const styles = {
      header: {
        font: { bold: true, color: { rgb: 'FFFFFF' } },
        fill: { fgColor: { rgb: '4472C4' } }, // Синий цвет заголовка
        alignment: { horizontal: 'center' }
      },
      title: {
        font: { bold: true, size: 14, color: { rgb: '000000' } },
        fill: { fgColor: { rgb: 'D9E1F2' } } // Светло-синий фон
      },
      dataHeader: {
        font: { bold: true },
        fill: { fgColor: { rgb: 'E9E9E9' } } // Серый фон
      },
      dataRow: {
        font: { color: { rgb: '000000' } }
      }
    };
  
    // Создаем книгу
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
  
    // Добавляем отчеты
    this.addReportToWorkbook(wb, 'Статистика отказов', this.rejectionStatsChartData, styles);
    this.addReportToWorkbook(wb, 'План мероприятий', this.eventPlanChartData, styles);
    this.addReportToWorkbook(wb, 'Статистика по найму', this.recruitmentStatsChartData, styles);
    this.addReportToWorkbook(wb, 'Работа HR и рекрутеров', this.hrPerformanceChartData, styles);
  
    // Добавляем сводный лист
    this.addSummarySheet(wb, styles);
  
    // Сохраняем файл
    XLSX.writeFile(wb, 'HR_Отчеты.xlsx', { bookSST: true });
  }
  
  private addReportToWorkbook(
    wb: XLSX.WorkBook,
    title: string,
    chartData: ChartData<any, any, any>,
    styles: any
  ): void {
    if (!chartData.labels || !chartData.datasets?.[0]?.data) return;
  
    const data = [
      // Заголовок отчета
      [{ v: title, t: 's', s: styles.title }],
      [], // Пустая строка
  
      // Заголовки столбцов
      [
        { v: 'Период', t: 's', s: styles.dataHeader },
        { v: 'Количество', t: 's', s: styles.dataHeader }
      ],
  
      // Данные
      ...chartData.labels.map((label, i) => [
        { v: label, t: 's', s: styles.dataRow },
        { v: chartData.datasets[0].data[i], t: 'n', s: styles.dataRow }
      ]),
  
/*      // Итого
      [
        { v: 'Итого', t: 's', s: styles.dataHeader },
        { 
          v: chartData.datasets[0].data.reduce((sum, val) => sum + val, 0) || 0;
          t: 'n',
          s: styles.dataHeader
        }
      ],*/
  
      [], [], // Пустые строки для разделения
    ];
  
    const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(data);
    
    // Настраиваем ширину столбцов
    ws['!cols'] = [{ width: 20 }, { width: 15 }];
    
    // Добавляем лист в книгу
    XLSX.utils.book_append_sheet(wb, ws, title.substring(0, 31)); // Ограничение длины имени листа
  }
  
  private addSummarySheet(wb: XLSX.WorkBook, styles: any): void {
    const summaryData = [
      // Заголовок
      [{ v: 'Сводный отчет', t: 's', s: styles.title }],
      [],
      
      // Заголовки столбцов
      [
        { v: 'Отчет', t: 's', s: styles.header },
        { v: 'Всего', t: 's', s: styles.header }
      ],
      
      // Данные
      ...this.getSummaryRows(styles),
      
      // Пустые строки
      [], []
    ];
  
    const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(summaryData);
    
    // Настраиваем ширину столбцов
    ws['!cols'] = [{ width: 30 }, { width: 15 }];
    
    // Добавляем лист в начало книги
    wb.SheetNames.unshift('Сводный отчет');
    wb.Sheets['Сводный отчет'] = ws;
  }
  
  private getSummaryRows(styles: any): any[] {
    const reports = [
      { name: 'Статистика отказов', chart: this.rejectionStatsChartData },
      { name: 'План мероприятий', chart: this.eventPlanChartData },
      { name: 'Статистика по найму', chart: this.recruitmentStatsChartData },
      { name: 'Работа HR и рекрутеров', chart: this.hrPerformanceChartData }
    ];
  
    return reports.map(report => {
      const total = report.chart.datasets?.[0]?.data?.reduce((sum, val) => sum + val, 0) || 0;
      
      return [
        { v: report.name, t: 's', s: styles.dataRow },
        { v: total, t: 'n', s: styles.dataRow }
      ];
    });
  }
}