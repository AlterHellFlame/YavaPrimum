import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';

@Component({//Спросить Сашу
  selector: 'app-authorization',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './authorization.component.html',
  styleUrl: './authorization.component.scss',
  providers: [AuthService]
})
export class AuthorizationComponent {

  constructor(private authService: AuthService,  private router: Router){}
  loginError: string = ''
  status: number = 0; 

  LogIn(form: NgForm): void 
  {
    if (form.invalid) 
    {
      console.log('Форма содержит ошибки');
      return;
    }

    this.authService.logIn(form.value).subscribe({ 
      next: (response) => {
        console.log('Успешный вход:', response);
        
        if (response == "HR") 
        {
          localStorage.setItem('isHR', 'true');
          localStorage.setItem('isAdmin', 'false');
          this.router.navigate(['/account/hr']);
        } 
        else if (response == "Кадровик") 
        {
          localStorage.setItem('isHR', 'false');
          localStorage.setItem('isAdmin', 'false');
          this.router.navigate(['/account/po']);
        }
        else if (response == "!Админ") 
        {
            localStorage.setItem('isHR', 'false');
            localStorage.setItem('isAdmin', 'true');
            this.router.navigate(['/account/admin']);
        }
      },
      error: (error) => {
        if(error.status == 500)
        {
          this.loginError = 'Неверный логин или пароль';
        }
        else
        {
            this.loginError = 'Ошибка соединения';
        }
      }
    }
    );

  }

  emailToPass: string = "";
  newPass(form: NgForm) {
  if (this.status === 0) {
    this.emailToPass = form.value.emailToPass;
    this.authService.sendToEmail(form.value.emailToPass).subscribe(
      res => {
        if (res) {
          form.controls['emailToPass'].disable();
          this.status++;
        } else {
          form.controls['emailToPass'].setErrors({ notExist: true }); // Добавляем ошибку
        }
      }
    );
  } else if (this.status === 1) {
    this.authService.checkCode(this.emailToPass, form.value.codeToPass).subscribe(
      res => {
        console.log("Почта " + res);
        if (res) {
          form.controls['codeToPass'].disable();
          this.status++;
        } else {
          form.controls['codeToPass'].setErrors({ invalidCode: true }); // Добавляем ошибку
        }
      }
    );
  } else if (this.status === 2) {
    this.authService.newPass(this.emailToPass, form.value.passToPass).subscribe(
      res => {
        this.Close(form);
      }
    );
  }
}


  Close(form: NgForm) {
    this.status = 0;
    form.controls['emailToPass'].enable();
    form.controls['emailToPass'].setErrors(null);
    form.controls['codeToPass']?.setErrors(null);
    form.controls['codeToPass']?.enable();
    form.resetForm();
  }
}
