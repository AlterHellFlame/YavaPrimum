import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Tasks } from '../../../../../data/interface/Tasks.interface';
import { DateTime } from 'luxon';
import { TaskService } from '../../../../../services/task/task.service';
import { NotifyService } from '../../../../../services/notify/notify.service';

@Component({
  selector: 'app-task-interview',
  imports: [CommonModule],
  templateUrl: './task-interview.component.html',
  styleUrl: './task-interview.component.scss'
})
export class TaskInterviewComponent 
{
  @Input() task!: Tasks;

  showElement: boolean = false; 
  dateTime = DateTime.now().toFormat("yyyy-MM-dd'T'HH:mm");
  constructor(private taskService: TaskService, private notify: NotifyService){}
  
  ngOnInit(): void {

  }

  public NewStatus(status: string)
  {
    console.log(this.task.candidate.surname + ' ' + this.task.taskId)
    this.taskService.newStatus(this.task.taskId, status);
  
    this.notify.SendToUser("Хабиб-Абиб");

  }


  @ViewChild('dateTimeInput', { static: false }) dateTimeInput!: ElementRef;
  public NextInterview()
  {
      this.CloseNextInterview(); 
  
      let dateTime = this.dateTimeInput.nativeElement.value;
      //const isoDateTime = DateTime(dateTime).toFormat("yyyy-MM-dd'T'HH:mm");
      /*this.taskService.RepeatInterview(this.task.taskResponseId, dateTime).subscribe(() => {
          window.location.reload();
      });*/
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
