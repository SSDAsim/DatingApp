import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, identity, of, tap } from 'rxjs';
import { environment } from '../../environments/environment';

const cache = new Map<string, HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  const generateCacheKey = (url: string, params: HttpParams) : string => {
    const paramString = params.keys().map(key => `${key}=${params.get(key)}`).join('&');
    return paramString ? `${url}?${paramString}` : url;
  }

  // we might do not need cache for every http request so we have to invaildate cache for certain http request
  const invalidateCache = (urlPattern: string) => {
    for (const key of cache.keys()){
      if (key.includes(urlPattern)){
        cache.delete(key);
        console.log(`Cache invalidated for: ${key}`);
      }
    }
  };

  const cacheKey = generateCacheKey(req.url, req.params);

  if(req.method.includes('POST') && req.url.includes('/likes')) {
    invalidateCache('/likes');
  }

   if(req.method.includes('POST') && req.url.includes('/messages')) {
    invalidateCache('/messages');
  }


  // using the following method, we would not have to hit the get api again and again for response. we are going to get the response from the cache
  if(req.method === 'GET'){
    const cachedResponse = cache.get(cacheKey);
    if(cachedResponse) {
      return of(cachedResponse);
    }
  }

  busyService.busy();

  return next(req).pipe(
    (environment.production ? identity : delay(500)),
    tap(response => {
      cache.set(cacheKey, response)
    }),
    finalize(() => {
      busyService.idle() 
    })
  );
};

// register this interceptor in app.config.ts