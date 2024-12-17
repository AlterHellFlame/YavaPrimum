import { Component, OnInit } from '@angular/core';

interface Day {
  date: number;
  tasks: string[];
}

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  days: Day[] = [];

  ngOnInit() {
    // Пример данных, вы можете заменить их на ваши
    this.days = [
      { date: 1, tasks: ['Task 1'] },
      { date: 2, tasks: ['Task 2'] },
      { date: 3, tasks: ['Task 3'] },
      { date: 4, tasks: ['Task 4'] },
      { date: 5, tasks: ['Task 5'] },
      { date: 6, tasks: ['Task 6'] },
      { date: 7, tasks: ['Task 7'] },
      // Добавьте больше дней и задач по вашему усмотрению
    ];
  }
}
