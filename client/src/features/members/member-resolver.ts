import { ResolveFn, Router } from '@angular/router';
import { Member } from '../../types/member';
import { inject } from '@angular/core';
import { MemberService } from '../../core/services/member-service';
import { EMPTY } from 'rxjs';

export const memberResolver: ResolveFn<Member> = (route, state) => {
  // inject the member service to access the member details
  const memberService = inject(MemberService);
  const router = inject(Router);
  const memberId = route.paramMap.get('id');

  if(! memberId) {
    router.navigateByUrl('/not-found');
    return EMPTY; // we use this to return an Empty observable
    // we use EMPTY when we want to complete an observable chain safely without sending any data or error
  }

  return memberService.getMember(memberId);
};
