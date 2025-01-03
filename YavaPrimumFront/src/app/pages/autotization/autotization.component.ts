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
}
