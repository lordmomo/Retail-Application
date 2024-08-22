import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Role } from "../Entity/role";
import { Observable } from "rxjs";
import { SuccessResponse } from "../Entity/successResponse";
import { RoleResponse } from "../Entity/roleResponse";
import { UserRoleAssignmentModel } from "../Entity/userRoleAssignmentModel";

@Injectable({
    providedIn: 'root'
})
export class RolesService{

    baseUrl : string = "http://localhost:5189/roles/"

    
    constructor(private httpClient : HttpClient){

    }

    createRole(role : string) : Observable<SuccessResponse>{
        const body = { 'roleName' : role}
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}create-role`,body)
    }

    editRole(role : RoleResponse) : Observable<SuccessResponse>{
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}edit-role`,role)
    }

    deleteRole(roleId : string) : Observable<SuccessResponse>{
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}delete-role/${roleId}`,{headers : new HttpHeaders({'Content-type':'application/json'})})
    }

    viewAllRoles() : Observable<RoleResponse[]>{
        return this.httpClient.get<RoleResponse[]>(`${this.baseUrl}get-all-roles`)
    }

    assignRoleToUser(model: UserRoleAssignmentModel): Observable<any> {
        return this.httpClient.post(`${this.baseUrl}assign-role`, model);
    }

  
}