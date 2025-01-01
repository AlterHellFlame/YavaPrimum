import { computed, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DateTime, Info, Interval } from 'luxon';
import { Tasks } from '../../data/interface/Tasks.interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginModel } from '../../data/interface/loginModel.interfase';
import { Subject } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}

  //Актиный день
  private daySubject: BehaviorSubject<DateTime> = new BehaviorSubject<DateTime>(DateTime.now()); 
  public activeDay$: Observable<DateTime> = this.daySubject.asObservable();
  private activeDay = DateTime.now();
 
  //Все таски
  private allTasksSubject: BehaviorSubject<Tasks[]> = new BehaviorSubject<Tasks[]>([]); 
  public allTasks$: Observable<Tasks[]> = this.allTasksSubject.asObservable();
  private allTasks : Tasks[] = [];

  //Таски на активный день
  private dayTasksSubject: BehaviorSubject<Tasks[]> = new BehaviorSubject<Tasks[]>([]); 
  public dayTasks$: Observable<Tasks[]> = this.dayTasksSubject.asObservable();
  private dayTasks : Tasks[] = [];

  //Активный таск
  private taskClickSubject: BehaviorSubject<Tasks | null> = new BehaviorSubject<Tasks| null>(null); 
  taskClick$ = this.taskClickSubject.asObservable(); 
  
  setClickedTask(task: Tasks) 
  { 
    console.log("1.Задать " + task.taskResponseId + " " + task.candidate.secondName)
    this.taskClickSubject.next(task); 
  }

  public getAllTasks() : void
  {
    this.http.get<Tasks[]>(`${this.baseApiUrl}get-tasks`, { withCredentials: true }).subscribe(data=>
    {
      this.allTasks = data.map(task => (
      {
        ...task,
        dateTime: DateTime.fromISO(task.dateTime as unknown as string)
      }));
      console.log("Все таски юзера " + this.allTasks);
      this.allTasksSubject.next(this.allTasks);
      this.setClickedTask(this.allTasks[0]);
      this.setActiveDay(DateTime.now())
    });
  }

  public PassedInterview(taskId :string) : void
  {
    this.http.post(`${this.baseApiUrl}api/Tasks/PassedInterview${taskId}`, { withCredentials: true })
    .subscribe(
      response => {
        console.log('Успешный ответ:', response);
      },
      error => {
        console.error('Ошибка:', error);
      }
    );
  }

  public FaildInterview(taskId :string) : void
  {
    this.http.post(`${this.baseApiUrl}api/Tasks/PassedInterview${taskId}`, { withCredentials: true })
    .subscribe(
      response => {
        console.log('Успешный ответ:', response);
      },
      error => {
        console.error('Ошибка:', error);
      }
    );
  
  }

  public DeleteTask(taskId :string) : void
  {
    this.http.delete(`${this.baseApiUrl}api/Tasks/DeleteTask${taskId}`, { withCredentials: true })
    .subscribe(
      response => {
        console.log('Успешный ответ:', response);
      },
      error => {
        console.error('Ошибка:', error);
      }
    );
  
  
  }

  public setActiveDay(day : DateTime) : void
  {
    console.log("Активный день " + day.day);
    this.activeDay = day;
    this.daySubject.next(this.activeDay);
    this.getTasksOfDay();
  }

  public getTasksOfDay(): void
  {
    this.dayTasks = this.allTasks.filter(task => 
    {
      const taskDate = typeof task.dateTime === 'string' ? DateTime.fromISO(task.dateTime) : task.dateTime;
      return taskDate.hasSame(this.activeDay, 'day');
    });
    console.log("Таски на " + this.activeDay.day + " : " + this.dayTasks);
    this.dayTasksSubject.next(this.dayTasks);
  }
  
}


