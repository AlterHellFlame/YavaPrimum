import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeTaskComponent } from './change-task.component';

describe('ChangeTaskComponent', () => {
  let component: ChangeTaskComponent;
  let fixture: ComponentFixture<ChangeTaskComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeTaskComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChangeTaskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
