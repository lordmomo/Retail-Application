import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Sale } from 'src/app/Entity/sale';

@Component({
  selector: 'app-transaction-details-dialog',
  templateUrl: './transaction-details-dialog.component.html',
  styleUrls: ['./transaction-details-dialog.component.scss']
})
export class TransactionDetailsDialogComponent implements OnInit {

  sale : Sale;
  
  constructor(@Inject(MAT_DIALOG_DATA)  data: Sale) {
    this.sale = data
  }

  ngOnInit(): void {
  }

}
