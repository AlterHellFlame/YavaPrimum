import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { TaskInfoComponent } from './task-info/task-info.component';

@Component({
  selector: 'app-task-card',
  imports: [CommonModule],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.scss'
})
export class TaskCardComponent {
  @Input() task!: Tasks;
  @Output() taskClicked = new EventEmitter<Tasks>();

  onButtonClick() 
  { 
    this.taskClicked.emit(this.task);
  }
}
