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

  public GetByEmail(email : string) : Observable<boolean>
  {
    console.log(email);
    return this.http.post(`${this.baseApiUrl}getByEmail`, email, {withCredentials: true }) as Observable<boolean>
  }
  
}
