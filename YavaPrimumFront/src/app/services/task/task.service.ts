import { computed, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DateTime, Info, Interval } from 'luxon';
import { Tasks } from '../../data/interface/Tasks.interface';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}

  /*public getTasks() : Observable<Task>
  {
    return this.http.post<Task>(`${this.baseApiUrl}`);
  }*/ 

    private tasksSubject: BehaviorSubject<Tasks[]> = new BehaviorSubject<Tasks[]>([]); 
    public dayTasks$: Observable<Tasks[]> = this.tasksSubject.asObservable();

    activeDay : WritableSignal<DateTime | null> = signal(null);
    dayTasks: Tasks[] = [];

    public setActiveDay(day : DateTime) : void
    {
      this.activeDay.set(day);
      console.log("Активный день " + this.activeDay()?.day);
      this.getTasks();
    }

    public getTasks(): void 
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
    }

    public tasks: Tasks[] = 
    [
      {
          secondName: 'Ivanov',
          status: 'Pending',
          dateTime: DateTime.local(2024, 12, 18, 10, 0)
      },
      {           
        secondName: 'Petrov',
        status: 'Completed',
        dateTime: DateTime.local(2024, 12, 20, 14, 30)
      }
    ];
}


