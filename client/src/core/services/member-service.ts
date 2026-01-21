import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member, Photo } from '../../types/member';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getMembers(): Observable<Member[]> {
    return this.http.get<any>(this.baseUrl + 'members')
      .pipe(map(response => response.result));

      // we also need to provide the authentication header which we can pass as second parameter of the .get() 
  }

  getMember(id: string){
    return this.http.get<Member>(this.baseUrl + 'members/' + id);
  }

  getPhotos(id: string){
    return this.http.get<Photo[]>(this.baseUrl + 'members/' + id + '/photos');
  }

}
