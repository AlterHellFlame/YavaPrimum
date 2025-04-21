import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../services/task/task.service';
import { DateTime } from 'luxon';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CandidatesFullData } from '../../data/interface/Candidate.interface';

@Component({
  selector: 'app-candidate-page',
  imports: [FormsModule, CommonModule],
  templateUrl: './candidate-page.component.html',
  styleUrls: ['./candidate-page.component.scss']
})
export class CandidatePageComponent implements OnInit {
  candidates: CandidatesFullData[] = [];
  filterStatus: string = 'all'; // Значение фильтра по умолчанию
  filter = {
    surname: '',
    firstName: ''
  };

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadCandidates();
  }

  // Загрузка данных кандидатов
  private loadCandidates(): void {
    this.taskService.getAllCandidatesOfUser().subscribe(c => {
      this.candidates = c;
    });
  }

  // Простой текстовый фильтр по имени и фамилии
  get filteredCandidates(): CandidatesFullData[] 
  {
    return this.candidates.filter(candidate =>
      candidate.candidate.surname.toLowerCase().includes(this.filter.surname.toLowerCase()) &&
      candidate.candidate.firstName.toLowerCase().includes(this.filter.firstName.toLowerCase())
    ).filter(candidate => 
      {
        console.log(this.filterStatus);
      if (this.filterStatus === 'completed') {
        
        return this.allTasksCompleted(candidate);
      }
      if (this.filterStatus === 'incomplete') {
        return !this.allTasksCompleted(candidate);
      }
      return true; // Возвращаем всех, если выбран "Все"
    });;
  }

  // Форматирование задач (дата + статус)
  getTaskEntries(tasks: Tasks[]): { key: string; value: string }[] {
    return tasks.map(task => ({
      key: DateTime.fromISO(task.dateTime.toString()).toFormat('dd.MM.yyyy HH:mm'),
      value: task.status
    }));
  }

  // Форматирование даты (универсальный метод)
  toFormat(dateTime: any): string {
    return DateTime.fromISO(dateTime.toString()).toFormat('dd.MM.yyyy HH:mm');
  }

  // Проверка статуса задачи (typeStatus 2 или 4 указывает на завершение)
  isTaskCompleted(task: Tasks): boolean {
    return task.typeStatus === 2 || task.typeStatus === 4;
  }

  // Проверка: все ли задачи кандидата выполнены
  allTasksCompleted(candidate: CandidatesFullData): boolean {
    return candidate.tasks.every(this.isTaskCompleted);
  }
}
