import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { DateTime, Info, Interval } from 'luxon';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';
import { CommonModule } from '@angular/common';
import { NotifyService } from '../../../services/notify/notify.service';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss'],
})
export class CalendarComponent implements OnInit {
  @Output() getTasks = new EventEmitter<any>();

  today: DateTime = DateTime.local().minus({ days: 1 });
  firstDayOfActiveMonth: DateTime = this.today.startOf('month');
  weekDays: string[] = Info.weekdays('short');
  daysOfMonth: DateTime[] = [];

  allTasks: Tasks[] = [];
  activeDay: DateTime = this.today;
  dayTaskLength: number = 0;

  constructor(private taskService: TaskService, private notify: NotifyService) {}

  ngOnInit(): void {
    console.log("Запрос");
    this.allTasks = this.taskService.getAllTasksOfUser();
    this.computeDaysOfMonth();
    this.setActiveDay(this.today);

     this.notify.addReceiveListener((message) => {
        this.allTasks = this.taskService.getAllTasksOfUser();
    });
  }

  computeDaysOfMonth(): void {
    this.daysOfMonth = Interval.fromDateTimes(
      this.firstDayOfActiveMonth.startOf('week'),
      this.firstDayOfActiveMonth.endOf('month').endOf('week')
    ).splitBy({ day: 1 }).map(d => {
      if (d.start == null) {
        throw new Error('Неверная дата');
      }
      return d.start;
    });
  }

  public setActiveDay(day: DateTime): void {
    this.activeDay = day;
    this.getTasks.emit(day);
  }

  public getTasksForDay(day: DateTime): Tasks[] {
    let tasks = this.taskService.getTasksOfDay(day);
    this.dayTaskLength = tasks.length;
    return tasks;
  }

  public goToPreviousMonth(): void {
    this.firstDayOfActiveMonth = this.firstDayOfActiveMonth.minus({ month: 1 });
    this.computeDaysOfMonth();
  }

  public goToFowardMonth(): void {
    this.firstDayOfActiveMonth = this.firstDayOfActiveMonth.plus({ month: 1 });
    this.computeDaysOfMonth();
  }

  public goToToday(): void {
    this.firstDayOfActiveMonth = this.today.startOf('month');
    this.computeDaysOfMonth();
  }

public isDateNow(date: DateTime): boolean {
    const today = DateTime.now().startOf('day'); // Получаем текущую дату без времени
    const inputDate = date.startOf('day'); // Убираем время у переданной даты

    return today <= inputDate; // Проверяем, меньше ли сегодня, чем переданная дата
}
}
