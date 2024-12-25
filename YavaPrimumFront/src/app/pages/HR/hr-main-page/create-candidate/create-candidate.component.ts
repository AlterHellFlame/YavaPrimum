import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NotifyHubService } from '../../../../services/notify-hub/notify-hub.service';
import { CandidateService } from '../../../../services/candidate/candidate.service';
import { AnotherService } from '../../../../services/another/another.service';
import { HttpClientModule } from '@angular/common/http';
import { TaskService } from '../../../../services/task/task.service';

@Component({
  selector: 'app-create-candidate',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './create-candidate.component.html',
  styleUrl: './create-candidate.component.scss',
  providers: [AnotherService]
})
export class ChangeCandidateComponent implements OnInit {

  form = new FormGroup({
    FirstName: new FormControl('', Validators.required),
    SecondName: new FormControl('', Validators.required),
    SurName: new FormControl('', Validators.required),
    Post: new FormControl('', Validators.required),
    Country: new FormControl('', Validators.required),
    Telephone: new FormControl('', Validators.required),
    Email: new FormControl('', Validators.required),
    InterviewDate: new FormControl('', Validators.required)
  });

  message: any;
  posts!: string[];
  countres!: string[];
  constructor(
    private anotherService : AnotherService,
    private notify: NotifyHubService,
    private candidateService: CandidateService,
    private taskSevice: TaskService
  ) {}

  ngOnInit(): void {
    this.notify.startConnection();
    this.anotherService.getPostsAndCountry().subscribe(res => {
      this.posts = res.posts;
      this.countres = res.countres;
    })
  }
  

  async sendMessage(): Promise<void> {
    await this.notify.sendMessage(this.message);
    this.message = '';
  }


  onSubmit(): void {    
      const formValue = {
      FirstName: this.form.get('FirstName')!.value!,
      SecondName: this.form.get('SecondName')!.value!,
      SurName: this.form.get('SurName')!.value!,
      Post: this.form.get('Post')!.value!,
      Country: this.form.get('Country')!.value!,
      Telephone: this.form.get('Telephone')!.value!,
      Email: this.form.get('Email')!.value!,
      InterviewDate: this.form.get('InterviewDate')!.value!,
    }
    console.log('Форма отправлена:', formValue);

    this.candidateService.addCandidateAndInterview(formValue);
    this.notify.sendMessage("Habib");
    //window.location.reload();
    this.taskSevice.getAllTasks();
  }
}
