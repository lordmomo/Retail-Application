import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RolesService } from 'src/app/Services/role.services';

@Component({
  selector: 'app-create-role',
  templateUrl: './create-role.component.html',
  styleUrls: ['./create-role.component.scss']
})
export class CreateRoleComponent implements OnInit {

  constructor(private roleService : RolesService) { }

  ngOnInit(): void {
  }

  roleForm = new FormGroup({
    roleName : new FormControl('',Validators.required)
  })

  onSubmitRoleForm(){
    if(this.roleForm.valid){

      console.log(this.roleForm.get('roleName')?.value)
      const roleName = this.roleForm.get('roleName')?.value
      this.roleService.createRole(roleName).subscribe({
        next : (value) =>{
          console.log(value)
        },
        error : (err)=>{
          console.log(err)

        }
      })
    }
  }
}
