import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Notifications } from '../../data/interface/Notifications.interface';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GlobalConfig, ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})

export class NotifyService {
  baseApiUrl = 'https://localhost:7247/';
  private hubConnection: signalR.HubConnection;

  constructor(private http : HttpClient, private toastr: ToastrService) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7247/chat')
      .build();
    this.configureToastr();
  }

  private configureToastr(): void {
    const options: Partial<GlobalConfig> = {
      positionClass: 'toast-top-right',
      maxOpened: 3,
      autoDismiss: true,
      preventDuplicates: true,
    };
    this.toastr.toastrConfig = {
      ...this.toastr.toastrConfig,
      ...options,
    };
  }

  public showError(message: string, title: string = 'Ошибка'): void {
    this.toastr.error(message, title, {
      timeOut: 4000,
      positionClass: 'toast-top-right',
    });
  }

  public showSuccess(message: string, title: string = 'Успех'): void {
    this.toastr.success(message, title, {
      timeOut: 3000,
      positionClass: 'toast-top-right',
    });
  }

  public showInfo(message: string, title: string): void {
    this.toastr.info(message, title, {
      timeOut: 2500,
      positionClass: 'toast-top-right',
    });
  }

  public startConnection(): Promise<void> {
    return this.hubConnection
      .start()
      .then(() => {
        console.log('Соединение подключено');
        return Promise.resolve();
      })
      .catch(err => {
        console.log('Ошибка соединения: ' + err);
        return Promise.reject(err);
      });
  }

  public addReceiveListener(callback: (message: string) => void): void {
    this.hubConnection.on('Receive', callback);
  }

  public async SendToUser(message: string): Promise<void> {
    console.log('Состояние соединения: ', this.hubConnection.state);
    if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
      console.log('Сообщение пытается');
      try {
        await this.hubConnection.invoke('SendToUser', message);
        console.log('Сообщение отправлено');
      } catch (err) {
        console.error('Ошибка отправки сообщения: ' + err);
      }
    } else {
      console.error('Соединение отсутствует');
    }
  }

  public getNotifications() : Observable<Notifications[]>
  {
      return this.http.get<Notifications[]>(`${this.baseApiUrl}get-notifications`, { withCredentials: true });
  }

  public readNotification(notificationId : string) : Observable<object>
  {
      return this.http.put(`${this.baseApiUrl}read-notification/${notificationId}`, {}, { withCredentials: true });
  }

  public setDate(notificationId : string, date: string) : Observable<object>
  {
      const dateTime = 
      {
        value: date
      };
      return this.http.put(`${this.baseApiUrl}set-date/${notificationId}`, dateTime, { withCredentials: true });
  }

    public setDateTask(taskId: string, date: string): Observable<object> 
    {
        const dateTime = { value: date };  // 'value' must match C# property name
        
        return this.http.put(
            `${this.baseApiUrl}set-date-task/${taskId}`, 
            dateTime, 
            { 
                withCredentials: true,
                headers: { 'Content-Type': 'application/json' }  // Explicit content type
            }
        )
    }
  public setTime(notificationId : string, date: string) : Observable<object>
  {
    console.log(notificationId + " " + date)
      const dateTime = 
      {
        value: date.toString()
      };
      return this.http.put(`${this.baseApiUrl}set-time/${notificationId}`, dateTime, { withCredentials: true });
  }

    public setTimeTask(taskId : string, date: string) : Observable<object>
  {
    console.log(taskId + " " + date)
      const dateTime = 
      {
        value: date.toString()
      };
      return this.http.put(`${this.baseApiUrl}set-time-task/${taskId}`, dateTime, { withCredentials: true });
  }
}
