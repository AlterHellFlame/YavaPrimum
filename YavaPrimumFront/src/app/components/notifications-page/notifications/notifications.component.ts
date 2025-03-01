import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { Notifications } from '../../../data/interface/Notifications.interface';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  notifications: Notifications[] = [];

  constructor(public userService: UserService) {}

  ngOnInit(): void {
    this.userService.getNotifications().subscribe(notifications =>
      {
        this.notifications = notifications;
        console.log('BRUH' + notifications[0].task.user.surname)

      }
    )
  }

  public toFormat(dateTime: string): string
  {
    return DateTime.fromISO(dateTime).toFormat('dd.MM.yy HH:mm').toString();
  }

  markAsRead(notification: Notifications): void {
  }
}