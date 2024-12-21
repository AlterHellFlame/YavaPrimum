import { CommonModule } from '@angular/common';
import { Component, OnInit, Signal } from '@angular/core';
import { CreateCandidateComponent } from "./create-candidate/create-candidate.component";
import { HttpClientModule } from '@angular/common/http';
import { CandidateService } from '../../../services/candidate/candidate.service';
import { Candidate } from '../../../data/interface/Candidate.interface';
import { CalendarComponent } from './calendar/calendar.component';
import { TaskCardComponent } from '../../../components/task-card/task-card.component';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-hr-main-page',
  standalone: true,
  imports: [CommonModule, CreateCandidateComponent, HttpClientModule,
             CalendarComponent, TaskCardComponent],
  templateUrl: './hr-main-page.component.html',
  styleUrls: ['./hr-main-page.component.scss'],
  providers: [CandidateService, TaskService]
})
export class HrMainPageComponent implements OnInit {
  candidates: Candidate[] = [];
  activeDay: DateTime = DateTime.now();
  tasks: Tasks[] = [];
  tasksAll: Tasks[] = [];

  constructor(private candidateService: CandidateService, private taskService: TaskService) {}

  //Выполнение при знапуске странички
  ngOnInit(): void {
    this.taskService.getAllTasks().subscribe(tasks =>
    {
      this.tasksAll = tasks;
      console.log("Все таски ", this.tasksAll);
    }
    );
    this.taskService.activeDay$.subscribe(day => 
    {
      this.activeDay = day;
      console.log("Выбранная дата: ", this.activeDay);
      this.getTasksOfDay();
    });
  }

  //Открытие модального окна
  isDropdownOpen = false;
  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  //Фильтры для тасков
  applyFilter(filter: string) {
    console.log(`Применён фильтр: ${filter}`);
  }


  public getTasksOfDay(): void 
  {
      if (this.activeDay == null) 
      {
        this.tasks = [];
      } 
      else 
      {
        this.tasks = this.tasks.filter(task =>
          task.dateTime.hasSame(this.activeDay, 'day')
        );
      }
  }
}
