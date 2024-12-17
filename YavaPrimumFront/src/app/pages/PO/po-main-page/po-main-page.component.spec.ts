import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoMainPageComponent } from './po-main-page.component';

describe('PoMainPageComponent', () => {
  let component: PoMainPageComponent;
  let fixture: ComponentFixture<PoMainPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PoMainPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PoMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
