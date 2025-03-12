import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../../data/interface/User.interface';
import { Notifications } from '../../data/interface/Notifications.interface';
import { StringUnitLength } from 'luxon';

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

  public getAllUsersData() : Observable<User[]>
  {
      return this.http.get<User[]>(`${this.baseApiUrl}get-all-users-data`, { withCredentials: true });
  }

  addUser(user: User): Observable<User> 
  {

    // Создаем task объект, соответствующий структуре InterviewCreateRequest
    const userPayload = 
    {
      userId: user.userId,
      surname: user.surname,
      firstName: user.firstName,
      patronymic: user.patronymic,
      imgUrl: user.imgUrl,
      phone: user.phone,
      email: user.email,
      company: user.company,
      post: user.post,
    }
    
    console.log(user);
    // Отправляем POST запрос
    return this.http.post<User>(`${this.baseApiUrl}register`, userPayload, {
      withCredentials: true,
    });
  }

  updateUser(user: User): Observable<User> {
    // Создаем task объект, соответствующий структуре InterviewCreateRequest
    const userPayload = 
    {
      userId: user.userId,
      surname: user.surname,
      firstName: user.firstName,
      patronymic: user.patronymic,
      imgUrl: user.imgUrl,
      phone: user.phone,
      email: user.email,
      company: user.company,
      post: user.post,
    }
    
    console.log(user);
    // Отправляем POST запрос
    return this.http.post<User>(`${this.baseApiUrl}update-user-data`, userPayload, {
      withCredentials: true,
    });
  }

  deleteUser(userId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseApiUrl}delete-user/${userId}`);
  }
}
