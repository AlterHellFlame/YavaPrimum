import { Component, OnInit } from '@angular/core';
import { User } from '../../data/interface/User.interface';
import { UserService } from '../../services/user/user.service';
import { Router, RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NotifyService } from '../../services/notify/notify.service';

@Component({
  selector: 'app-header',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit{
  
  isAdmin: boolean = (localStorage.getItem('isAdmin') === 'true'? true : false);
  user: User = {
    userId: "",
    surname: "Пользователь",
    firstName: "не найден",
    patronymic: "",
    company: "",
    email: "",
    post: "",
    phone: "",
    imgUrl: "default.png",
  };;

  constructor(public userService: UserService, private router: Router, private notify: NotifyService){}

  countOfNotify = 0;
  ngOnInit(): void {
    this.userService.getUserData().subscribe({
      next: (user) => {
        this.user = user; // Если данные успешно получены
      },
      error: (err) => {
        if(err == "166")
        {
          this.router.navigate(['/log-in']);
        }
      }
    });
    
    this.notify.getNotifications().subscribe(notifications =>
      {
        this.countOfNotify = notifications.filter(notification => notification.isReaded === false).length;
      });


    this.notify.addReceiveListener((message) => {
      this.notify.getNotifications().subscribe(notifications =>
        {
          this.countOfNotify = notifications.filter(notification => notification.isReaded === false).length;
        });
    });
    
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}