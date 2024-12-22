import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginModel } from '../../data/interface/loginModel.interfase';
import { Observable } from 'rxjs';

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

  
}
