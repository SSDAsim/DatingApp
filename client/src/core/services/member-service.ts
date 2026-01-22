import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditableMember, Member, Photo } from '../../types/member';
import { map, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  editMode = signal(false);

  member = signal<Member | null>(null);

  getMembers(): Observable<Member[]> {
    return this.http.get<any>(this.baseUrl + 'members')
      .pipe(map(response => response.result));

      // we also need to provide the authentication header which we can pass as second parameter of the .get() 
  }

  getMember(id: string){
    return this.http.get<Member>(this.baseUrl + 'members/' + id).pipe(
      tap(member => {
        this.member.set(member); // set the member signal
      })
    );
  }

  getPhotos(id: string){
    return this.http.get<Photo[]>(this.baseUrl + 'members/' + id + '/photos');
  }

  updateMember(member: EditableMember) {
    return this.http.put(this.baseUrl + 'members', member);
  }

}
