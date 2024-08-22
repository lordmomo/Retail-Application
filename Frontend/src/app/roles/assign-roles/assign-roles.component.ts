import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Person } from 'src/app/Entity/person';
import { Role } from 'src/app/Entity/role';
import { RoleResponse } from 'src/app/Entity/roleResponse';
import { UserRoleAssignmentModel } from 'src/app/Entity/userRoleAssignmentModel';
import { RolesService } from 'src/app/Services/role.services';
import { UserServices } from 'src/app/Services/user.services';

@Component({
  selector: 'app-assign-roles',
  templateUrl: './assign-roles.component.html',
  styleUrls: ['./assign-roles.component.scss']
})
export class AssignRolesComponent implements OnInit {

  selectedUsers = new FormControl('',Validators.required);
  selectedRole = new FormControl('',Validators.required);
  isSelected : boolean = true;

  users: Person[] = []

  roleList: RoleResponse[] = []

  constructor(private userService: UserServices, private roleService: RolesService) { }

  ngOnInit(): void {
    this.userService.getAllUserList().subscribe({
      next: (value) => {
        this.users = value
        // console.log(this.users)
      },
      error: (err) => {
        console.log(err)
      }
    });

    this.roleService.viewAllRoles().subscribe(
      {
        next: (value) => {
          this.roleList = value
          // console.log(this.roleList)
        },
        error: (err) => {
          console.log(err)
        }
      }
    );
  }


  assginRole() {
    const model: UserRoleAssignmentModel = {
      userIds: this.selectedUsers.value,
      roleId: this.selectedRole.value.id,
      isSelected: this.isSelected
    };

    // console.log(model)

    if(this.selectedRole.valid && this.selectedUsers.valid){
      this.roleService.assignRoleToUser(model).subscribe(
        {
         next : () => {
           console.log('Roles assigned successfully.');
         },
         error:(err) => {
           console.error('Error assigning roles:', err);
         }
        }
       );
    }
    else{
      alert("form field can't be empty")
    }
   
  }

  removeRole(){
    const model: UserRoleAssignmentModel = {
      userIds: this.selectedUsers.value,
      roleId: this.selectedRole.value.id,
      isSelected: false
    };

    // console.log(model)

    if(this.selectedRole.valid && this.selectedUsers.valid){
      this.roleService.assignRoleToUser(model).subscribe(
        {
         next : () => {
           console.log("User's role removed successfully.");
         },
         error:(err) => {
           console.error("Error while removing users's roles:", err);
         }
        }
       );
    }
    else{
      alert("form field can't be empty")
    }
  
  }
}
