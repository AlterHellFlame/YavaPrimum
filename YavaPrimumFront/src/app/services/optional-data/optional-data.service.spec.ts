import { TestBed } from '@angular/core/testing';

import { OptionalDataService } from './optional-data.service';

describe('OptionalDataService', () => {
  let service: OptionalDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OptionalDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
