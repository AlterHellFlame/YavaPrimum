import { Component, Input } from '@angular/core';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { TaskInfoComponent } from './task-info/task-info.component';

@Component({
  selector: 'app-task-card',
  imports: [CommonModule, TaskInfoComponent],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.scss'
})
export class TaskCardComponent {
  @Input() task!: Tasks;
}
