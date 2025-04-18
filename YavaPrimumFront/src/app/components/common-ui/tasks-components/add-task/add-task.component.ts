import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DateTime } from 'luxon';
import { TaskService } from '../../../../services/task/task.service';
import { OptionalDataService } from '../../../../services/optional-data/optional-data.service';
import { NotifyService } from '../../../../services/notify/notify.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-task',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.scss']
})
export class AddTaskComponent implements OnInit {
  form!: FormGroup;
  message: any;
  posts: string[] = [];
  countries: string[] = [];

  constructor(
    private optionalDataService: OptionalDataService,
    private notify: NotifyService,
    private taskService: TaskService
  ) {}

  ngOnInit(): void {
    console.log("ngOnInit AddTaskComponent");
    this.initializeForm();
    this.loadData();
    this.notify.startConnection();
  }

  initializeForm(): void {
    this.form = new FormGroup({
      surname: new FormControl('', Validators.required),
      firstName: new FormControl('', Validators.required),
      patronymic: new FormControl(''),
      post: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      phone: new FormControl('', Validators.required), //Validators.pattern('^\\+375[0-9]{9}$')),
      email: new FormControl('', Validators.email),
      interviewDate: new FormControl('', Validators.required),
    });    
  }

  loadData(): void {
    this.optionalDataService.getPostsAndCountry().subscribe((res) => {
      this.posts = res.posts || [];
      this.countries = res.countries || [];

      if (this.posts.length > 0 && this.countries.length > 0) {
        this.form.patchValue({
          post: this.posts[0],
          country: this.countries[0],
        });
      }
    });
  }

  async sendMessage(): Promise<void> {
    await this.notify.SendToUser(this.message);
    this.message = '';
  }

  onSubmit(): void {
    console.log(JSON.stringify(this.form.value, null, 2));

    if (this.form.valid) {
      const formValue = this.form.value;

      console.log(formValue);
      this.taskService.addNewTask(formValue).subscribe(() => {
        console.log('Кандидат на приём успешно отправлен');
        this.notify.SendToUser("Habib");
        window.location.reload();
        this.taskService.getAllTasksOfUser();
      });

      /*this.candidateService.addCandidateAndInterview(formValue).subscribe(() => {
        console.log('Кандидат на приём успешно отправлен');
        this.notify.SendToUser("Habib");
        window.location.reload();
        this.taskService.getAllTasksOfUser();
      });*/
    } else {
      console.log('Форма не валидна');
    }
  }
}
