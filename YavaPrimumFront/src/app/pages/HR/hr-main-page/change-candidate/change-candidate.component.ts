import { CommonModule, JsonPipe } from '@angular/common';
import { Component, HostListener, Input, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AnotherService } from '../../../../services/another/another.service';
import { Tasks } from '../../../../data/interface/Tasks.interface';
import { CandidateService } from '../../../../services/candidate/candidate.service';
import { TaskService } from '../../../../services/task/task.service';

@Component({
  selector: 'app-change-candidate',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './change-candidate.component.html',
  styleUrl: './change-candidate.component.scss',
  providers: [AnotherService]
})
export class CreateCandidateComponent implements OnInit {
  task!: Tasks;

  form!: FormGroup;
  posts!: string[];
  countres!: string[];
  phoneMask: string = '+375 (00) 000-00-00'

  constructor(
    private anotherService : AnotherService,
    private fb: FormBuilder,
    private candidateService: CandidateService,
    private taskService: TaskService
  ) 
  {}


  ngOnInit(): void {
    this.anotherService.getPostsAndCountry().subscribe(res => {
      this.posts = res.posts;
      this.countres = res.countres;
    })

    this.taskService.taskClick$.subscribe(task =>
    {
      this.task = task!;
      if(task != null)
      {
        this.form = this.fb.group({
          secondName: [this.task?.candidate.secondName || '', Validators.required],
          firstName: [this.task?.candidate.firstName || '', Validators.required],
          surName: [this.task?.candidate.surName || '', Validators.required],
          post: [this.task?.candidate.post || '', Validators.required],
          country: [this.task?.candidate.country || '', Validators.required],
          telephone: [this.task?.candidate.telephone || '', Validators.required],
          email: [this.task?.candidate.email || '', [Validators.required, Validators.email]],
          interviewDate: [this.task?.dateTime ? this.task.dateTime.toISO() : '', Validators.required]
        });
        console.log(JSON.stringify(this.form.value, null, 2));
      }
      else
      {
        console.error("Таска нет");
      }
    })
    
  }


  onSubmit(): void {    
    const formValue = {
    FirstName: this.form.get('firstName')!.value!,
    SecondName: this.form.get('secondName')!.value!,
    SurName: this.form.get('surName')!.value!,
    Post: this.form.get('post')!.value!,
    Country: this.form.get('country')!.value!,
    Telephone: this.form.get('telephone')!.value!,
    Email: this.form.get('email')!.value!,
    InterviewDate: this.form.get('interviewDate')!.value!,
    }
    console.log('Форма отправлена:', formValue);

    this.candidateService.changeCandidateAndInterview(formValue, this.task.taskResponseId)
    .subscribe((t) => {
      console.log('Кандидат на изменен успешно');
      //this.notify.sendMessage("Habib");
      window.location.reload();
    });;
  }

}


