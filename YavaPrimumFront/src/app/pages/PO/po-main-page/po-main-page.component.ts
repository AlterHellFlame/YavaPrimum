import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NotifyHubService } from '../../../services/notify-hub/notify-hub.service';

@Component({
  selector: 'app-po-main-page',
  imports: [CommonModule],
  templateUrl: './po-main-page.component.html',
  styleUrl: './po-main-page.component.scss'
})
export class PoMainPageComponent implements OnInit 
{
  constructor(private notify: NotifyHubService){}
  message: any;

  ngOnInit(): void {
    this.notify.startConnection();
    this.notify.addReceiveListener((message: string) => 
    {
      console.log("Сообщение доставлено")
      this.message =  message;
      alert("Заявка от HR " + this.message);
    });
  }
}
