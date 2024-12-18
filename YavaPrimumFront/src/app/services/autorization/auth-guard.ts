import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AutorizationService } from './autorization.service'

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AutorizationService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.baseApiUrl) 
    {
      console.log("fff");
      return true;
    } else 
    {
      return false;
    }
  }
}
