import { Component, Input } from '@angular/core';
import { Candidate } from '../../../../data/interface/Candidate.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-candidate-card',
  standalone: true,
  imports: [],
  templateUrl: './candidate-card.component.html',
  styleUrl: './candidate-card.component.scss'
})
export class CandidateCardComponent
 {
  @Input() candidate!: Candidate;
}
