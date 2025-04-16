import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CandidatePageComponent } from './candidate-page.component';

describe('CandidatePageComponent', () => {
  let component: CandidatePageComponent;
  let fixture: ComponentFixture<CandidatePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CandidatePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CandidatePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
