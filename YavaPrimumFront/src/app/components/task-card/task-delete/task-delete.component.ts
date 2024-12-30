import { Component, Input, OnInit } from '@angular/core';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';

@Component({
  selector: 'app-task-delete',
  imports: [],
  templateUrl: './task-delete.component.html',
  styleUrl: './task-delete.component.scss'
})
export class TaskDeleteComponent implements OnInit {
  task!: Tasks;

  constructor(private taskService: TaskService){}
  
  ngOnInit(): void 
  {
    this.taskService.taskClick$.subscribe(task =>
      {
        if(task != null)
        {
          console.log("candidate " + task!.candidate);
          this.task = task!;
        }
        else
        {
          console.error("Задачи нет");
        }
      })
  }

  public Delete()
  {
    this.taskService.DeleteTask(this.task.taskResponseId);
    window.location.reload();
  }
}
