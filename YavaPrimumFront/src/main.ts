import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .then(() => { console.log('Приложение успешно запущено.')})
  .catch((err) => console.error("Ошибки в main.ts " + err));


/*
if ('serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/ngsw-worker.js')
      .then(registration => {
        console.log('ServiceWorker работает : ', registration.scope);
      })
      .catch(error => {
        console.log('ServiceWorker не работает: ', error);
      });
  });
}*/



