import { Component, OnInit } from '@angular/core';
import { DateTime, Info, Interval } from 'luxon';
import { CommonModule } from '@angular/common';
import { Tasks } from '../../../../data/interface/Tasks.interface';
import { TaskService } from '../../../../services/task/task.service';

@Component({
  selector: 'app-calendar',
  imports: [CommonModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  today: DateTime = DateTime.local();
  firstDayOfActiveMonth: DateTime = this.today.startOf('month');
  weekDays: string[] = Info.weekdays('short');
  daysOfMonth: DateTime[] = [];

  allTasks: Tasks[] = [];
  activeDay: DateTime = this.today;
  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.allTasks$.subscribe(allTasks =>
    {
      this.allTasks = allTasks;
    });
    this.computeDaysOfMonth();
    this.taskService.setActiveDay(this.activeDay);
  }

  computeDaysOfMonth(): void {
    this.daysOfMonth = Interval.fromDateTimes(
      this.firstDayOfActiveMonth.startOf('week'),
      this.firstDayOfActiveMonth.endOf('month').endOf('week')
    ).splitBy({ day: 1 }).map((d) => {
      if (d.start == null) {
        throw new Error('Неверная дата');
      }
      return d.start;
    });
  }

  public setActiveDay(day: DateTime): void {
    this.activeDay = day;
    this.taskService.setActiveDay(this.activeDay);
  }

  public getTasksForDay(day: DateTime) : Tasks[]
  {
    return this.allTasks.filter(task => task.dateTime.hasSame(day, 'day'));
  }
}
