import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../services/task/task.service';
import { DateTime } from 'luxon';
import { Tasks } from '../../data/interface/Tasks.interface';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CandidatesFullData } from '../../data/interface/Candidate.interface';

@Component({
  selector: 'app-candidate-page',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './candidate-page.component.html',
  styleUrls: ['./candidate-page.component.scss']
})
export class CandidatePageComponent implements OnInit {
  candidates: CandidatesFullData[] = [];
  filterStatus: string = 'all';
  filter = {
    surname: '',
    firstName: ''
  };

  // Переменные для модального окна изменения даты
  selectedTasks: Tasks[] | null = null;
  isChangeDate: boolean = false;
  additionalData: string = '';

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadCandidates();
  }

  private loadCandidates(): void {
    this.taskService.getAllCandidatesOfUser().subscribe(c => {
      this.candidates = c;
    });
  }

  get filteredCandidates(): CandidatesFullData[] {
    return this.candidates.filter(candidate =>
      candidate.candidate.surname.toLowerCase().includes(this.filter.surname.toLowerCase()) &&
      candidate.candidate.firstName.toLowerCase().includes(this.filter.firstName.toLowerCase())
    ).filter(candidate => {
      if (this.filterStatus === 'completed') {
        return this.allTasksCompleted(candidate);
      }
      if (this.filterStatus === 'incomplete') {
        return !this.allTasksCompleted(candidate);
      }
      return true;
    });
  }

  getTaskEntries(tasks: Tasks[]): { key: string; value: string }[] {
    return tasks.map(task => ({
      key: DateTime.fromISO(task.dateTime.toString()).toFormat('dd.MM.yyyy HH:mm'),
      value: task.status
    }));
  }

  toFormat(dateTime: any): string {
    return DateTime.fromISO(dateTime.toString()).toFormat('dd.MM.yyyy HH:mm');
  }

  isTaskCompleted(task: Tasks): boolean {
    let isComp = (task.typeStatus === 2 || task.typeStatus === -2);
    //if(task.user.post == 'HR' && task.status.startsWith("Запрос"))  isComp = true 
    return isComp;
  }

  allTasksCompleted(candidate: CandidatesFullData): boolean {
    return candidate.tasks.every(this.isTaskCompleted);
  }

  // Выбор задачи для изменения даты
  selectTaskForChange(tasks: Tasks[]): void {
    this.selectedTasks = tasks;
  }

  // Подтверждение изменения даты
  confirmDateTimeChange(): void {
    if (!this.selectedTasks) return;

    const updateData = {
      taskId: this.selectedTasks[0].taskId,
      isChangeDate: this.isChangeDate,
      additionalData: this.additionalData
    };

    this.taskService.updateTaskDateTime(updateData).subscribe({
      next: () => {
        this.loadCandidates(); // Перезагружаем данные
        this.closeModal();
      },
      error: (err) => {
        console.error('Ошибка при изменении даты:', err);
      }
    });
  }

  // Закрытие модального окна
  closeModal(): void {
    this.selectedTasks = null;
    this.additionalData = '';
    // Здесь можно добавить закрытие модального окна через JavaScript
  }

    isHR: boolean = (localStorage.getItem('isHR') === 'true'? true : false);
  // Проверка, можно ли изменить дату для задачи
  canChangeDateTime(tasks: Tasks[] = this.selectedTasks || []): string | null {
    if (!tasks.length) return null; // ✅ Если список пустой, возвращаем null

    if(!this.isHR) return null;

    if (tasks.some(task => task.status === 'Дата подтверждена')) {
        return 'date'; // ✅ Если хотя бы одна задача имеет статус "Дата подтверждена"
    }
    if (tasks.some(task => task.status === 'Время подтверждено')) {
        return 'datetime'; // ✅ Если хотя бы одна задача имеет статус "Время подтверждено"
    }
    return null; // ✅ Если статусы не найдены, возвращаем null
}

expandedCandidates: { [key: string]: boolean } = {};

  // Переключение отображения деталей кандидата
  toggleCandidateDetails(candidate: CandidatesFullData): void {
    this.expandedCandidates[candidate.candidate.candidateId!] = !this.expandedCandidates[candidate.candidate.candidateId!];
  }
}