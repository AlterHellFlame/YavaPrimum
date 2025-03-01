import { Component, Input, OnInit } from '@angular/core';
import { CalendarComponent } from '../../common-ui/calendar/calendar.component';
import { CommonModule } from '@angular/common';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';
import { DateTime } from 'luxon';
import { TaskComponent } from '../../common-ui/tasks-components/task/task.component';
import { TaskInterviewComponent } from '../../common-ui/tasks-components/task/task-interview/task-interview.component';
import { AddTaskComponent } from '../../common-ui/tasks-components/add-task/add-task.component';
import { ChangeTaskComponent } from '../../common-ui/tasks-components/change-task/change-task.component';


@Component({
  selector: 'app-hr',
  standalone: true,
  imports: [CalendarComponent, CommonModule, TaskComponent, 
    TaskInterviewComponent, AddTaskComponent, ChangeTaskComponent],
  templateUrl: './hr.component.html',
  styleUrl: './hr.component.scss',
})
export class HrComponent implements OnInit {
  isHr: boolean = (localStorage.getItem('isHR') === 'true'? true : false);

  tasks: Tasks[] = [];
  loading: boolean = true;
  activeTask: Tasks = this.tasks[0]; 
  constructor(private taskService: TaskService)
  {}

  ngOnInit(): void {
    this.taskService.loadAllTasks().subscribe({
      next: data => {
        let allTasks = data.map(task => ({
          ...task,
          dateTime: DateTime.fromISO(task.dateTime as unknown as string)
        }));
        console.log(allTasks.length);
        this.taskService.setAllTasks(allTasks);
        this.getTasks(DateTime.now());
        this.loading = false;
      }
    });
    
  }

  public getTasks(day: any) : void
  {
    this.tasks = this.taskService.getTasksOfDay(day);
    console.log("Длинна " + this.tasks.length);
    this.activeTask = this.tasks[0];
  }

  public setActiveTask(task: any) : void
  {
    this.activeTask = task;
  }
}
