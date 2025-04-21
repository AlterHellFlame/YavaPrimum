import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { Notifications } from '../../../data/interface/Notifications.interface';
import { DateTime } from 'luxon';
import { NotifyService } from '../../../services/notify/notify.service';
import { FormsModule } from '@angular/forms';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule, FormsModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  @ViewChild('dateTimeModal') dateTimeModal!: ElementRef;

  notifications: Notifications[] = [];
  filteredNotifications: Notifications[] = [];
  selectedNotification?: Notifications;
  
  showUnreadOnly = false;
  selectedDate = '';
  selectedTime = '';
  minDate = new Date().toISOString().split('T')[0];

  constructor(public notify: NotifyService) {}

  ngOnInit(): void {
    this.loadNotifications();
    
    this.notify.addReceiveListener(() => {
      this.loadNotifications();
    });
  }

  private loadNotifications(): void {
    this.notify.getNotifications().subscribe(notifications => {
      this.notifications = notifications;
      this.selectedNotification = notifications[0];
      this.filterNotifications();
    });
  }

  filterNotifications(): void {
    this.filteredNotifications = this.showUnreadOnly 
      ? this.notifications.filter(n => !n.isReaded) 
      : [...this.notifications];
  }

  openModal(): void {
    if (!this.dateTimeModal) {
      console.error("Modal element not found");
      return;
    }
    
    const modal = new bootstrap.Modal(this.dateTimeModal.nativeElement);
    modal.show();
  }

  closeModal(): void {
    const modal = bootstrap.Modal.getInstance(this.dateTimeModal.nativeElement);
    modal?.hide();
  }

  onSubmit(): void {
    if (!this.selectedTime && !this.selectedDate || !this.selectedNotification) return;

    console.log(this.selectedNotification!.task!.status)
    if(['Собеседование пройдено', 'Дата отказана', ].includes(this.selectedNotification!.task!.status))
    {
      const currentTime = DateTime.now().toFormat('HH:mm');
      const fullDateTime = DateTime.fromISO(`${this.selectedDate}T${currentTime}`);
      const formattedDateTime = fullDateTime.toFormat('dd.MM.yy HH:mm');

      console.log('Задать дату' + formattedDateTime);
      this.notify.setDate(this.selectedNotification.notificationsId, formattedDateTime).subscribe({
        next: () => {
          this.selectedNotification!.isReaded = true;
          this.notify.SendToUser("Прочитано");
          this.closeModal();
        },
        error: (err) => console.error('Ошибка:', err)
      });
    }
    else
      {
        console.log('Задать : ' + this.selectedTime);

        // Преобразуем дату из `dateTime` в ISO-формат
        const dateTimeLuxon = DateTime.fromISO(this.selectedNotification.task.dateTime.toString());
        const currentDate = dateTimeLuxon.toFormat('yyyy-MM-dd'); // Исправленный формат даты для luxon
        
        console.log('Задать время1: ' + currentDate);
        
        // Объединяем дату и время в ISO-формате
        const fullDateTime = DateTime.fromISO(`${currentDate}T${this.selectedTime}`);
        const formattedDateTime = fullDateTime.toFormat('dd.MM.yy HH:mm');
        
        console.log('Задать время2: ' + fullDateTime);
        console.log('Задать время: ' + formattedDateTime);
        
             
        this.notify.setTime(this.selectedNotification.notificationsId, formattedDateTime).subscribe({
          next: () => {
            this.selectedNotification!.isReaded = true;
            this.notify.SendToUser("Прочитано");
            this.closeModal();
          },
          error: (err) => console.error('Ошибка:', err)
        });
      }
  }

  markAsRead(notification: Notifications): void {
    const shouldOpenModal = [
      'Собеседование пройдено', 
      'Дата отказана',
      'Дата подтверждена',
      'Время отказано'
    ].includes(notification.task.status);

    if (shouldOpenModal) {
      this.selectedNotification = notification;
      this.openModal();
    } 
    else 
    {
      this.notify.readNotification(notification.notificationsId).subscribe({
        next: () => {
          notification.isReaded = true;
          this.notify.SendToUser("Прочитано");
        },
        error: (err) => console.error('Ошибка:', err)
      });
    }
  }

  getNotificationText(notification: Notifications): string {
    const statusTextMap: Record<string, string> = {
      'Собеседование пройдено': 'Взять кандидата',
      'Дата отказана': 'Выбрать дату',
      'Дата подтверждена': 'Выбрать время',
      'Время отказано': 'Выбрать время'
    };

    return statusTextMap[notification.task.status] || 'Прочитано';
  }

  toFormat(dateTime: string): string {
    return DateTime.fromISO(dateTime).toFormat('dd.MM.yy HH:mm');
  }
}