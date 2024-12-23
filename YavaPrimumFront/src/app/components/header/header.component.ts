import { Component, Input, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { User } from '../../data/interface/User.interfase';
import { UserService } from '../../services/user/user.service';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-header',
  imports: [RouterModule, CommonModule, HttpClientModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  providers: [UserService]
})
export class HeaderComponent implements OnInit{
  user!: User;

  constructor(public userService: UserService){}

  ngOnInit(): void {
    this.userService.getUserDate().subscribe(user =>
      {
        this.user = user
      }
    )
  }
}
