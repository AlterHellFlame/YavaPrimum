import { Component, Input, OnInit } from '@angular/core';
import { Tasks } from '../../../../data/interface/Tasks.interface';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { OptionalDataService } from '../../../../services/optional-data/optional-data.service';
import { NotifyService } from '../../../../services/notify/notify.service';
import { TaskService } from '../../../../services/task/task.service';
import { DateTime } from 'luxon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-change-task',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './change-task.component.html',
  styleUrl: './change-task.component.scss'
})
export class ChangeTaskComponent implements OnInit {
  @Input() task!: Tasks;
  form!: FormGroup;
  posts: string[] = [];
  countries: string[] = [];
  phoneMask = '+375 (00) 000-00-00';
  private fb!: FormBuilder;

  constructor(
    private optionalDataService: OptionalDataService,
    private notify: NotifyService,
    private taskService: TaskService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadPostsAndCountries();
    this.subscribeToTaskSelection();
  }

  private initForm(): void {
    this.form = this.fb.group({
      secondName: ['', Validators.required],
      firstName: ['', Validators.required],
      surName: ['', Validators.required],
      post: ['', Validators.required],
      country: ['', Validators.required],
      telephone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      interviewDate: [DateTime.now().toFormat("yyyy-MM-dd'T'HH:mm"), Validators.required]
    });
  }

  private loadPostsAndCountries(): void {
    this.optionalDataService.getPostsAndCountry().subscribe(res => {
      this.posts = res.posts;
      this.countries = res.countries;
    });
  }

  private subscribeToTaskSelection(): void {
    
      if (this.task) {
        this.form.patchValue({
          secondName: this.task.candidate.surname,
          firstName: this.task.candidate.firstName,
          surName: this.task.candidate.patronymic,
          post: this.task.candidate.post,
          country: this.task.candidate.country,
          telephone: this.task.candidate.phone,
          email: this.task.candidate.email,
          interviewDate: this.task.dateTime.toFormat("yyyy-MM-dd'T'HH:mm")
        });
        console.log('Form initialized with task:', this.form.value);
      } else {
        console.error('task not found');
      }
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;
      console.log('Form submitted:', formValue);

      /*this.candidateService.changeCandidateAndInterview(formValue, this.taskResponseId).subscribe(() => {
        console.log('Candidate updated successfully');
      });*/
    } else {
      console.error('Form is invalid');
    }
  }
}
