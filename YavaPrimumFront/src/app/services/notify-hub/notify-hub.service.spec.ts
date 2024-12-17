import { TestBed } from '@angular/core/testing';

import { NotifyHubService } from './notify-hub.service';

describe('NotifyHubService', () => {
  let service: NotifyHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotifyHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
