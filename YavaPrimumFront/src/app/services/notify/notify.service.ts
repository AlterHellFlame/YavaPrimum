import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Notifications } from '../../data/interface/Notifications.interface';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotifyService {
  baseApiUrl = 'https://localhost:7247/';
  private hubConnection: signalR.HubConnection;

  constructor(private http : HttpClient) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7247/chat')
      .build();
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


}
