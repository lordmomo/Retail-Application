import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserServices } from '../Services/user.services';

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent implements OnInit {

  profileImage: string | ArrayBuffer | null = null; 
  username : string = '';
  user : any ;
  constructor(private route : ActivatedRoute, private userService : UserServices) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.username = params['username'];
      this.fetchUserData(this.username)
    })
  }

  fetchUserData(username : string):void{
    this.userService.getUserDetails(username).subscribe({
      next : (userData:any) =>{
        this.user = userData
      } ,
      error : (err : any)=>{
        console.log('error fetching data: ',err)
      }
    });

    this.userService.getProfileImage(this.username).subscribe({
      next: (response: Blob) => {
        const reader = new FileReader();
        
        reader.readAsDataURL(response);

        reader.onload = () => {
          this.profileImage = reader.result;
        };
      },
      error: (error) => {
        console.error('Error fetching profile image:', error);
      }
    });
  }


}
