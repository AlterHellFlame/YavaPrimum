import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DateTime } from 'luxon';
import { Injectable } from '@angular/core';
import { Tasks, TasksRequest } from '../../data/interface/Tasks.interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { Candidate, CandidatesFullData } from '../../data/interface/Candidate.interface';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  baseApiUrl = 'https://localhost:7247/';
  private allTasks: Tasks[] = [];
  

  constructor(private http: HttpClient) {
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
    return this.http.get<Tasks[]>(`${this.baseApiUrl}get-all-archive-tasks`, { withCredentials: true });
  }

  public setAllTasks(tasks: Tasks[]): void {
    this.allTasks = tasks;
    //console.log("SetAllTasks " + this.allTasks);
    this.getAllTasksOfUser()
    
  }

  public getTasksOfDay(day: DateTime): Tasks[] {
    //console.log("Получение задач для дня: " + day.day + " из: " + this.allTasks);
    let tasks = this.allTasks
      .filter(task => task.dateTime.hasSame(day, 'day'))
      .sort((a, b) => a.dateTime.valueOf() - b.dateTime.valueOf());
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

  public async newStatus(tasksId: string, status: string): Promise<void> {
    console.log('newStatus ' + status + ' for id ' + tasksId);
    const payload = { value: status };

    try {
      const response = await this.http.put(
        `${this.baseApiUrl}new-status-for-task/${tasksId}`,
        payload,
        {
          withCredentials: true,
          headers: { 'Content-Type': 'application/json' },
        }
      ).toPromise();
      window.location.reload();
      console.log('Status updated successfully:', response);
    } catch (error) {
      console.error('Error updating status:', error);
    }
}


  public addNewTask(payload: {surname: string; firstName: string; patronymic: string;
    country: string; phone: string; email: string;
    post: string;
    interviewDate: string;
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
  
}


