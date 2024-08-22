import { Injectable } from "@angular/core";
import {HttpClient} from '@angular/common/http'
import { Person } from "../Entity/person";
import { BehaviorSubject, Observable } from "rxjs";
import { SuccessResponse } from "../Entity/successResponse";

@Injectable({
    providedIn :'root'
})
export class RegisterFormData{
    
    baseUrl : string = "http://localhost:5189/";
    loggedIn : boolean = false;

    role : string = ''; 

    private loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private isAdminSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(
        private httpClient : HttpClient
    ){

    }

    setRole(role : string){
      this.role = role
    }

    isAdmin():boolean{
      return this.role === 'Admin'
    }

    getLoggedInObservable(): Observable<boolean> {
        return this.loggedInSubject.asObservable();
      }
    
      setLoggedIn(value: boolean): void {
        this.loggedInSubject.next(value);
      }
    
      getLoggedIn(): boolean {
        return this.loggedInSubject.getValue();
      }

    
      getIsAdminObservable() : Observable<boolean>{
        return this.isAdminSubject.asObservable();
      }

    sendRegisterFormData(formData : any):Observable<SuccessResponse>{
        // console.log("Sending form data:",formData)
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}register`,formData)
    }

    sendLoginFormData(loginData:Person):Observable<any>{
        // console.log("Sending form data:",loginData)
        return this.httpClient.post<any>(`${this.baseUrl}login`,loginData)
    }

}