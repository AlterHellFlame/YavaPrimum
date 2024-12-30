import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { TaskInfoComponent } from './task-info/task-info.component';
import { TaskDeleteComponent } from './task-delete/task-delete.component';
import { TaskService } from '../../services/task/task.service';

@Component({
  selector: 'app-task-card',
  imports: [CommonModule],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.scss'
})
export class TaskCardComponent {
  @Input() task!: Tasks;

  constructor(private taskSevice: TaskService){}
  onButtonClick() 
  { 
    this.taskSevice.setClickedTask(this.task);
  }
}
