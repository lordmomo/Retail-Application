import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { RegisterFormData } from 'src/app/Services/registerFormData.services';
import { UserServices } from 'src/app/Services/user.services';

@Component({
  selector: 'app-edit-user-details',
  templateUrl: './edit-user-details.component.html',
  styleUrls: ['./edit-user-details.component.scss']
})
export class EditUserDetailsComponent implements OnInit {

  username : string = ''
  constructor(private userService : UserServices, private router : Router, private route :ActivatedRoute) { }
  

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.username = params['username'];
    })
  }

  changeUserDetailsForm = new FormGroup({
    CustomerName : new FormControl('',[Validators.required,Validators.maxLength(20)]),
    Address : new FormControl(),
  });

  onSubmitChangeUserDetailsForm(){
    if (this.changeUserDetailsForm.valid) {

      try {
        if(this.username){
          this.userService.changeUserDetails(this.username,this.changeUserDetailsForm.value).subscribe({
            next: (response) => {
              console.log(response);
              this.changeUserDetailsForm.reset();
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
        
      } catch (error) {
        console.error('Error setting update user details subscription:', error);
      }
    }
  }
  
}
