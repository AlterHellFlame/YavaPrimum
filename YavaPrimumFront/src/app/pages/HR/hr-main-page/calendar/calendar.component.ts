import { Component, OnInit } from '@angular/core';
import { DateTime, Info, Interval } from 'luxon';
import { CalendarDayComponent } from './calendar-day/calendar-day.component';
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

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.computeDaysOfMonth();
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
    this.taskService.setActiveDay(day);
  }
}
