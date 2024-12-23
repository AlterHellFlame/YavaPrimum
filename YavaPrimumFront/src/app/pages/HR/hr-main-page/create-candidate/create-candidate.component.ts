import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NotifyHubService } from '../../../../services/notify-hub/notify-hub.service';
import { CandidateService } from '../../../../services/candidate/candidate.service';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-create-candidate',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './create-candidate.component.html',
  styleUrl: './create-candidate.component.scss',
})
export class CreateCandidateComponent implements OnInit {
  message: any;
  constructor(
    private notify: NotifyHubService,
    private candidateService: CandidateService
  ) {}

  ngOnInit(): void {
    this.notify.startConnection();
  }

  async sendMessage(): Promise<void> {
    await this.notify.sendMessage(this.message);
    this.message = '';
  }

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
    window.location.reload();
  }
}
