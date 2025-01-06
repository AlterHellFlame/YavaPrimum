import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginModel } from '../../data/interface/loginModel.interfase';
import { Observable } from 'rxjs';
import { User } from '../../data/interface/User.interfase';

@Injectable({
  providedIn: 'root'
})
export class AutorizationService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}

  public logIn(payload : 
    {
      EMail: string;
      Password : string;
    }
  ) : Observable<string>
  {
    console.log(payload);
    return this.http.post(`${this.baseApiUrl}login`, payload, 
      { responseType: 'text' as 'json', withCredentials: true }) as Observable<string>
  }

  public sendToEmail(email: string): Observable<boolean> {
    console.log(`Отправить сообщение на ${email}`);
    
    const payload = { value: email };
  
    return this.http.post<boolean>(`${this.baseApiUrl}sendToEmail`, payload, {
      responseType: 'json', // Указание типа ответа как json
      withCredentials: true,
      headers: { 'Content-Type': 'application/json' }
    });
  }

  public checkCode(email: string, code: string): Observable<boolean> {
    console.log(`Отправить сообщение на ${email}`);
    
    const payload = { value: code };
  
    return this.http.post<boolean>(`${this.baseApiUrl}checkCode/${email}`, payload, {
      responseType: 'json', // Указание типа ответа как json
      withCredentials: true,
      headers: { 'Content-Type': 'application/json' }
    });
  }
  
  public newPass(email: string, newPass: string): Observable<User> {
    console.log(`Отправить сообщение на ${email}`);
    
    const payload = { value: newPass };
  
    return this.http.post<User>(`${this.baseApiUrl}newPass/${email}`, payload, {
      responseType: 'json', // Указание типа ответа как json
      withCredentials: true,
      headers: { 'Content-Type': 'application/json' }
    });
  }
  
}
