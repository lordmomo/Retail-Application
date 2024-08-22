import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RegisterFormData } from '../Services/registerFormData.services';
import { CookieService } from 'ngx-cookie-service';
import { HttpErrorResponse } from '@angular/common/http';
@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent implements OnInit {

  loginValid : boolean =false;
  username : string = ''

  returnUrl : string = '';
  hidePassword: boolean = true;

  errorMessage : string =''
  constructor(private router :Router,
    private registerFormData : RegisterFormData, private route : ActivatedRoute
    ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      // this.username = params['username'];
      this.returnUrl = params['returnUrl'] || '/';
      
    })

  } 

  loginForm = new FormGroup({
    username : new FormControl('',[Validators.required]),
    password : new FormControl('',[Validators.required,Validators.minLength(8)])
  })

  togglePasswordVisibility() {
    this.hidePassword = !this.hidePassword;
  }
  
  onSubmit(){
    console.log(this.loginForm)
    if(this.loginForm.valid){
      this.loginValid = true;
      
      this.registerFormData.sendLoginFormData(this.loginForm.value).subscribe({
        next : (res) => {
          // console.log(res)
          this.registerFormData.setRole(res.role);
          this.registerFormData.setLoggedIn(true)
          this.username = res.username;
          sessionStorage.setItem('role',res.role)
          sessionStorage.setItem('username',res.username)
          sessionStorage.setItem('token',res.token)


          // console.log(this.returnUrl);
          if (this.returnUrl === '/login' || this.returnUrl === '/') {
            this.router.navigateByUrl(`/users/${this.username}`);
          } else {
            this.router.navigateByUrl(this.returnUrl);
          }          

        },
        error: (err : HttpErrorResponse) =>{

          if (err.status === 400 || err.status === 404){
            this.errorMessage = err.error.message
          }
          else if(err.error instanceof ErrorEvent)
            console.log('An error occured:',err.error.message)
          
          else{
            console.error(`Backend returned code ${err.status}, body was: `, err.error);
          }
        }
      })
    }
    else{
      this.loginValid= false
    }
  }

}
