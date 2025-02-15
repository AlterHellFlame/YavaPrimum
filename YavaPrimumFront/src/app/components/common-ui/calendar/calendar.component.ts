import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { DateTime, Info, Interval } from 'luxon';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-calendar',
  imports: [CommonModule, HttpClientModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss',
  providers: [TaskService]
})
export class CalendarComponent {
  @Output() getTasks = new EventEmitter<any>();
  constructor(private taskService: TaskService) {}
  
  today: DateTime = DateTime.local();
  firstDayOfActiveMonth: DateTime = this.today.startOf('month');
  weekDays: string[] = Info.weekdays('short');
  daysOfMonth: DateTime[] = [];

  allTasks: Tasks[] = [];
  activeDay: DateTime = this.today;
  dayTaskLength: number = 0;

  ngOnInit(): void {
    /*this.taskService.getAllTasks().subscribe(data=>
      {
        this.allTasks = data.map(task => (
        {
          ...task,
          dateTime: DateTime.fromISO(task.dateTime as unknown as string)
        }));
        console.log("Все таски юзера " + this.allTasks);
  
        return this.allTasks;
      });*/

    this.allTasks = this.taskService.GetAllTasks();
    this.computeDaysOfMonth();
    this.setActiveDay(this.activeDay);
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
    console.log("ddd");
    this.activeDay = day;
    this.getTasks.emit(this.activeDay);
  }

  public getTasksForDay(day: DateTime): Tasks[] {
    return this.taskService.getTasksOfDay(day);
  }
  
  

  public goToPreviousMonth()
  {
    this.firstDayOfActiveMonth = this.firstDayOfActiveMonth.minus({month : 1})
    this.computeDaysOfMonth();
  }

  public goToFovardMonth()
  {
    this.firstDayOfActiveMonth = this.firstDayOfActiveMonth.plus({month : 1})
    this.computeDaysOfMonth();
  }

  public goToToday()
  {
    this.firstDayOfActiveMonth = this.today.startOf('month');
    this.computeDaysOfMonth();
  }

}
