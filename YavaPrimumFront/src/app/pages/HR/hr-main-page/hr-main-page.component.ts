import { CommonModule } from '@angular/common';
import { Component, OnInit, Signal } from '@angular/core';
import { HttpClientModule, provideHttpClient } from '@angular/common/http';
import { CandidateService } from '../../../services/candidate/candidate.service';
import { Candidate } from '../../../data/interface/Candidate.interface';
import { CalendarComponent } from './calendar/calendar.component';
import { TaskCardComponent } from '../../../components/task-card/task-card.component';
import { Tasks } from '../../../data/interface/Tasks.interface';
import { TaskService } from '../../../services/task/task.service';
import { TaskInfoComponent } from '../../../components/task-card/task-info/task-info.component';
import { CreateCandidateComponent } from './change-candidate/change-candidate.component';
import { ChangeCandidateComponent } from './create-candidate/create-candidate.component';
import { FormBuilder, Validators } from '@angular/forms';
import { TaskDeleteComponent } from '../../../components/task-card/task-delete/task-delete.component';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-hr-main-page',
  standalone: true,
  imports: [CommonModule, CreateCandidateComponent, HttpClientModule,
    CalendarComponent, TaskCardComponent, TaskInfoComponent, ChangeCandidateComponent, TaskDeleteComponent],
  templateUrl: './hr-main-page.component.html',
  styleUrls: ['./hr-main-page.component.scss'],
  providers: [CandidateService, TaskService]
})
export class HrMainPageComponent implements OnInit {
  candidates: Candidate[] = [];
  tasks: Tasks[] = [];

  constructor(private taskService: TaskService, private fb: FormBuilder) {}

  //Выполнение при знапуске странички
  ngOnInit(): void {
    this.taskService.getAllTasks();
    this.taskService.dayTasks$.subscribe( tasks =>
    {
      this.tasks = tasks;
      console.log("Полученные таски: " + this.tasks.length);
    })
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



}
