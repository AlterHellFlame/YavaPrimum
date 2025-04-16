import { Location } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-not-found',
  imports: [],
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.scss'
})
export class NotFoundComponent {
  constructor(private location: Location) {}

  goBack(): void {
    this.location.back(); // Возврат на предыдущую страницу
  }
}
