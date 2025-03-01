import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../../data/interface/User.interface';
import { Notifications } from '../../data/interface/Notifications.interface';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}

  public getUserData() : Observable<User>
  {
      return this.http.get<User>(`${this.baseApiUrl}get-user-data`, { withCredentials: true });
  }

  public getNotifications() : Observable<Notifications[]>
  {
      return this.http.get<Notifications[]>(`${this.baseApiUrl}get-notifications`, { withCredentials: true });
  }
}
