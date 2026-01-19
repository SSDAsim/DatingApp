import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs';
import { ToastService } from '../services/toast-service';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const router = inject(Router);


  return next(req).pipe(
    catchError(err => {
      if(err) {
        switch (err.status) {
          case 400:
            // err will contain nested array error => error => errors and will contain different validation errors 
            if(err.error.errors){
              const modelStateErrors = [];
              for (const key in err.error.errors) {
                if (err.error.errors[key]){
                  modelStateErrors.push(err.error.errors[key]);
                }
              }
              throw modelStateErrors.flat(); // throw a flat array containing just strings
            } else {
              toast.error(err.error);
            }
            break;
          case 401:
            toast.error("Unauthorized");
            break;
          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            // we are going to provide error details to the 'server-error' component via 'Router State' which we are going to supply in Navigation Extras
            const navigationExtras: NavigationExtras  = {state: {error: err.error}};
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            toast.error('Something went wrong..');
            break;
        }
      }

      throw err
    })
  );

  // req => is the http request object
  // next => pass onto next elements
};
