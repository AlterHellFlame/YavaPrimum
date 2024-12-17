import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { NotifyHubService } from '../../../../services/notify-hub/notify-hub.service';
import { CandidateService } from '../../../../services/candidate/candidate.service';

@Component({
  selector: 'app-create-candidate',
  imports: [CommonModule, FormsModule],
  templateUrl: './create-candidate.component.html',
  styleUrl: './create-candidate.component.scss'
})
export class CreateCandidateComponent implements OnInit{
  message: any;
  constructor(private notify: NotifyHubService, private candidateService: CandidateService) { }

  ngOnInit(): void 
  {
    this.notify.startConnection();
  }

  async sendMessage(): Promise<void> {
    await this.notify.sendMessage(this.message);
    this.message = '';
  }


  onSubmit(form: NgForm): void {
    const payload = {
      FirstName: form.value.firstName,
      SecondName: form.value.secondName,
      SurName: form.value.surName,
      Post: form.value.post,
      Country: form.value.country
    };

    console.log('Форма отправлена:', payload);
    this.candidateService.addCandidate(payload).subscribe(
      response => {
        console.log('Ответ от сервера:', response);
      },
      error => {
        console.error('Ошибка при отправке данных:', error);
      }
    );
  }
  

}