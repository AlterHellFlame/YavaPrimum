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
export class TaskInfoComponent implements OnInit {
  task!: Tasks;
  showElement: boolean = false; 
  dateTime = DateTime.now().toFormat("yyyy-MM-dd'T'HH:mm");
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

  @ViewChild('dateTimeInput', { static: false }) dateTimeInput!: ElementRef;
  public NextInterview()
  {
      this.CloseNextInterview(); 
  
      let dateTime = this.dateTimeInput.nativeElement.value;
      //const isoDateTime = DateTime(dateTime).toFormat("yyyy-MM-dd'T'HH:mm");
      this.taskService.RepeatInterview(this.task.taskResponseId, dateTime).subscribe(() => {
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

