import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-calendar',
  imports: [CommonModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  calendar: number[][] = [];

  ngOnInit() {
    this.generateCalendar(2024, 0); // Январь 2024 года
  }

  generateCalendar(year: number, month: number) {
    const date = new Date(year, month);
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    let day = 1;
    let week: number[] = [];
    let startDay = date.getDay();
    
    // Начиная с понедельника (понедельник = 1)
    if (startDay === 0) {
      startDay = 7;
    }

    // Заполняем первую неделю пробелами до первого дня месяца
    for (let i = 1; i < startDay; i++) {
      week.push(0);
    }

    // Заполняем дни месяца
    while (day <= daysInMonth) {
      if (week.length === 7) {
        this.calendar.push(week);
        week = [];
      }
      week.push(day++);
    }

    // Заполняем пробелами оставшуюся часть последней недели
    while (week.length < 7) {
      week.push(0);
    }
    this.calendar.push(week);
  }
}
