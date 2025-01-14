import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { RegisterFormData } from "./registerFormData.services";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn:'root'
})
export class AuthGuardService implements CanActivate{

    constructor(
        private router: Router, private registerForm : RegisterFormData
    ){

    }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree 
    {
        if(this.registerForm.getLoggedIn()){
            return true;
        }
        else{
   
            this.router.navigate(['login'], { queryParams: { returnUrl: state.url } });
            return false;
        }
    }
}