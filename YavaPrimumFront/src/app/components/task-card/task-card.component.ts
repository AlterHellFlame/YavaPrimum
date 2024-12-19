import { Component, Input } from '@angular/core';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-task-card',
  imports: [CommonModule],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.scss'
})
export class TaskCardComponent {
  @Input() task!: Tasks;
}
