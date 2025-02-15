import { Component } from '@angular/core';
import { CalendarComponent } from '../../common-ui/calendar/calendar.component';
import { CommonModule } from '@angular/common';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';
import { DateTime } from 'luxon';
import { HttpClientModule } from '@angular/common/http';
import { TaskComponent } from '../../common-ui/task/task.component';

@Component({
  selector: 'app-hr',
  imports: [CalendarComponent, CommonModule, HttpClientModule, TaskComponent],
  templateUrl: './hr.component.html',
  styleUrl: './hr.component.scss',
  providers: [TaskService]
})
export class HrComponent {

  tasks: Tasks[] = [];

  constructor(private taskService: TaskService){}

  public getTasks(day: any)
  {
    this.tasks = this.taskService.getTasksOfDay(day);
    console.log("Длинна " + this.tasks.length);
  }
}
