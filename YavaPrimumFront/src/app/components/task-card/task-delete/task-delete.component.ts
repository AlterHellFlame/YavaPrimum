import { Component, Input } from '@angular/core';
import { Tasks } from '../../../data/interface/Tasks.interface';

@Component({
  selector: 'app-task-delete',
  imports: [],
  templateUrl: './task-delete.component.html',
  styleUrl: './task-delete.component.scss'
})
export class TaskDeleteComponent {
  @Input() task!: Tasks;

  public Delete()
  {
    alert("Удалён")
  }
}
