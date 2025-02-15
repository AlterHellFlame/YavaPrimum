import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../../data/interface/User.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}


  public logIn(payload : { EMail: string; Password : string; }): Observable<string> 
  {
    console.log(payload);

    return this.http.post<string>(`${this.baseApiUrl}login`, payload, {responseType: 'text' as 'json', withCredentials: true});
  }

  public sendToEmail(email: string): Observable<boolean> 
  {
    console.log(`Отправить сообщение на ${email}`);

    return this.http.post<boolean>(`${this.baseApiUrl}sendToEmail`, { value: email }, {
      headers: { 'Content-Type': 'application/json' },
    });
  }

  public checkCode(email: string, code: string): Observable<boolean> {
    console.log(`Проверка кода для ${email}`);
    return this.http.post<boolean>(`${this.baseApiUrl}checkCode/${email}`, { value: code }, {
      headers: { 'Content-Type': 'application/json' },
    });
  }

  public newPass(email: string, newPass: string): Observable<User> {
    console.log(`Изменение пароля для ${email}`);
    return this.http.post<User>(`${this.baseApiUrl}newPass/${email}`, { value: newPass }, {
      withCredentials: true,
      headers: { 'Content-Type': 'application/json' },
    });
  }
}