import { computed, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DateTime, Info, Interval } from 'luxon';
import { Tasks } from '../../data/interface/Tasks.interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginModel } from '../../data/interface/loginModel.interfase';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}


  private daySubject: BehaviorSubject<DateTime> = new BehaviorSubject<DateTime>(DateTime.now()); 
  public activeDay$: Observable<DateTime> = this.daySubject.asObservable();


  public getAllTasks() : Observable<Tasks[]>
  {
    return this.http.get<Tasks[]>(`${this.baseApiUrl}api/HR`);
  }


  //dayTasks: Tasks[] = [];

  public setActiveDay(day : DateTime) : void
  {
    console.log("Активный день " + day);
    this.daySubject.next(day);
  }

    /*public getTasks(): void 
    {
        const activeDay = this.activeDay();
        if (activeDay == null) 
        {
          this.dayTasks = [];
          this.tasksSubject.next(this.dayTasks);
        } 
        else 
        {
          this.dayTasks = this.tasks.filter(task =>
            task.dateTime.hasSame(activeDay, 'day')
          );
          this.tasksSubject.next(this.dayTasks);
        }
    }*/
}


