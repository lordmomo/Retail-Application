import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";
import { EditUserDetails } from "../Entity/editUserDetails";
import { ChangePasswordRequest } from "../Entity/changePasswordRequest";
import { SelectedItem } from "../Entity/selectedItem";

@Injectable({
    providedIn:"root"
})
export class UserServices{
    

    baseUrl : string = "http://localhost:5189/users/"
    
    constructor(
        private httpClient : HttpClient
    ){

    }

    
    getUserDetails(username: string) : Observable<any>{
      return this.httpClient.get<any>(`${this.baseUrl}${username}`)
    }

    getUserTransactionHistory(username : string){
      return this.httpClient.get<any>(`${this.baseUrl}${username}/transaction-history`)
    }

    
    changeUserDetails(username : string, body : EditUserDetails){
      return this.httpClient.post<any>(`${this.baseUrl}${username}/edit-details`,body)
    }
    
    changeUserPassword(username : string, body : ChangePasswordRequest){
      return this.httpClient.post<any>(`${this.baseUrl}${username}/change-password`,body)
    }

    changeUserProfilePicture(username:string,formData : FormData){
      return this.httpClient.post<any>(`${this.baseUrl}${username}/add-profile-image`, formData);
    }

    getProfileImage(username : string) {
      return this.httpClient.get(`${this.baseUrl}${username}/profile-image`, { responseType: 'blob' });
    }



    getAllUserList():Observable<any>{
        // console.log("getting data of all users:")
        return this.httpClient.get<any>(`${this.baseUrl}view-all-users`).pipe(
            catchError(this.handleError)
          );
    }

    private handleError(error: HttpErrorResponse) {
        console.error("An error occurred:", error);
        return throwError(
          "Something bad happened; please try again later."
        );
      }

    
}