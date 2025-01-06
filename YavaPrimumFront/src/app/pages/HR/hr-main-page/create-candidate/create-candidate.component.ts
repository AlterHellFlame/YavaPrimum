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
import { DateTime } from 'luxon';

@Component({
  selector: 'app-create-candidate',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './create-candidate.component.html',
  styleUrl: './create-candidate.component.scss',
  providers: [AnotherService]
})
export class ChangeCandidateComponent implements OnInit {

  form!: FormGroup;

  message: any;
  posts: string[] = [];
  countries: string[] = [];
  constructor(
    private anotherService : AnotherService,
    private notify: NotifyHubService,
    private candidateService: CandidateService,
    private taskSevice: TaskService
  ) {}

  ngOnInit(): void 
  {
    this.anotherService.getPostsAndCountry().subscribe(res => {
    this.posts = res.posts;
    this.countries = res.countres;
    
    this.notify.startConnection();

    this.form = new FormGroup({
      secondName: new FormControl('', Validators.required),
      firstName: new FormControl('', Validators.required),
      surName: new FormControl('', Validators.required),
      post: new FormControl(this.posts[0], Validators.required),
      country: new FormControl(this.countries[0], Validators.required),
      telephone: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      interviewDate: new FormControl(DateTime.now().toFormat("yyyy-MM-dd'T'HH:mm"), Validators.required)
    });
    })
  }

  async sendMessage(): Promise<void> {
    await this.notify.sendMessage(this.message);
    this.message = '';
  }

  onSubmit(): void {    
    console.log(JSON.stringify(this.form.value, null, 2));
      
    if (this.form.valid) 
      {
        
      const formValue = {
        SecondName: this.form.get('secondName')!.value,
        FirstName: this.form.get('firstName')!.value,
        SurName: this.form.get('surName')!.value,
        Post: this.form.get('post')!.value,
        Country: this.form.get('country')!.value,
        Telephone: this.form.get('telephone')!.value,
        Email: this.form.get('email')!.value,
        InterviewDate: this.form.get('interviewDate')!.value,
      }
      
      this.candidateService.addCandidateAndInterview(formValue).subscribe((t) => {
        console.log('Кандидат на приём успешно отправлен');
        this.notify.sendMessage("Habib");
        window.location.reload();
        this.taskSevice.getAllTasks();
      });
    } else {
      console.log('Форма не валидна');
    }
  }
}
