import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../../data/interface/User.interfase';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}


  public getUserDate() : Observable<User>
  {
      return this.http.get<User>(`${this.baseApiUrl}userData`, { withCredentials: true });
  }
}
