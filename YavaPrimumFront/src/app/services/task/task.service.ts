import { computed, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DateTime, Info, Interval } from 'luxon';
import { Tasks } from '../../data/interface/Tasks.interface';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){
    this.LoadAllTasks();
  }
 
  private allTasks : Tasks[] = [];

  //Активный таск
  private taskClickSubject: BehaviorSubject<Tasks | null> = new BehaviorSubject<Tasks| null>(null); 
  taskClick$ = this.taskClickSubject.asObservable(); 
  
  setClickedTask(task: Tasks) 
  { 
    console.log("1.Задать " + task.taskResponseId + " " + task.candidate.secondName)
    this.taskClickSubject.next(task); 
  }

  public LoadAllTasks() : void
  {
    console.log("LoadAllTasks");
    this.http.get<Tasks[]>(`${this.baseApiUrl}get-tasks`, { withCredentials: true }).subscribe(data =>
    {
      this.allTasks = data.map(task => (
        {
          ...task,
          dateTime: DateTime.fromISO(task.dateTime as unknown as string)
        }));
        console.log("Все таски юзера " + this.allTasks);
    }
    )
  }

  public GetAllTasks() : Tasks[]
  {
    console.log("GetAllTasks " + this.allTasks);
    return this.allTasks;
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
      && task.status === true;
    });
    
    // Сортировка задач по возрастанию даты
    return filteredTasks.sort((a, b) => a.dateTime.toMillis() - b.dateTime.toMillis());
  }
  

  public Interview(taskId :string, status: string) : void
  {
    const payload = { value: status };

    this.http.post(`${this.baseApiUrl}api/Tasks/Interview/${taskId}`, payload, { withCredentials: true })
    .subscribe(
      response => {
        console.log('Успешный ответ:', response);
        
      },
      error => {
        console.error('Ошибка:', error);
      }
    );
  
  }

  public RepeatInterview(taskId: string, dateTime: string): Observable<any> {
    console.log("Повторяем " + taskId + " с датой " + dateTime + " Типа " + typeof(dateTime));

    const payload = { value: dateTime };

    return this.http.post(
      `${this.baseApiUrl}api/Tasks/RepeatInterview/${taskId}`,
      payload, 
      {
        withCredentials: true,
        headers: { 'Content-Type': 'application/json' } 
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

  public getTasksOfDay(day: DateTime): Tasks[]
  {
    //if(this.allTasks.length == 0) this.LoadAllTasks();
    console.log("getTasksOfDay " +  day.day +" из : " + this.allTasks);
    let tasks =  this.allTasks
      .filter(task => task.dateTime.hasSame(day, 'day'))
      .sort((a, b) => {
        if (a.status !== b.status) {
          return a.status ? 1 : -1;
        }
        return a.dateTime.valueOf() - b.dateTime.valueOf();
      });
      return tasks;
  }
  
}


