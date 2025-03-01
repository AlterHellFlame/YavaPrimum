import { Component, OnInit } from '@angular/core';
import { User } from '../../data/interface/User.interface';
import { UserService } from '../../services/user/user.service';
import { Router, RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit{
  
  user: User = {
    surname: "Пользователь",
    firstName: "не найден",
    patronymic: "",
    company: "",
    email: "",
    post: "",
    phone: "",
    imgUrl: "profile_photo/default.jpg",
  };

  constructor(public userService: UserService, private router: Router){}

  countOfNotify = 0;
  ngOnInit(): void {
    this.userService.getUserData().subscribe(user =>
      {
        this.user = user
      }
    )
    this.userService.getNotifications().subscribe(notifications =>
      {
        this.countOfNotify = notifications.length;
      }
    )
    
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}