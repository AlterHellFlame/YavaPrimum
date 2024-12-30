import { Component, EventEmitter, input, Input, OnInit, Output } from '@angular/core';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskCardComponent } from '../task-card.component';
import { TaskService } from '../../../services/task/task.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-task-info',
  imports: [CommonModule],
  templateUrl: './task-info.component.html',
  styleUrl: './task-info.component.scss'
})
export class TaskInfoComponent implements OnInit {
  task!: Tasks;
  showElement: boolean = false; 

  constructor(private taskService: TaskService){}
  
  ngOnInit(): void {
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

  public PassedInterview()
  {
    this.taskService.PassedInterview(this.task);
  }

  public FaildInterview()
  {
    this.taskService.FaildInterview(this.task.taskResponseId);
  }

  public OpenNextInterview()
  {
    this.showElement = true;
  }
  public CloseNextInterview()
  {
    this.showElement = false;
  }

  public NextInterview()
  {
    this.CloseNextInterview();
  }
}
function createDefaultTasks(): Tasks {
  throw new Error('Function not implemented.');
}

