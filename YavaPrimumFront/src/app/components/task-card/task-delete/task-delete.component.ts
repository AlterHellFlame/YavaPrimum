import { Component, Input } from '@angular/core';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';

@Component({
  selector: 'app-task-delete',
  imports: [],
  templateUrl: './task-delete.component.html',
  styleUrl: './task-delete.component.scss'
})
export class TaskDeleteComponent {
  @Input() task!: Tasks;

  constructor(private taskService: TaskService){}
  public Delete()
  {
    this.taskService.DeleteTask(this.task.taskResponseId);
    window.location.reload();
  }
}
