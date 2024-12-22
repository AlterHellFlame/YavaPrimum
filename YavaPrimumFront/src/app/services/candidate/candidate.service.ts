import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { DateTime } from 'luxon';
import { TasksRequest } from '../../data/interface/Tasks.interface';
import { Candidate } from '../../data/interface/Candidate.interface';

@Injectable({
  providedIn: 'root',
})
export class CandidateService {
  private baseApiUrl = 'https://localhost:7247/';

  constructor(private http: HttpClient) {}

  task!: TasksRequest;
  public addCandidate(payload: {
    FirstName: string;
    SecondName: string;
    SurName: string;
    Post: string;
    Country: string;
    Telephone: string;
    Email: string;
    InterviewDate: string;
  }) {

    const candidate: Candidate = {
      firstName: payload.FirstName,
      secondName: payload.SecondName,
      surName: payload.SurName,
      post: payload.Post,
      country: payload.Country,
      telephone: payload.Telephone,
      email: payload.Email,
    };

    const task = {
      candidate: candidate,
      interviewDate: payload.InterviewDate,
    };

    return this.http
      .post<TasksRequest>(`${this.baseApiUrl}create-task`, task, {
        withCredentials: true,
      })
      .subscribe((t) => {
        console.log('Кандидат на приём успешно отправлен');
      });
  }
}
