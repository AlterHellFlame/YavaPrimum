import { Component, ElementRef, ViewChild } from '@angular/core';
import { Tasks } from '../../../../../data/interface/Tasks.interface';
import { DateTime } from 'luxon';
import { TaskService } from '../../../../../services/task/task.service';
import { NotifyService } from '../../../../../services/notify/notify.service';

@Component({
  selector: 'app-task-interview',
  imports: [],
  templateUrl: './task-interview.component.html',
  styleUrl: './task-interview.component.scss'
})
export class TaskInterviewComponent 
{

  task!: Tasks;
  showElement: boolean = false; 
  dateTime = DateTime.now().toFormat("yyyy-MM-dd'T'HH:mm");
  constructor(private taskService: TaskService, private notify: NotifyService){}
  
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

  public Interview(status: string)
  {
    //this.taskService.Interview(this.task.taskResponseId, status);
    this.notify.SendToUser("Хабиб-Абиб");
    //window.location.reload();
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
