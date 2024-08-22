import { Routes } from "@angular/router";
import { LoginFormComponent } from "./login-form/login-form.component";
import { HomeComponent } from "./home/home.component";
import { RegisterFormComponent } from "./register-form/register-form.component";
import { ListAllUsersComponent } from "./list-all-users/list-all-users.component";
import { UserDetailsComponent } from "./user-details/user-details.component";
import { ShopComponent } from "./shop/shop.component";
import { CartItemsComponent } from "./cart-items/cart-items.component";
import { AuthGuardService } from "./Services/authGuard.services";
import { BillComponent } from "./bill/bill.component";
import { RolesComponent } from "./roles/roles.component";
import { CreateRoleComponent } from "./roles/create-role/create-role.component";
import { EditRoleComponent } from "./roles/edit-role/edit-role.component";
import { ViewAllRolesComponent } from "./roles/view-all-roles/view-all-roles.component";
import { AssignRolesComponent } from "./roles/assign-roles/assign-roles.component";
import { TransactionComponent } from "./transaction/transaction.component";
import { AdminGuardService } from "./Services/adminGuard.services";
import { ChangePasswordComponent } from "./user-details/change-password/change-password.component";
import { EditUserDetailsComponent } from "./user-details/edit-user-details/edit-user-details.component";
import { ChangeProfilePicutreComponent } from "./user-details/change-profile-picutre/change-profile-picutre.component";

export const routes :  Routes = [
    {path:'',redirectTo:'/login',pathMatch:'full'},
    {path:'home',component:HomeComponent},
    {path:'shop',component:ShopComponent,
        canActivate : [AuthGuardService],
        children:[
            {path:'cart',component:CartItemsComponent
                ,canActivate : [AuthGuardService]
            }
        ]
    },
    {path:'',
        children:[
        {path:'register',component:RegisterFormComponent},
        {path:'login',component:LoginFormComponent}
    ]},
    {path:'list-all-users',component:ListAllUsersComponent, 
        canActivate : [AuthGuardService]
    },
    {path:'users/:username',component:UserDetailsComponent, 
       canActivate : [AuthGuardService],
       children:[
        {path:'change-password',component:ChangePasswordComponent},
        {path:'edit-user-details',component:EditUserDetailsComponent},
        {path:'change-profile-picutre',component:ChangeProfilePicutreComponent},
       ]
    },
    {path:'bill',component:BillComponent,
        canActivate : [AuthGuardService]
        },
    {path:'roles',component:RolesComponent,
        canActivate : [AuthGuardService,AdminGuardService],
        // canActivateChild : [AdminGuardService],
        children : [
            {path:'create-role',component:CreateRoleComponent},
            // {path:'edit-role',component:EditRoleComponent},
            {path:'assign-role',component:AssignRolesComponent},
            {path:'view-all-roles',component:ViewAllRolesComponent},
        ]
    },
    {path:'transaction',component:TransactionComponent,canActivate:[AuthGuardService]},
    { path: '**', redirectTo: '/' } 
]