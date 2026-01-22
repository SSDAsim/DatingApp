import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequestCount = signal(0);

  busy(){
    this.busyRequestCount.update(current => current + 1);
  }

  idle(){
    this.busyRequestCount.update(current => Math.max(0, current - 1)); // decrement but do not go below 0
  }

  // now we have to use this service somewhere. A good place would be an interceptor. Since we are using HttpClient, every request is going through interceptor
}
