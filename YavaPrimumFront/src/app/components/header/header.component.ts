import { Component, OnInit } from '@angular/core';
import { User } from '../../data/interface/User.interface';
import { UserService } from '../../services/user/user.service';
import { RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-header',
  imports: [RouterOutlet, HttpClientModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  providers: [UserService]
})
export class HeaderComponent implements OnInit{
  user: User = {
    secondName: "",
    firstName: "",
    surName: "",
    company: "",
    country: "",
    email: "",
    post: "",
    phone: "",
    imgUrl: "profile_photo/default.jpg",
  };

  constructor(public userService: UserService){}

  ngOnInit(): void {
    this.userService.getUserData().subscribe(user =>
      {
        this.user = user
      }
    )
    
  }
}