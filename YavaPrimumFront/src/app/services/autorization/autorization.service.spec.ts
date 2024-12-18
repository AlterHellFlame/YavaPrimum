import { TestBed } from '@angular/core/testing';
import { AutorizationService } from './autorization.service';

describe('AutorizationService', () => {
  let service: AutorizationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AutorizationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
