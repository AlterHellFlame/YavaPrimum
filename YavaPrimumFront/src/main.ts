import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .then(() => { console.log('Приложение успешно запущено.')})
  .catch((err) => console.error("Ошибки в main.ts " + err));
