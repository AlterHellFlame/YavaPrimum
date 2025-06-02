import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../../../services/task/task.service';
import { OptionalDataService } from '../../../../services/optional-data/optional-data.service';
import { NotifyService } from '../../../../services/notify/notify.service';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { Tasks } from '../../../../data/interface/Tasks.interface';
import { DateTime } from 'luxon';

interface CandidateForm {
  surname: string;
  firstName: string;
  patronymic: string;
  post: string;
  country: string;
  phone: string;
  email: string;
  interviewDate: string;
  additionalData: string;
}

@Component({
  selector: 'app-add-task',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.scss']
})
export class AddTaskComponent implements OnInit, OnDestroy {
  form: FormGroup;
  posts: string[] = [];
  countries: string[] = [];
  minDateTime: string = new Date().toISOString().slice(0, 16);
  @Input() tasksForDay: Tasks[] = [];
  
  private dataSubscription?: Subscription;
  
  constructor(
    private optionalDataService: OptionalDataService,
    private notify: NotifyService,
    private taskService: TaskService
  ) {
    this.form = this.initForm();
  }

  ngOnInit(): void {
    this.loadData();
    this.notify.startConnection();
  }

  ngOnDestroy(): void {
    this.dataSubscription?.unsubscribe();
  }

  private initForm(): FormGroup {
    return new FormGroup({
      surname: new FormControl('', [
        Validators.required,
        Validators.maxLength(25)
      ]),
      firstName: new FormControl('', [
        Validators.required,
        Validators.maxLength(25)
      ]),
      patronymic: new FormControl('', [
        Validators.maxLength(25)
      ]),
      post: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      phone: new FormControl('', [
        Validators.required,
        Validators.maxLength(20)
      ]),
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        Validators.maxLength(254)
      ]),
      interviewDate: new FormControl('', [
        Validators.required,
        this.validateInterviewTime.bind(this)
      ]),
      additionalData: new FormControl('', [
        Validators.minLength(3),
        Validators.maxLength(500)
      ])
    });
  }

  loadData(): void {
    this.dataSubscription = this.optionalDataService.getPostsAndCountry().subscribe({
      next: (res) => {
        this.posts = res.posts || [];
        this.countries = res.countries || [];
        this.setDefaultValues();
      },
      error: (err) => console.error('Ошибка загрузки данных:', err)
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      
      if (this.form.get('interviewDate')?.errors?.['timeConflict']) {
        this.notify.showError('Это время уже занято (разница менее 20 минут)');
      }
      
      if (this.form.get('surname')?.errors?.['maxLength']) {
        this.notify.showError('Фамилия не должна превышать 25 символов');
      }
      
      if (this.form.get('firstName')?.errors?.['maxLength']) {
        this.notify.showError('Имя не должно превышать 25 символов');
      }
      
      if (this.form.get('patronymic')?.errors?.['maxLength']) {
        this.notify.showError('Отчество не должно превышать 25 символов');
      }
      
      if (this.form.get('phone')?.errors?.['maxLength']) {
        this.notify.showError('Телефон не должен превышать 13 символов');
      }
      
      if (this.form.get('email')?.errors?.['maxLength']) {
        this.notify.showError('Email не должен превышать 254 символов');
      }
      
      if (this.form.value.additionalData) {
        if (this.form.get('additionalData')?.errors?.['minlength']) {
          this.notify.showError('Доп. информация должна содержать минимум 3 символа');
        }
        
        if (this.form.get('additionalData')?.errors?.['maxlength']) {
          this.notify.showError('Доп. информация не должна превышать 500 символов');
        }
      }
      else
      {
        this.form.value.additionalData = "";
      }
      
      return;
    }

    this.taskService.addNewTask(this.form.value as CandidateForm).subscribe({
      next: () => {
        this.taskService.getAllTasksOfUser();
        this.notify.SendToUser('Новая задача');
        this.notify.showSuccess('Добавлено собеседование');
        this.form.reset();
        this.setDefaultValues();
      },
      error: (err) => {
        if(err.status == '106') {
          this.notify.showError('Такой почты не существует');
        }
        console.error('Ошибка добавления кандидата:', err);
        this.notify.showError('Что-то сломалось');
      }
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  private setDefaultValues(): void {
    if (this.posts.length && this.countries.length) {
      this.form.patchValue({
        post: this.posts[0],
        country: this.countries[0]
      });
    }
  }

  private validateInterviewTime(control: FormControl): { [key: string]: boolean } | null {
    if (!control.value) {
      return null;
    }
    const selectedTime = DateTime.fromISO(control.value).toMillis();
    const fiveMinutesInMs = 20 * 60 * 1000;

    console.log(this.taskService.getTasksOfDay(DateTime.fromISO(control.value)));
    const hasConflict = this.taskService.getTasksOfDay(DateTime.fromISO(control.value)).some(task => {
      if (!DateTime.fromISO(control.value)) return false;
      
      const taskTime = task.dateTime instanceof DateTime 
        ? task.dateTime.toMillis() 
        : DateTime.fromISO(task.dateTime).toMillis();
      
      const timeDifference = Math.abs(selectedTime - taskTime);
      
      return timeDifference <= fiveMinutesInMs;
    });
    //alert( hasConflict ? { timeConflict: true } : null)
    return hasConflict ? { timeConflict: true } : null;
  }
}