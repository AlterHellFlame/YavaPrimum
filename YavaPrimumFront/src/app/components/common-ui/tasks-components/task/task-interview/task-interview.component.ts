import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Tasks } from '../../../../../data/interface/Tasks.interface';
import { DateTime } from 'luxon';
import { TaskService } from '../../../../../services/task/task.service';
import { NotifyService } from '../../../../../services/notify/notify.service';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

declare var bootstrap: any;

@Component({
  selector: 'app-task-interview',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './task-interview.component.html',
  styleUrls: ['./task-interview.component.scss']
})
export class TaskInterviewComponent {
  @Input() task: Tasks | null = null;

  selectedDecision: string = '';
  additionalData: string = '';
  newDate: string = DateTime.now().toFormat("yyyy-MM-dd'T'HH:mm");
  newDateTime: string = '';
  minDate: string = new Date().toISOString().slice(0, 16);
  minDateTime: string = new Date().toISOString().slice(0, 16);
  sendTestTask: boolean = false;
  activeModal: string | null = null;
  timeControl = new FormControl('', [
    Validators.required,
    this.validateTime.bind(this)
    ]);
  timeConflictError = false;

  isClose: boolean = false;

  constructor(
    private taskService: TaskService,
    private notify: NotifyService
  ) {}

  onDecisionChange() {
    console.log('Выбрано:', this.selectedDecision);
  }

  closeModal(modalId: string) {
    const modalElement = document.getElementById(modalId);
    if (modalElement) {
      const modal = bootstrap.Modal.getInstance(modalElement);
      modal?.hide();
    }
    this.task = null;
    window.location.reload();
  }

  onModalHidden() {
    this.task = null;
    this.activeModal = null;
  }

  submitDecision(status?: string) {
    const finalStatus = status || this.selectedDecision;
    
    if (!finalStatus) {
      this.notify.showError('Выберите решение!', 'Ошибка');
      return;
    }

    this.taskService.newStatus(
      this.task!.taskId,
      finalStatus,
      this.newDate,
      this.additionalData,
      this.sendTestTask ?? false
    ).then(t => {
      this.closeModal(this.activeModal!);
      this.notify.SendToUser('Статус изменен');
    });
  }

  getEventTypeText(): string {
    if (!this.task) return 'мероприятия';
    
    switch(this.task.status) {
      case 'Назначено собеседование':
        return 'собеседования';
      case 'Назначен приём':
        return 'приёма';
      case 'Срок тестового задания':
        return 'тестового задания';
      default:
        return 'мероприятия';
    }
  }

  confirmReschedule() {
    if (!this.newDateTime || !this.task) return;
    
    this.taskService.rescheduleEvent(this.task.taskId, this.newDateTime).subscribe({
      next: () => {
        this.notify.showSuccess('Дата успешно перенесена');
        this.notify.SendToUser('Смена даты');
        this.closeModal('ChangeDateTime');
      },
      error: (err) => {
        console.error('Ошибка переноса:', err);
        this.notify.showError('Не удалось перенести дату');
      }
    });
  }


  selectedDate = '';
  selectedTime = '';
  onDateSubmit(): void {
  if (!this.task || !this.selectedDate) return;
  
  const currentTime = DateTime.now().toFormat('HH:mm');
  const formattedDateTime = DateTime.fromISO(`${this.selectedDate}T${currentTime}`).toFormat('dd.MM.yy HH:mm');
  
  this.notify.setDateTask(this.task.taskId, formattedDateTime).subscribe({
    next: () => this.handleSuccess(),
    error: (err) => console.error('Ошибка:', err)
  });
}

onTimeSubmit(): void {
  if (!this.selectedTime || this.timeControl.invalid) {
    this.timeConflictError = true;
    return;
  }

  const taskDate = DateTime.fromISO(this.task!.dateTime.toString()).toISODate();
  const formattedDateTime = DateTime.fromISO(`${taskDate}T${this.selectedTime}`).toFormat('dd.MM.yy HH:mm');
  
  this.notify.setTimeTask(this.task!.taskId, formattedDateTime).subscribe({
    next: () => this.handleSuccess(),
    error: (err) => console.error('Ошибка:', err)
  });
}

private handleSuccess() {
  // Handle success logic here
  this.closeModal('ChangeKadrDate');
  this.closeModal('ChangeKadrTime');
  // Any other success handling
}


private validateTime(control: FormControl): { [key: string]: boolean } | null {
      
   console.log("ggg" + !control.value);
    if (!control.value) {
      return null;
    }

    const selectedTime = DateTime.fromISO(
      control.value.includes("T") 
        ? `2000-01-01T${DateTime.fromISO(control.value).toFormat('HH:mm:ss')}` // Если есть "T", значит это дата+время
        : `2000-01-01T${control.value}` // Если только время, добавляем фиксированную дату
    ).toMillis();

    const twentyMinutesInMs = 20 * 60 * 1000;

    // Get tasks for the same day as the selected notification
    const notificationDate = DateTime.fromISO(control.value);
    
    console.log(notificationDate);
    // This should be replaced with actual task fetching from your service
    const tasksForDay = this.taskService.getTasksOfDay(notificationDate!);// Adjust based on your actual service method
    
    console.log(tasksForDay?.length + "Задачи");
    if (!tasksForDay?.length) {
      return null;
    }

    const hasConflict = tasksForDay.some(task => {
      if (!task.dateTime) 
        {   
        console.log("1"+ task.dateTime);
        return false;
      }
      
      
      if(this.task == task)
      {
        return false;
      }
      const taskDateTime = DateTime.fromISO(task.dateTime.toString());
      console.log('taskDateTime '+ taskDateTime)

      if (taskDateTime.toISODate() !== notificationDate.toISODate()) {
        console.log("2");
        return false;
      }
      
      const taskTime = DateTime.fromISO(`2000-01-01T${taskDateTime.toFormat('HH:mm')}`).toMillis();
      const timeDifference = Math.abs(selectedTime - taskTime);
      console.log("3" + (selectedTime + " - " +  taskTime));

      console.log("4" + (timeDifference < twentyMinutesInMs));
      return timeDifference < twentyMinutesInMs;
    });

    console.log(hasConflict);
    return hasConflict ? { timeConflict: true } : null;
  }

}