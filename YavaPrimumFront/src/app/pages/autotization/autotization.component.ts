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
    const formValues = form.value;
    for (const key in formValues) 
      { 
        if (formValues[key] == "") 
        { 
          console.log(`${key}: ${formValues[key]}`); 
          return;
        }
     }

    let loginEror = document.getElementById('loginEror')!;

      this.authService.logIn(form.value).subscribe(response => 
      {
        loginEror.textContent = "Данные введены верно";
        loginEror.style.color = "green";
        console.log('Успешный вход:', response);
        if(response == "HR")
        {
          this.router.navigate(['/account/HR']);
        }
        else if (response == "Кадровик")
        {
          this.router.navigate(['/account/PO']);
        }

      }, 
      error => 
      {
        console.error('Ошибка входа:', error);

        loginEror.textContent = "Неверный логин или пароль";
      });

  }
}

