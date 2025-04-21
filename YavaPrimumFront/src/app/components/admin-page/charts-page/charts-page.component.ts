import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../../services/task/task.service';
import { ChartData, ChartOptions } from 'chart.js';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
//import * as XLSX from 'xlsx';
import * as ExcelJS from 'exceljs';

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

const EXCEL_STYLES = {
  header: {
    fill: { type: 'pattern', pattern: 'solid', fgColor: { argb: '4472C4' } },
    font: { bold: true, color: { argb: 'FFFFFF' } },
    alignment: { horizontal: 'center' }
  },
  title: {
    fill: { type: 'pattern', pattern: 'solid', fgColor: { argb: 'D9E1F2' } },
    font: { bold: true, size: 14 }
  },
  dataHeader: {
    fill: { type: 'pattern', pattern: 'solid', fgColor: { argb: 'E9E9E9' } },
    font: { bold: true }
  },
  dataRow: {
    font: { color: { argb: '000000' } }
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



  async exportToExcel(): Promise<void> {
    // Создаем новую книгу Excel
    /*const workbook = new ExcelJS.Workbook();
    workbook.creator = 'HR System';
    workbook.created = new Date();

    // Добавляем отчеты
    await this.addReportToWorkbook(workbook, 'Статистика отказов', this.rejectionStatsChartData);
    await this.addReportToWorkbook(workbook, 'План мероприятий', this.eventPlanChartData);
    await this.addReportToWorkbook(workbook, 'Статистика по найму', this.recruitmentStatsChartData);
    await this.addReportToWorkbook(workbook, 'Работа HR и рекрутеров', this.hrPerformanceChartData);

    // Добавляем сводный лист
    await this.addSummarySheet(workbook);

    // Генерируем файл и сохраняем
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), 'HR_Отчеты.xlsx');
  }

  private async addReportToWorkbook(
    workbook: ExcelJS.Workbook,
    title: string,
    chartData: ChartData<any, any, any>
  ): Promise<void> {
    if (!chartData.labels || !chartData.datasets?.[0]?.data) return;

    const worksheet = workbook.addWorksheet(title.substring(0, 31)); // Ограничение длины имени листа

    // Заголовок отчета
    const titleRow = worksheet.addRow([title]);
    titleRow.font = EXCEL_STYLES.title.font;
    titleRow.fill = EXCEL_STYLES.title.fill;
    worksheet.addRow([]); // Пустая строка

    // Заголовки столбцов
    const headerRow = worksheet.addRow(['Период', 'Количество']);
    headerRow.eachCell(cell => {
      cell.style = EXCEL_STYLES.dataHeader;
    });

    // Данные
    chartData.labels.forEach((label, i) => {
      const row = worksheet.addRow([label, chartData.datasets[0].data[i]]);
      row.eachCell(cell => {
        cell.style = EXCEL_STYLES.dataRow;
      });
    });

    // Итого
    const total = chartData.datasets[0].data.reduce((sum, val) => sum + val, 0);
    const totalRow = worksheet.addRow(['Итого', total]);
    totalRow.eachCell(cell => {
      cell.style = EXCEL_STYLES.dataHeader;
    });

    // Настраиваем ширину столбцов
    worksheet.columns = [
      { width: 20 },
      { width: 15 }
    ];
  }

  private async addSummarySheet(workbook: ExcelJS.Workbook): Promise<void> {
    const worksheet = workbook.addWorksheet('Сводный отчет', 0); // Добавляем в начало

    // Заголовок
    const titleRow = worksheet.addRow(['Сводный отчет']);
    titleRow.font = EXCEL_STYLES.title.font;
    titleRow.fill = EXCEL_STYLES.title.fill;
    worksheet.addRow([]); // Пустая строка

    // Заголовки столбцов
    const headerRow = worksheet.addRow(['Отчет', 'Всего']);
    headerRow.eachCell(cell => {
      cell.style = EXCEL_STYLES.header;
    });

    // Данные
    const reports = [
      { name: 'Статистика отказов', chart: this.rejectionStatsChartData },
      { name: 'План мероприятий', chart: this.eventPlanChartData },
      { name: 'Статистика по найму', chart: this.recruitmentStatsChartData },
      { name: 'Работа HR и рекрутеров', chart: this.hrPerformanceChartData }
    ];

    reports.forEach(report => {
      const total = report.chart.datasets?.[0]?.data?.reduce((sum, val) => sum + val, 0) || 0;
      const row = worksheet.addRow([report.name, total]);
      row.eachCell(cell => {
        cell.style = EXCEL_STYLES.dataRow;
      });
    });

    // Настраиваем ширину столбцов
    worksheet.columns = [
      { width: 30 },
      { width: 15 }
    ];*/
  }
}