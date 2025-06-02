import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { Notifications } from '../../../data/interface/Notifications.interface';
import { DateTime } from 'luxon';
import { NotifyService } from '../../../services/notify/notify.service';
import { FormsModule, ReactiveFormsModule, FormControl, ValidatorFn, AbstractControl, ValidationErrors, Validators } from '@angular/forms';
import * as bootstrap from 'bootstrap';
import { TaskService } from '../../../services/task/task.service';

// Конфигурация статусов
const STATUS_CONFIG = {
  DATE_REQUIRED: ['Собеседование пройдено', 'Выполнено тестовое задание', 'Дата отказана', 'Запрос на смену даты'],
  TIME_REQUIRED: ['Дата подтверждена', 'Время отказано', 'Запрос на смену времени'],
  MODAL_ACTIONS: [
    'Собеседование пройдено', 
    'Выполнено тестовое задание',
    'Дата отказана',
    'Дата подтверждена',
    'Время отказано',
    'Запрос на смену даты',
    'Запрос на смену времени'
  ],
  MODAL_CONFIRMED: [
    'Взят кандидат',
    'Ожидается подтверждение даты',
    'Ожидается подтверждение времени',
  ]
};

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  @ViewChild('dateTimeModal') dateTimeModal!: ElementRef;
  @ViewChild('confirmModal') confirmModal!: ElementRef;

  private readonly BUTTON_TEXTS: Record<string, string> = {
    'Собеседование пройдено': 'Взять кандидата',
    'Выполнено тестовое задание': 'Взять кандидата',
    'Дата отказана': 'Выбрать дату',
    'Дата подтверждена': 'Выбрать время',
    'Время отказано': 'Выбрать время',
    'Запрос на смену даты': 'Выбрать дату',
    'Запрос на смену времени': 'Выбрать время',
    'Взят кандидат': 'Подтвердить дату',
    'Ожидается подтверждение даты': 'Подтвердить дату',
    'Ожидается подтверждение времени': 'Подтвердить время',
  };

  notifications: Notifications[] = [];
  filteredNotifications: Notifications[] = [];
  selectedNotification?: Notifications;
  
  showUnreadOnly = false;
  selectedDate = '';
  selectedTime = '';
  minDate = DateTime.now().toISODate();
  confirmModalTitle = '';
  confirmType: 'date' | 'time' | null = null;
  timeControl = new FormControl('', [
    Validators.required,
    this.validateTime.bind(this)
    ]);
  timeConflictError = false;

  private currentModal?: bootstrap.Modal;

  constructor(public notify: NotifyService, public taskService: TaskService) {}

  ngOnInit(): void {
    this.loadNotifications();
    this.notify.addReceiveListener(() => this.loadNotifications());

  this.taskService.loadAllTasks().subscribe({
      next: data => {
        let allTasks = data.map(task => ({
          ...task,
          dateTime: DateTime.fromISO(task.dateTime as unknown as string)
        }));
        console.log(allTasks.length);
        this.taskService.setAllTasks(allTasks);
      }});
  }

  private loadNotifications(): void {
    this.notify.getNotifications().subscribe(notifications => {
      this.notifications = notifications;
      this.filteredNotifications = this.notifications;
    });
  }


candidateLastNameFilter: string = '';
candidatePositionFilter: string = '';
senderLastNameFilter: string = '';

