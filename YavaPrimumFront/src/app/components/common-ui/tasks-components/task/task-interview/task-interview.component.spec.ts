import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskInterviewComponent } from './task-interview.component';

describe('TaskInterviewComponent', () => {
  let component: TaskInterviewComponent;
  let fixture: ComponentFixture<TaskInterviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskInterviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TaskInterviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
