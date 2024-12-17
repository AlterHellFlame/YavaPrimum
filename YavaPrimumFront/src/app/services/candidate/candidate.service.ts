import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Candidate } from '../../data/interface/Candidate.interface';

@Injectable({
  providedIn: 'root',
})
export class CandidateService {
  private baseApiUrl = 'https://localhost:7247/api/Candidate';

  constructor(private http: HttpClient) {}

  public addCandidate(payload: {
    FirstName: string;
    SecondName: string; 
    SurName: string; 
    Post: string; 
    Country: string;
  }): Observable<Candidate> {
    console.log('Отправка кандидата:', payload);
    return this.http.post<Candidate>(`${this.baseApiUrl}`, payload);
  }

  public getProductCards(): Observable<Candidate[]> {
    return this.http.get<Candidate[]>(`${this.baseApiUrl}`);
  }
}
