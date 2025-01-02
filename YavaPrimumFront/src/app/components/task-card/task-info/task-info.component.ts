import { AfterViewInit, Component, ElementRef, EventEmitter, input, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskCardComponent } from '../task-card.component';
import { TaskService } from '../../../services/task/task.service';
import { CommonModule } from '@angular/common';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-task-info',
  imports: [CommonModule],
  templateUrl: './task-info.component.html',
  styleUrl: './task-info.component.scss'
})
export class TaskInfoComponent implements OnInit, AfterViewInit {
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
    this.taskService.PassedInterview(this.task.taskResponseId);
    window.location.reload();
  }

  public FaildInterview()
  {
    this.taskService.FaildInterview(this.task.taskResponseId);
    window.location.reload();
  }
  
  ngAfterViewInit()
  { // Убедимся, что dateTimeInput доступен после инициализации представления 
    if (!this.dateTimeInput) 
    { 
      console.error('DateTime input element is still not found!'); 
    } 
    else
    {
      console.log('Всё ок'); 
    }
  }

  @ViewChild('dateTime') dateTimeInput!: ElementRef;
  public NextInterview()
  {
      this.CloseNextInterview();
  
      if (!this.dateTimeInput || !this.dateTimeInput.nativeElement)
      {
          console.error('DateTime input element is not found!');
          return;
      }
  
      const dateTime = this.dateTimeInput.nativeElement.value;
      const isoDateTime = new Date(dateTime).toISOString();
  
      this.taskService.RepeatInterview(this.task.taskResponseId, isoDateTime).subscribe(() => {
          window.location.reload();
      });
  }

  public OpenNextInterview()
  {
    this.showElement = true;
  }
  public CloseNextInterview()
  {
    this.showElement = false;
  }

  
}

