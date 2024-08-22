import { Component, OnInit } from '@angular/core';
import { UserServices } from '../Services/user.services';
import { Sale } from '../Entity/sale';
import { MatDialog } from '@angular/material/dialog';
import { TransactionDetailsDialogComponent } from './transaction-details-dialog/transaction-details-dialog.component';

@Component({
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.scss']
})
export class TransactionComponent implements OnInit {

  displayedColumns: string[] = ['transactionId', 'dateOfSale', 'productId'];
  // displayedColumns: string[] = ['transactionId', 'dateOfSale'];

  transactionHistory : Sale[] =[]
  groupedTransactions: Map<number, Sale[]> = new Map<number, Sale[]>(); 
  
  constructor(private userService : UserServices,private dialog : MatDialog
  ) { }

  ngOnInit(): void {
    this.getUserTransactionHistory();
  }

  getUserTransactionHistory(){
    const username = sessionStorage.getItem('username')
    if(username != null){
      this.userService.getUserTransactionHistory(username).subscribe(
        {
          next : (value) =>{
            this.transactionHistory = value
            // console.log(this.transactionHistory)
            // this.groupTransactionById();
          },
          error : (err) =>{
            console.log(err)
          }
        }
      )
    }
  }

  groupTransactionById(){
    this.groupedTransactions.clear();
    this.transactionHistory.forEach(
      transaction =>{
        
        if(!this.groupedTransactions.has(transaction.TransactionId)){
          this.groupedTransactions.set(transaction.TransactionId,[transaction])

        }
        else{
          this.groupedTransactions.get(transaction.TransactionId)?.push(transaction);

        }
      }
    );
    console.log('Grouped Transactions: ',this.groupedTransactions)
  }



  // openTransactionDetailsDialog(transaction: Sale): void {
  //   const transactionsToShow = this.transactionHistory.filter(sale => sale.TransactionId === transaction.TransactionId);

  //   this.dialog.open(TransactionDetailsDialogComponent, {
  //     width: '400px',
  //     data: {transactionsToShow}
  //   });
  // }

  
}
