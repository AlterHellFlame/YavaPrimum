import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AutorizationService } from '../../services/autorization/autorization.service';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-autotization',
  imports: [FormsModule, CommonModule, HttpClientModule],
  templateUrl: './autotization.component.html',
  styleUrl: './autotization.component.scss',
    providers: [AutorizationService] 
})
export class AutotizationComponent{

constructor(private authService : AutorizationService, private router : Router){}

  onSubmitLogIns(form: NgForm) : void
  {
    
    let loginEror = document.getElementById('loginEror')!;

      this.authService.logIn(form.value).subscribe(response => 
      {
        loginEror.textContent = "Данные введены верно";
        loginEror.style.color = "green";
        console.log('Успешный вход:', response);
         this.router.navigate(['/head']);
      }, 
      error => 
      {
        console.error('Ошибка входа:', error);

        loginEror.textContent = "Неверный логин или пароль";
      });
  }
}

