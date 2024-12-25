import { TestBed } from '@angular/core/testing';
import { AnotherService } from './another.service';

describe('AnotherService', () => {
  let service: AnotherService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AnotherService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
