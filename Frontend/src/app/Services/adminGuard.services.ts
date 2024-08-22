
import { CanActivate, UrlTree } from "@angular/router";
import { RegisterFormData } from "./registerFormData.services";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn:'root'
})
export class AdminGuardService implements CanActivate{

    constructor(
        private registerForm : RegisterFormData
    ){

    }
    canActivate(): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree 
    {
        if(this.registerForm.isAdmin()){
            return true;
        }
        else{
            // alert("Access restricted");
            return false;
        }
    }
}