import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { ToastService } from '../../core/services/toast-service';
import { themes } from '../theme';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit {
  public accountService = inject(AccountService);
  private router = inject(Router);
  private toastService = inject(ToastService);
  protected creds: any = {};
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes;

  // set the theme on the component load
  ngOnInit(): void {
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  }

  // save the selected theme in the localStorage
  handleSelectedTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);

    // close the dropdown once the selection is made
    const elem = document.activeElement as HTMLElement;
    if(elem) elem.blur();
  }

  login(){
    this.accountService.login(this.creds).subscribe({
      next: result => {
        this.router.navigateByUrl('/members');
        this.creds.password = '';
        this.toastService.success('Logged in Successfully');
      },
      error: error => {
        this.toastService.error(error.error);
      }
    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
