import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { Notifications } from '../../../data/interface/Notifications.interface';
import { DateTime } from 'luxon';
import { NotifyService } from '../../../services/notify/notify.service';
import { FormsModule, NgModel } from '@angular/forms';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule, FormsModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  notifications: Notifications[] = [];

  constructor(public notify: NotifyService) {}
  filteredNotifications: Notifications[] = [];
  showUnreadOnly: boolean = false;

  filterNotifications(): void {
    if (this.showUnreadOnly) {
      this.filteredNotifications = this.notifications.filter(notification => !notification.isReaded);
    } else {
      this.filteredNotifications = this.notifications;
    }
  }
  ngOnInit(): void {
    this.notify.getNotifications().subscribe(notifications =>
      {
        this.notifications = notifications;
        console.log(notifications[0].notificationsId)
        this.filterNotifications();
      });
    
    this.notify.addReceiveListener((message) => {
      this.notify.getNotifications().subscribe(notifications =>
        {
          this.notifications = notifications;
          console.log(notifications[0].notificationsId)
  
        });
    });
  }

  public toFormat(dateTime: string): string
  {
    return DateTime.fromISO(dateTime).toFormat('dd.MM.yy HH:mm').toString();
  }

  markAsRead(notification: Notifications): void 
  {
    console.log('Читаем сообщение' + notification.notificationsId );
    this.notify.readNotification(notification.notificationsId).subscribe(
      t => 
      {
        console.log('Успех');
        notification.isReaded = true;

        this.notify.SendToUser("Прочитано");
      }
    );
  }
}