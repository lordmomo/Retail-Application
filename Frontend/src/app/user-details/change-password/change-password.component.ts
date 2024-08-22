import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserServices } from 'src/app/Services/user.services';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

  username : string | null =''
  constructor(private userService : UserServices, private router : Router, private route : ActivatedRoute) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.username = params['username'];
    });
  }

  changeUserPasswordForm = new FormGroup({
    oldPassword : new FormControl('',[Validators.required,Validators.minLength(8)]),
    newPassword : new FormControl('',[Validators.required,Validators.minLength(8)]),
    confirmPassword : new FormControl('',[Validators.required,Validators.minLength(8)])
  })

  onSubmitChangeUserPasswordForm(){
    if (this.changeUserPasswordForm.valid) {
       this.username = sessionStorage.getItem('username')

      try {
        if(this.username != null){
          if(this.changeUserPasswordForm.get('newPassword')?.value === this.changeUserPasswordForm.get('confirmPassword')?.value){
            this.userService.changeUserPassword(this.username,this.changeUserPasswordForm.value).subscribe({
              next: (response) => {
                console.log(response);
                this.changeUserPasswordForm.reset();
              },
              error: (error) => {
                try {
                  console.error('Error during updating user details:', error);
                } catch (innerError) {
                  console.error('Error handling updating user details error:', innerError);
                }
              },
              complete : ()=>{
                this.router.navigateByUrl(`users/${this.username}`);
              }
            });
          }
          else{
            alert("passwords are different")
          }
          
        }
        
      } catch (error) {
        console.error('Error setting up registration subscription:', error);
      }
    }
  }

}
