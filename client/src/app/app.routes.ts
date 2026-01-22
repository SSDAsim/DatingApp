import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemberList } from '../features/members/member-list/member-list';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/gurads/auth-guard';
import { TestErrors } from '../features/test-errors/test-errors';
import { NotFound } from '../shared/error/not-found/not-found';
import { ServerError } from '../shared/error/server-error/server-error';
import { MemberProfile } from '../features/members/member-profile/member-profile';
import { MemberPhotos } from '../features/members/member-photos/member-photos';
import { MemberMessages } from '../features/members/member-messages/member-messages';
import { memberResolver } from '../features/members/member-resolver';
import { preventUnsavedChangesGuard } from '../core/gurads/prevent-unsaved-changes-guard';

export const routes: Routes = [
    {path: '', component: Home},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            {path: 'members', component: MemberList},
            {
                path: 'members/:id', 
                resolve: {member: memberResolver},
                runGuardsAndResolvers: 'always',
                component: MemberDetailed,
                children: [
                    {path: '', redirectTo: 'profile', pathMatch: 'full'},
                    {path: 'profile', component: MemberProfile, title: 'Profile', canDeactivate: [preventUnsavedChangesGuard]},
                    {path: 'photos', component: MemberPhotos, title: 'Photos'},
                    {path: 'messages', component: MemberMessages, title: 'Messages'},
                ]

                // when you visit 'members/id' it is going to redirect you to 'members/id/profile' and the title is going to help you show the page title on the browser page.
                // using the router link you can show different components in a single parent page.
                // pathMatch is going to check how much part of the url must match for a route to be considered a match.  
                // 'full' => entire url path must match exactly
                // 'prefix' => only start of the url must match
            },
            {path: 'lists', component: Lists},
            {path: 'messages', component: Messages},
        ]
    },
    {path: 'errors', component: TestErrors},
    {path: 'server-error', component: ServerError},
    {path: '**', component: NotFound} // this is wild-card route (when no route matches then this will be hit)
];
