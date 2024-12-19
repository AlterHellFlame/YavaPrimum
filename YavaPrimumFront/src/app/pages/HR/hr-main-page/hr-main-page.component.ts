import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { CreateCandidateComponent } from "./create-candidate/create-candidate.component";
import { HttpClientModule } from '@angular/common/http';  // Импортируем HttpClientModule
import { CandidateService } from '../../../services/candidate/candidate.service';
import { Candidate } from '../../../data/interface/Candidate.interface';
import { CandidateCardComponent } from "./candidate-card/candidate-card.component";
import { CalendarComponent } from './calendar/calendar.component';

@Component({
  selector: 'app-hr-main-page',
  standalone: true,  // Убедитесь, что компонент standalone
  imports: [CommonModule, CreateCandidateComponent, HttpClientModule,
            CandidateCardComponent, CalendarComponent],  // Добавляем HttpClientModule сюда
  templateUrl: './hr-main-page.component.html',
  styleUrls: ['./hr-main-page.component.scss'],
  providers: [CandidateService] 
})
export class HrMainPageComponent implements OnInit {
  candidates: Candidate[] = [];

  constructor(private candidateService: CandidateService)
  {
    this.candidateService.getProductCards().subscribe((val) => 
    {
      console.log(val);
      this.candidates = val;
    });
  }

  ngOnInit(): void {
    // Логика инициализации
  }


 isDropdownOpen = false;

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  applyFilter(filter: string) {
    console.log(`Применён фильтр: ${filter}`);
  }
}
