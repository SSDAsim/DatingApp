import { Location } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-not-found',
  imports: [],
  templateUrl: './not-found.html',
  styleUrl: './not-found.css',
})
export class NotFound {
  private location = inject(Location);

  // send user back to the previous page
  goBack() {
    this.location.back();
  }
}