filterNotifications(): void {
  this.filteredNotifications = this.notifications.filter(notification => {
    // Фильтр по прочитанности
    if (this.showUnreadOnly && notification.isReaded) {
      return false;
    }
    
    // Фильтр по фамилии кандидата (регистронезависимый)
    if (this.candidateLastNameFilter && 
        !notification.task.candidate.surname?.toLowerCase().includes(this.candidateLastNameFilter.toLowerCase())) {
      return false;
    }
    
    // Фильтр по должности кандидата (регистронезависимый)
    if (this.candidatePositionFilter && 
        !notification.task.candidate.post.toLowerCase().includes(this.candidatePositionFilter.toLowerCase())) {
      return false;
    }
    
    // Фильтр по фамилии отправителя (регистронезависимый)
    if (this.senderLastNameFilter && 
        !notification.user.surname?.toLowerCase().includes(this.senderLastNameFilter.toLowerCase())) {
      return false;
    }
    
    return true;
  });
}
  openModal(): void {
    if (!this.dateTimeModal) return;
    this.currentModal = new bootstrap.Modal(this.dateTimeModal.nativeElement);
    this.currentModal.show();
  }

  openConfirmModal(): void {
    if (!this.selectedNotification || !this.confirmModal) return;

    const status = this.selectedNotification.task.status;
    
    if (status === 'Ожидается подтверждение даты' || status === 'Взят кандидат') {
      this.confirmModalTitle = 'Подтвердить дату собеседования?';
      this.confirmType = 'date';
    } else if (status === 'Ожидается подтверждение времени') {
      this.confirmModalTitle = 'Подтвердить время собеседования?';
      this.confirmType = 'time';
    } else {
      return;
    }

    this.currentModal = new bootstrap.Modal(this.confirmModal.nativeElement);
    this.currentModal.show();
  }

  closeModal(): void {
    if (this.currentModal) {
      this.currentModal.hide();
      this.currentModal = undefined;
    }
    this.timeConflictError = false;
  }

  onSubmit(): void {
    if (!this.selectedNotification) return;

    const config = this.getDateTimeConfig();
    if (!config) return;

    if (config.requiresTime && this.timeControl.invalid) {
      this.timeConflictError = true;
      return;
    }

    this.processDateTime(config);
  }

  submitDecision(confirmed: boolean): void {
    if (!this.selectedNotification || !this.confirmType) return;

    let status: string;
    
    if (this.confirmType === 'date') {
      status = confirmed ? 'Дата подтверждена' : 'Дата отказана';
    } else {
      status = confirmed ? 'Время подтверждено' : 'Время отказано';
    }

    this.selectedNotification.isReaded = true;
    this.taskService.ConfirmDateTime(
      this.selectedNotification.task.taskId, 
      status, 
      DateTime.now().toFormat("yyyy-MM-dd'T'HH:mm")
    ).then(t => {
      this.closeModal();
      this.notify.SendToUser('Статус изменен');
    });
  }

  private validateTime(control: FormControl): { [key: string]: boolean } | null {
    if (!this.selectedNotification || !control.value) {
      return null;
    }

    const selectedTime = DateTime.fromISO(`2000-01-01T${control.value}`).toMillis();
    const twentyMinutesInMs = 20 * 60 * 1000;

    // Get tasks for the same day as the selected notification
    const notificationDate = DateTime.fromISO(this.selectedNotification.task.dateTime.toString());
    
    // This should be replaced with actual task fetching from your service
    const tasksForDay = this.taskService.getTasksOfDay(notificationDate!);// Adjust based on your actual service method
    
    if (!tasksForDay?.length) {
      console.log('Нуль')
      return null;
    }
    //console.log(tasksForDay?.length);

    const hasConflict = tasksForDay.some(task => {
      if (!task.dateTime || task.taskId === this.selectedNotification?.task.taskId) {
        return false;
      }
      
      const taskDateTime = DateTime.fromISO(task.dateTime.toString());
      if (taskDateTime.toISODate() !== notificationDate.toISODate()) {
        return false;
      }
      
      const taskTime = DateTime.fromISO(`2000-01-01T${taskDateTime.toFormat('HH:mm')}`).toMillis();
      const timeDifference = Math.abs(selectedTime - taskTime);
      
      //console.log(hasConflict);
      return timeDifference < twentyMinutesInMs;
    });

    console.log(hasConflict);
    hasConflict ? { timeConflict: true } : null;
    
    return hasConflict ? { timeConflict: true } : null;
  }

  private getDateTimeConfig() {
    if (!this.selectedNotification) return null;
    
    const status = this.selectedNotification.task.status;
    
    if (STATUS_CONFIG.DATE_REQUIRED.includes(status) && !this.selectedDate) {
      return null;
    }
    
    if (STATUS_CONFIG.TIME_REQUIRED.includes(status) && !this.selectedTime) {
      return null;
    }
    
    return {
      status,
      requiresDate: STATUS_CONFIG.DATE_REQUIRED.includes(status),
      requiresTime: STATUS_CONFIG.TIME_REQUIRED.includes(status)
    };
  }

  private processDateTime(config: any) {
    let formattedDateTime = '';
    
    if (config.requiresDate) {
      const currentTime = DateTime.now().toFormat('HH:mm');
      formattedDateTime = DateTime.fromISO(`${this.selectedDate}T${currentTime}`).toFormat('dd.MM.yy HH:mm');
    } else if (config.requiresTime) {
      const taskDate = DateTime.fromISO(this.selectedNotification!.task.dateTime.toString()).toISODate();
      formattedDateTime = DateTime.fromISO(`${taskDate}T${this.selectedTime}`).toFormat('dd.MM.yy HH:mm');
    }

    this.sendDateTimeUpdate(formattedDateTime, config);
  }

  private sendDateTimeUpdate(dateTime: string, config: any) {
    const serviceMethod = config.requiresDate ? 'setDate' : 'setTime';
    
    this.notify[serviceMethod](this.selectedNotification!.notificationsId, dateTime).subscribe({
      next: () => this.handleSuccess(),
      error: (err) => console.error('Ошибка:', err)
    });
  }

  private handleSuccess() {
    if (this.selectedNotification) {
      this.selectedNotification.isReaded = true;
    }
    this.notify.SendToUser("Прочитано");
    this.closeModal();
    this.resetSelections();
  }

  private resetSelections() {
    this.selectedDate = '';
    this.selectedTime = '';
    this.timeControl.reset();
    this.timeConflictError = false;
  }

  markAsRead(notification: Notifications): void {
    if (STATUS_CONFIG.MODAL_ACTIONS.includes(notification.task.status)) {
      this.selectedNotification = notification;
      this.openModal();
    } 
    else if (STATUS_CONFIG.MODAL_CONFIRMED.includes(notification.task.status)) {
      this.selectedNotification = notification;
      this.openConfirmModal();
    } 
    else {
      this.notify.readNotification(notification.notificationsId).subscribe({
        next: () => {
          notification.isReaded = true;
          this.notify.SendToUser("Прочитано");
        },
        error: (err) => {
          console.error('Ошибка:', err)
          this.notify.showError("Упс...")
        }
      });
    }
  }

  getNotificationText(notification: Notifications): string {
    return this.BUTTON_TEXTS[notification.task.status] || "Прочитано";
  }

  toFormat(dateTime: string): string {
    return DateTime.fromISO(dateTime).toFormat('dd.MM.yy HH:mm');
  }

  getModalTitle(): string {
    if (!this.selectedNotification) return 'Выберите значение';
    return STATUS_CONFIG.DATE_REQUIRED.includes(this.selectedNotification.task.status) 
      ? 'Выберите дату' 
      : 'Выберите время';
  }

  getInputLabel(): string {
    if (!this.selectedNotification) return '';
    return STATUS_CONFIG.DATE_REQUIRED.includes(this.selectedNotification.task.status) 
      ? 'Дата' 
      : 'Время';
  }

  isDateInput(): boolean {
    return this.selectedNotification 
      ? STATUS_CONFIG.DATE_REQUIRED.includes(this.selectedNotification.task.status)
      : false;
  }

  isTimeInput(): boolean {
    return this.selectedNotification 
      ? STATUS_CONFIG.TIME_REQUIRED.includes(this.selectedNotification.task.status)
      : false;
  }
}