import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutotizationComponent } from './autotization.component';

describe('AutotizationComponent', () => {
  let component: AutotizationComponent;
  let fixture: ComponentFixture<AutotizationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AutotizationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AutotizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
