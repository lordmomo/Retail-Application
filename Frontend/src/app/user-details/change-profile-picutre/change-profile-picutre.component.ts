import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserServices } from 'src/app/Services/user.services';

@Component({
  selector: 'app-change-profile-picutre',
  templateUrl: './change-profile-picutre.component.html',
  styleUrls: ['./change-profile-picutre.component.scss']
})
export class ChangeProfilePicutreComponent implements OnInit {



  username : string | null =''
  selectedFile !: File ;

  constructor(private userService : UserServices, private router : Router, private route : ActivatedRoute) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.username = params['username'];
    });
  }
  
  changeProfileImage = new FormGroup({
    picture : new FormControl('',Validators.required)
  });

  onFileSelected(event : any){
    this.selectedFile = event.target.files[0] as File;
  }

  onSubmit() {
    if (!this.selectedFile) {
      console.error('No file selected');
      return;
    }
    const formData = new FormData();
    formData.append('file', this.selectedFile, this.selectedFile.name);
  
    this.username = sessionStorage.getItem('username');
    if (this.username != null) {
      this.userService.changeUserProfilePicture(this.username, formData).subscribe({
            next: (res: any) => {
              console.log('Image details saved:', res);
              this.router.navigateByUrl(`users/${this.username}`)
              
            },
            error: (err: any) => {
              console.error('Failed to save image :', err);
            }
          });
    }
  }
}
