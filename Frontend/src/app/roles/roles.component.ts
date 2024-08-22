import { Component, OnInit } from '@angular/core';
import { RolesService } from '../Services/role.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss']
})
export class RolesComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
