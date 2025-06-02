import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Vacancy, VacancyRequest } from '../../data/interface/Vacancy.interface';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user/user.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { OptionalDataService } from '../../services/optional-data/optional-data.service';
import { User } from '../../data/interface/User.interface';
import { NotifyService } from '../../services/notify/notify.service';

@Component({
  selector: 'app-vacancy-page',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './vacancy-page.component.html',
  styleUrls: ['./vacancy-page.component.scss']
})
export class VacancyPageComponent implements OnInit {
  vacancies: Vacancy[] = [];
  posts: string[] = [];
  selectedVacancy!: Vacancy;
  user! : User;

    
  isAdmin: boolean = (localStorage.getItem('isAdmin') === 'true'? true : false);
  isHr: boolean = (localStorage.getItem('isHr') === 'true'? true : false);

  vacancyForm: FormGroup;
  editVacancyForm: FormGroup;

  @ViewChild('addVacancyModal') addVacancyModal!: TemplateRef<any>;
  @ViewChild('editVacancyModal') editVacancyModal!: TemplateRef<any>;
  public editModal!: NgbModalRef;

  constructor(
    private userService: UserService,
    public modalService: NgbModal,
    private optionalDataService: OptionalDataService,
    private fb: FormBuilder,
    private notify: NotifyService
  ) {
    this.vacancyForm = this.fb.group({
      post: ['', Validators.required],
      count: [1, [Validators.required, Validators.min(1)]],
      additionalData: ['']
    });

    this.editVacancyForm = this.fb.group({
      post: ['', Validators.required],
      count: [1, [Validators.required, Validators.min(1)]],
      additionalData: ['']
    });
  }

  ngOnInit(): void {
    this.loadVacancies();
  }

  private loadVacancies(): void {
    this.userService.getAllVacancies().subscribe(v => {
      this.vacancies = v;
    });
    
    this.optionalDataService.getPostsAndCountry().subscribe(p => {
      this.posts = p.posts;
    });

     this.userService.getUserData().subscribe(u => {
      this.user = u;
    });
  }

  openAddVacancyModal(): void {
    this.modalService.open(this.addVacancyModal);
  }

  openEditVacancyModal(vacancy: Vacancy): void {
    this.selectedVacancy = vacancy;
    this.editVacancyForm.patchValue({
      post: vacancy.post,
      count: vacancy.count,
      additionalData: vacancy.additionalData
    });
    
    this.editModal = this.modalService.open(this.editVacancyModal);
  }

  onSubmit(): void {
    if (this.vacancyForm.valid) {
      const newVacancy: VacancyRequest = this.vacancyForm.value;
      this.userService.createVacancy(newVacancy).subscribe({
        next: (createdVacancy) => {
          this.modalService.dismissAll();
          this.vacancies.unshift(createdVacancy);
          this.vacancyForm.reset({ count: 1 });
          this.notify.showSuccess("Вакансия добавлена");
          this.notify.SendToUser("Добавление вакансии");
        },
        error: (err) => 
          {
            console.error('Ошибка при создании:', err);
            this.notify.showError("Что-то пошло не так...");
          }
        
      });
    }
  }

  onEditSubmit(): void {
    if (this.editVacancyForm.valid && this.selectedVacancy) {
      const updatedVacancy: VacancyRequest = this.editVacancyForm.value;
      this.userService.updateVacancy(this.selectedVacancy.vacancyId, updatedVacancy).subscribe({
        next: () => {
          const index = this.vacancies.findIndex(v => v.vacancyId === this.selectedVacancy.vacancyId);
          if (index > -1) {
            this.vacancies[index] = { 
              ...this.vacancies[index], 
              ...updatedVacancy 
            };
          }
          this.editModal.dismiss();
        },
        error: (err) => console.error('Ошибка при обновлении:', err)
      });
    }
  }

  closeVacancy(vacancyId: string): void {
    this.userService.closeVacancy(vacancyId).subscribe({
      next: () => {
        this.vacancies = this.vacancies.map(v => 
          v.vacancyId === vacancyId ? { ...v, isClose: true } : v
        );
      },
      error: (err) => console.error('Ошибка при закрытии:', err)
    });
  }

get filteredVacancies(): Vacancy[] {
  return this.vacancies.filter(vacancy => {
    const matchesPost = vacancy.post.toLowerCase().includes(this.filter.post.toLowerCase());
    const matchesStatus = this.filterStatus === 'all' || 
      (this.filterStatus === 'completed' && vacancy.isClose) || 
      (this.filterStatus === 'incomplete' && !vacancy.isClose);
    const matchesMyTasks = !this.showOnlyMyTasks || this.canEditVacancy(vacancy);
    
    return matchesPost && matchesStatus && matchesMyTasks;
  });
}

// Метод проверки прав
canEditVacancy(vacancy: Vacancy): boolean {
  return vacancy.user?.userId === this.user?.userId;
}

filter = { post: '' };
filterStatus = 'all'; // Начальное значение фильтра статуса
showOnlyMyTasks: boolean = false;

  resetFilters(): void {
    this.filter.post = '';
    this.filterStatus = 'all';
    this.showOnlyMyTasks = false;
  }

  getStatusBadgeClass(isClose: boolean): any {
    return {
      'bg-opacity-10': true,
      'text-success': isClose,
      'text-primary': !isClose
    };
  }

  getStatusIconClass(isClose: boolean): any {
    return {
      'bi-check-circle-fill': isClose,
      'bi-hourglass-top': !isClose
    };
  }
}