import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class NotifyService {
  
  private hubConnection: signalR.HubConnection;

  constructor() {
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
}
