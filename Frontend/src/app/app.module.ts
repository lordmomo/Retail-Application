import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginFormComponent } from './login-form/login-form.component';
import {MatCardModule} from '@angular/material/card';
import {MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule} from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { HomeComponent } from './home/home.component';
import { RegisterFormComponent } from './register-form/register-form.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ListAllUsersComponent } from './list-all-users/list-all-users.component';
import {MatTableModule} from '@angular/material/table';
import { UserDetailsComponent } from './user-details/user-details.component';
// import {CookieModule} from 'ngx-cookie'
import { CookieService } from 'ngx-cookie-service';
import { ShopComponent } from './shop/shop.component';
import {MatGridListModule} from '@angular/material/grid-list';
import { FormsModule } from '@angular/forms';
import {MatIconModule} from '@angular/material/icon';
import { CartItemsComponent } from './cart-items/cart-items.component';
import { BillComponent } from './bill/bill.component';
import { UniversalAppInterceptor } from './Services/UniversalAppInterceptor';
import { RolesComponent } from './roles/roles.component';
import { CreateRoleComponent } from './roles/create-role/create-role.component';
import { EditRoleComponent } from './roles/edit-role/edit-role.component';
import { ViewAllRolesComponent } from './roles/view-all-roles/view-all-roles.component';
import { AssignRolesComponent } from './roles/assign-roles/assign-roles.component';
import {MatSelectModule} from '@angular/material/select';
import { TransactionComponent } from './transaction/transaction.component';
import { ChangePasswordComponent } from './user-details/change-password/change-password.component';
import { EditUserDetailsComponent } from './user-details/edit-user-details/edit-user-details.component';
import { ChangeProfilePicutreComponent } from './user-details/change-profile-picutre/change-profile-picutre.component';
import { EditItemDialogComponent } from './shop/edit-item-dialog/edit-item-dialog.component';
import { AddItemDialogComponent } from './shop/add-item-dialog/add-item-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { RemoveItemDialogComponent } from './shop/remove-item-dialog/remove-item-dialog.component';
import { TransactionDetailsDialogComponent } from './transaction/transaction-details-dialog/transaction-details-dialog.component';
@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    LoginFormComponent,
    HomeComponent,
    RegisterFormComponent,
    ListAllUsersComponent,
    UserDetailsComponent,
    ShopComponent,
    CartItemsComponent,
    BillComponent,
    RolesComponent,
    CreateRoleComponent,
    EditRoleComponent,
    ViewAllRolesComponent,
    AssignRolesComponent,
    TransactionComponent,
    ChangePasswordComponent,
    EditUserDetailsComponent,
    ChangeProfilePicutreComponent,
    EditItemDialogComponent,
    AddItemDialogComponent,
    RemoveItemDialogComponent,
    TransactionDetailsDialogComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatButtonModule,
    MatInputModule,
    MatTableModule,
    MatIconModule,
    MatGridListModule,
    MatSelectModule,
    MatDialogModule,
    // CookieModule.withOptions()
  ],
  providers: [{provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'outline'}},
    CookieService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UniversalAppInterceptor,
      multi: true
  }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
