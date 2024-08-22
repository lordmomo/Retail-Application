import { Component, OnInit } from '@angular/core';
import { UserServices } from '../Services/user.services';
import { Person } from '../Entity/person';

@Component({
  selector: 'app-list-all-users',
  templateUrl: './list-all-users.component.html',
  styleUrls: ['./list-all-users.component.scss']
})
export class ListAllUsersComponent implements OnInit {

  userList :Person[] =[]
  tableHeaders : string[] = ['Customer Name','Address','Username','Password','Balance']
  constructor(private userService : UserServices) { }

  ngOnInit(): void {
    this.userService.getAllUserList().subscribe((response) =>{
      this.userList = response
      // console.log(this.userList)
    },
    (err)=>{
      console.log("Request failed with error: " , err)
    }
  )
  }

}
