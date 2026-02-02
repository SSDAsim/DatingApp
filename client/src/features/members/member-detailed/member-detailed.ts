import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { Member } from '../../../types/member';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { AccountService } from '../../../core/services/account-service';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-detailed',
  imports: [RouterLink, RouterLinkActive, RouterOutlet, AgePipe],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css',
})
export class MemberDetailed implements OnInit {
  protected memberService = inject(MemberService);
  private accountService = inject(AccountService);
  protected presenceService = inject(PresenceService);
  private route = inject(ActivatedRoute); // this will help us to get the query parameter
  private router = inject(Router);
  protected title = signal<string | undefined>('Profile'); 

  // compute if the current user is authenticated user 
  protected isCurrentUser = computed(() => {
    return this.accountService.currentUser()?.id === this.route.snapshot.paramMap.get('id');
  })

  ngOnInit(): void {
    this.title.set(this.route.firstChild?.snapshot?.title);

    // we need to extract the title whenever the route changes. 
    // there are a number of events in the router and we need to extract a specific event to get the title of the path
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe({
      next: () => {
        this.title.set(this.route.firstChild?.snapshot?.title);
      }
    })
  }
}
