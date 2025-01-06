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
export class AutotizationComponent {
  constructor(private authService: AutorizationService, private router: Router) {}

  onSubmitLogIns(form: NgForm): void {
    if (form.invalid) {
      console.log('Форма содержит ошибки');
      return;
    }

    let loginEror = document.getElementById('loginEror')!;

    loginEror.textContent = "Подождите..";
    loginEror.style.color = "black";
    this.authService.logIn(form.value).subscribe(
      response => {
        console.log('Успешный вход:', response);
        if (response == "HR") {
          this.router.navigate(['/account/HR']);
        } else if (response == "Кадровик") {
          this.router.navigate(['/account/PO']);
        }
      },
      error => {
        console.error('Ошибка входа:', error);

        loginEror.textContent = "Неверный логин или пароль";
        loginEror.style.color = "red";
      }
    );
  }

  Close(form: NgForm)
  {
    this.status = 0;
    document.getElementById('emailToPass')!.removeAttribute('disabled');
    document.getElementById('emailToPassError')!.textContent = "";
    document.getElementById('codeToPassError')!.textContent = "";
    document.getElementById('btnSuccess')!.removeAttribute('data-bs-dismiss');
    form.reset();
  }

  status = 0
  newPass(form: NgForm)
  {
    if(this.status == 0)//Подтвержение что почта есть в бд
    {
      let emailToPassError = document.getElementById('emailToPassError')!;
      console.log(form.value.emailToPass);
      this.authService.sendToEmail(form.value.emailToPass).subscribe(
      res => {
        if(res)
        {
          emailToPassError.textContent = "";
          document.getElementById('emailToPass')!.setAttribute('disabled', 'true');
          this.status++;
        }
        else
        {
          emailToPassError.textContent = "Пользователя с такой почтой не существует";
        }
      },
      );
      return;
    }
    if(this.status == 1)//Ожидание кода с почты
    {
      let codeToPassError = document.getElementById('codeToPassError')!;
      this.authService.checkCode(form.value.emailToPass, form.value.codeToPass).subscribe(
        res => {
          if(res)
          {
            codeToPassError.textContent = "";
            document.getElementById('codeToPass')!.setAttribute('disabled', 'true');
            document.getElementById('btnSuccess')!.setAttribute('data-bs-dismiss', 'modal');
            this.status++;
          }
          else
          {
            codeToPassError.textContent = "Введенный код неверный";
          }
        },
      );
      return;
    }
    if(this.status == 2)//Установка нового пароля
    {
      this.authService.newPass(form.value.emailToPass, form.value.passToPass).subscribe(
        res => {
          this.Close(form);
        },
      );
      return;
    }
  }
}
