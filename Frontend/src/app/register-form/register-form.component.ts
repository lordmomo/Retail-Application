import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RegisterFormData } from '../Services/registerFormData.services';
import { Router } from '@angular/router';
import { SuccessResponse } from '../Entity/successResponse';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.scss']
})
export class RegisterFormComponent implements OnInit {

  defaulImage : string = 'defaultUserImage.jpg'
  hidePassword: boolean = true;

  constructor(
    private registerFormData : RegisterFormData, private router :Router  ){

  }

  ngOnInit(): void {
  }

  registerForm = new FormGroup({
    CustomerName : new FormControl('',[Validators.required,Validators.maxLength(20)]),
    Address : new FormControl(),
    Username : new FormControl('',[Validators.required,Validators.maxLength(15),Validators.minLength(8)]),
    Password : new FormControl('',[Validators.required,Validators.minLength(8)]),
    Balance :new FormControl('',[Validators.required,Validators.min(0)]),
    FilePath : new FormControl(this.defaulImage)
  }
  )

  togglePasswordVisibility() {
    this.hidePassword = !this.hidePassword;
  }
  onSubmitRegisterForm(): void {
    if (this.registerForm.valid) {
      try {
        this.registerFormData.sendRegisterFormData(this.registerForm.value).subscribe({
          next: (response : SuccessResponse) => {
            if(response.success)
              {
                console.log('Registration successful:', response.message);
                this.registerForm.reset();
              }
              else{
                console.log('Registration failed:', response.message);
                console.log('Username already exists')
              }
           
          },
          error: (error) => {
            try {
              console.error('Error during registration:', error);
            } catch (innerError) {
              console.error('Error handling registration error:', innerError);
            }
          },
          complete : ()=>{
            this.router.navigateByUrl('login');
          }
        });
      } catch (error) {
        console.error('Error setting up registration subscription:', error);
      }
    }
  }

}
