import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { InitService } from '../core/services/init-service';
import { lastValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withViewTransitions()),
    provideHttpClient(),
    // initialize the app
    provideAppInitializer(async ()=> {
      const initService = inject(InitService);

      // delay the splash screen
      return new Promise<void>((resolve) => {
        setTimeout(async () => {
          try{
            // load the auth user
            return lastValueFrom(initService.init());
          } finally {
            // when the user is loaded, remove the splash screen
            const splash = document.getElementById('initial-splash');

            if(splash) {
              splash.remove();
            }
            resolve()
          }
        }, 500)
      })
    })
  ]
};
