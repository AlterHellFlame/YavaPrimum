import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../services/task/task.service';
import { DateTime } from 'luxon';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Candidate } from '../../data/interface/Candidate.interface';

@Component({
  selector: 'app-candidate-page',
  imports: [FormsModule, CommonModule],
  templateUrl: './candidate-page.component.html',
  styleUrl: './candidate-page.component.scss'
})
export class CandidatePageComponent implements OnInit 
{
  constructor(private taskService: TaskService){};

  candidates: Candidate[] = [];

  ngOnInit(): void 
  {
      this.taskService.getAllCandidatesOfUser().subscribe(c =>
      {
        this.candidates = c;
      }
      );
  }

  filter = {
    surname: '',
    firstName: ''
  };

  // Метод для фильтрации списка
  get filteredCandidates(): Candidate[] {
    return this.candidates.filter(candidate =>
      candidate.surname.toLowerCase().includes(this.filter.surname.toLowerCase()) &&
      candidate.firstName.toLowerCase().includes(this.filter.firstName.toLowerCase())
    );
  }

}
