import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DateTime } from 'luxon';
import { Injectable } from '@angular/core';
import { Tasks, TasksRequest } from '../../data/interface/Tasks.interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { Candidate, CandidatesFullData } from '../../data/interface/Candidate.interface';
import { NotifyService } from '../notify/notify.service';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  baseApiUrl = 'https://localhost:7247/';
  private allTasks: Tasks[] = [];
  

  constructor(private http: HttpClient, private notify: NotifyService) {
  }

  public loadAllTasks(): Observable<Tasks[]> {
    return this.http.get<Tasks[]>(`${this.baseApiUrl}get-all-tasks-of-user`, { withCredentials: true });
  }

  public getAllTasksOfUser(): Tasks[] {
    //console.log("GetAllTasks " + this.allTasks);
    return this.allTasks;
  }

  
  public getAllTasks(): Observable<Tasks[]> {
    return this.http.get<Tasks[]>(`${this.baseApiUrl}get-all-tasks`, { withCredentials: true });
  }

  public getAllArchiveTasks(): Observable<Tasks[]> {
    return this.http.get<Tasks[]>(`${this.baseApiUrl}get-all-tasks`, { withCredentials: true });
  }

  public setAllTasks(tasks: Tasks[]): void {
    this.allTasks = tasks;
    //console.log("SetAllTasks " + this.allTasks);
    this.getAllTasksOfUser()
    
  }

  public getTasksOfDay(day: DateTime): Tasks[] {
      let tasks = this.allTasks
        .filter(task => 
          task.dateTime.hasSame(day, 'day') && task.typeStatus !== -1 && task.typeStatus !== -2
        ) // ✅ Фильтруем задачи, исключая статусы -1 и -2
        .sort((a, b) => {
          let statusA = (a.typeStatus === 0 || a.typeStatus === 3) ? 0 : 1;
          let statusB = (b.typeStatus === 0 || b.typeStatus === 3) ? 0 : 1;
          
          if (statusA !== statusB) {
            return statusA - statusB; // ✅ Гарантируем, что 0 и 3 идут первыми
          }

          return a.dateTime.valueOf() - b.dateTime.valueOf(); // ✅ Затем сортируем по дате
        });

      return tasks;
  }



  public filterAndSortTasks(tasks: Tasks[]): Tasks[] 
  {
    const currentMonth = DateTime.now().month;
    const currentYear = DateTime.now().year;
    
    // Фильтрация задач, относящихся к текущему месяцу
    const filteredTasks = tasks.filter(task => 
    {
      return task.dateTime.month === currentMonth 
      && task.dateTime.year === currentYear 
      /*&& task.status === true*/;
    });
    
    // Сортировка задач по возрастанию даты
    return filteredTasks.sort((a, b) => a.dateTime.toMillis() - b.dateTime.toMillis());
  }

  public async newStatus(
      taskId: string, 
      status: string, 
      newDate: string, 
      additionalData?: string, 
      isTestTask: boolean = false
  ): Promise<void> {
      console.log(`Updating status: ${status}, Task: ${taskId}`);

      try {
          const response = await this.http.put(
              `${this.baseApiUrl}new-status-for-task/${taskId}`,
              { status, additionalData, newDateTime: newDate, isTestTask },
              { withCredentials: true, headers: { 'Content-Type': 'application/json' } }
          ).toPromise();
          
          console.log('Status updated:', response);
      } catch (error) {
          console.error('Update error:', error);
          this.notify.showError("Упс...");
          throw error;
      }
  }

  public async ConfirmDateTime(
      taskId: string, 
      status: string, 
      newDate: string, 
      additionalData?: string, 
      isTestTask: boolean = false
  ): Promise<void> {
      console.log(`Updating status: ${status}, Task: ${taskId}`);

      try {
          const response = await this.http.put(
              `${this.baseApiUrl}confirm-dateTime/${taskId}`,
              { status, additionalData, newDateTime: newDate, isTestTask },
              { withCredentials: true, headers: { 'Content-Type': 'application/json' } }
          ).toPromise();
          
          console.log('Status updated:', response);
      } catch (error) {
          console.error('Update error:', error);
          this.notify.showError("Упс...");
          throw error;
      }
  }



  public addNewTask(payload: {surname: string; firstName: string; patronymic: string;
    country: string; phone: string; email: string;
    post: string;
    interviewDate: string; additionalData: string;
  }) : Observable<TasksRequest>{
    
    // Создаем task объект, соответствующий структуре InterviewCreateRequest
    const task = {
      candidate: {
        surname: payload.surname,
        firstName: payload.firstName,
        patronymic: payload.patronymic,
        email: payload.email,
        phone: payload.phone,
        country: payload.country,
        post: payload.post
      },
      interviewDate: payload.interviewDate,
      additionalData: payload.additionalData
    };
    
    console.log(task);
    // Отправляем POST запрос
    return this.http.post<TasksRequest>(`${this.baseApiUrl}create-new-task`, task, {
      withCredentials: true,
    });
  }

    
  public getAllCandidatesOfUser(): Observable<CandidatesFullData[]> {
    return this.http.get<CandidatesFullData[]>(`${this.baseApiUrl}get-all-candidates-of-user`, { withCredentials: true });
  }
  
        
  public rescheduleEvent(taskId: string, DateTime: string): Observable<any> {
    const newDateTime = { value: DateTime }; // ✅ Переменная вынесена отдельно
    
    return this.http.put(
        `${this.baseApiUrl}change-dateTime-without-check/${taskId}`,
        newDateTime, // ✅ Передаём объект
        {
            withCredentials: true,
            headers: { 'Content-Type': 'application/json' },
        }
    );
  }

  
  public updateTaskDateTime(updateData: { taskId: string, isChangeDate: boolean, additionalInfo?: string }): Observable<any> {
    console.log('updateTaskDateTime');
    return this.http.put(
        `${this.baseApiUrl}change-dateTime/${updateData.taskId}`,
        updateData, // ✅ Передаём объект целиком
        {
            withCredentials: true,
            headers: { 'Content-Type': 'application/json' },
        }
    );
}


}


