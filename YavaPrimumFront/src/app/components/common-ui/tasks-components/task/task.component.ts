import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Tasks } from '../../../../data/interface/Tasks.interface';

@Component({
  selector: 'app-task',
  imports: [CommonModule],
  templateUrl: './task.component.html',
  styleUrl: './task.component.scss'
})
export class TaskComponent {
  @Output() setActiveTask = new EventEmitter<any>();
  @Input() task!: Tasks;

  public onButtonClick(): void
  {
    this.setActiveTask.emit(this.task);
  }
}
