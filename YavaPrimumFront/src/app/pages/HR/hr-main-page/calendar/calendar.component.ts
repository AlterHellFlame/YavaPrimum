import { Component, computed, OnInit, signal, Signal, WritableSignal } from '@angular/core';
import { DateTime, Info, Interval } from 'luxon';
import { CalendarDayComponent } from './calendar-day/calendar-day.component';
import { CommonModule } from '@angular/common';
import { Tasks } from '../../../../data/interface/Tasks.interface';
import { TaskService } from '../../../../services/task/task.service';

@Component({
  selector: 'app-calendar',
  imports: [CommonModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss'
})
export class CalendarComponent implements OnInit
{
  constructor(private taskService : TaskService){}
  today: Signal<DateTime> = signal(DateTime.local());

  ngOnInit(): void {
  }

  fistDayOfActiveMonth: WritableSignal<DateTime> = signal( 
    this.today().startOf('month')
  );
  
  weekDays : Signal<string[]> = signal(Info.weekdays('short'));

  daysOfMonth : Signal<DateTime[]> = computed(() => 
    {
      return Interval.fromDateTimes(
        this.fistDayOfActiveMonth().startOf('week'),
        this.fistDayOfActiveMonth().endOf('month').endOf('week')
      ).splitBy({day:1}).map((d) => 
      {
        if (d.start == null)
        {
          throw new Error('Неверная дата')
        }
        return d.start;
      });
    });

    public setActiveDay(day : DateTime) : void
    {
      this.taskService.setActiveDay(day);
    }
}