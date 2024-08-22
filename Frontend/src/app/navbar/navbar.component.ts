import { Component, EventEmitter, HostListener, OnDestroy, OnInit, Output } from '@angular/core';
import { RegisterFormData } from '../Services/registerFormData.services';
import { ActivatedRoute, Router } from '@angular/router';
import { from, Subscription } from 'rxjs';
import { AdminGuardService } from '../Services/adminGuard.services';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit, OnDestroy {

  constructor(private formData: RegisterFormData, private route: ActivatedRoute,
    private router: Router, private adminGuardService: AdminGuardService) { }

  username: string | null = ''
  loggedIn: boolean = false;
  loggedInSubscription !: Subscription;
  isAdmin: boolean = false;
  showDrawer: boolean = false;
  @Output() drawerSelection = new EventEmitter<string>();


  ngOnInit(): void {
    this.loggedInSubscription = this.formData.getLoggedInObservable().subscribe((loggedIn: boolean) => {
      this.loggedIn = loggedIn;
      if (loggedIn) {
        this.checkAdminRole();
        this.username = this.getUsername();
      }
  });


  }

  getUsername() {
    return sessionStorage.getItem('username')
  }


  ngOnDestroy(): void {
    if (this.loggedInSubscription) {
      this.loggedInSubscription.unsubscribe();
    }
  }

  checkAdminRole() {
    if (this.adminGuardService.canActivate()) {
      this.isAdmin = true;
      return;
    }
    this.isAdmin = false;
  }

  toggleDrawer() {
    this.showDrawer = !this.showDrawer;

  }

  selectDrawerOption(option: string): void {
    this.drawerSelection.emit(option);
  }

  logout(): void {
    this.formData.setLoggedIn(false);
    sessionStorage.clear();
    this.router.navigateByUrl('login')
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: Event) {
    if (!this.showDrawer) {
      return;
    }
    const target = event.target as HTMLElement;
    if (!target.closest('.user-button') && !target.closest('.drawer')) {
      this.showDrawer = false;
    }
  }
  onChangeUserDetails() {
    this.username = this.getUsername();
    this.router.navigateByUrl(`/users/${this.username}/edit-user-details`)
    this.showDrawer = false;
  }

  onChangePassword() {
    this.username = this.getUsername();
    this.router.navigateByUrl(`/users/${this.username}/change-password`)
    this.showDrawer = false;
  }
  onChangeUserProfilePicture() {
    this.username = this.getUsername();
    this.showDrawer = false;
    this.router.navigateByUrl(`/users/${this.username}/change-profile-picutre`)
  }
  onShowUserDetails() {
    this.username = this.getUsername();
    this.showDrawer = false;
    this.router.navigateByUrl(`/users/${this.username}`)
  }

}
