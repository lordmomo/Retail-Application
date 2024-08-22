import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Route, Router } from '@angular/router';
import { RoleResponse } from 'src/app/Entity/roleResponse';
import { RolesService } from 'src/app/Services/role.services';

@Component({
  selector: 'app-view-all-roles',
  templateUrl: './view-all-roles.component.html',
  styleUrls: ['./view-all-roles.component.scss']
})
export class ViewAllRolesComponent implements OnInit {

  roleList : RoleResponse[] =[]
  constructor(private roleService: RolesService, private router : Router) { }

  tableHeaders = ["Id","Name","Normalized Name","Actions"]

  editRoleBtn : boolean = false;
  roleToEdit !: RoleResponse ;

  editRoleForm = new FormGroup({
    roleName : new FormControl('',Validators.required)
  });

  ngOnInit(): void {
    this.roleService.viewAllRoles().subscribe(
      {
        next : (value)=>{
            this.roleList = value
            // console.log(this.roleList)
        },
        error : (err)=>{
            console.log(err)
        }
      }
    )
  }

  onEditRole(role : RoleResponse){
    this.editRoleBtn = true;
    this.roleToEdit = role;

    
  }

  onSubmitEditRoleForm(){
    if(this.editRoleForm.valid){
      this.roleToEdit.name = this.editRoleForm.get('roleName')?.value
      this.roleToEdit.normalizedName = this.roleToEdit.name.toUpperCase()
      this.roleService.editRole(this.roleToEdit).subscribe({
        next : (res) =>{
          console.log(res)
        },
        error : (err) =>{
          console.log(err)
        },
        complete : () =>{
          this.router.navigateByUrl("/roles/view-all-roles")
        }
      })
    }
    
  }
  onDeleteRole(roleId : string){
    this.roleService.deleteRole(roleId).subscribe({
      next : (res) =>{
        console.log(res)
        this.roleList = this.roleList.filter(role => role.id !== roleId);

      },
      error : (err) =>{
        console.log(err)
      },
      complete : () =>{
        this.router.navigateByUrl("/roles/view-all-roles")
      }
    })
  }




}
