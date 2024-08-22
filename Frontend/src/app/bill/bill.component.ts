import { Component, OnInit } from '@angular/core';
import { ItemsInCartService } from '../Services/itemsInCart.services';
import { Item } from '../Entity/item';
import { Router } from '@angular/router';
import { ItemService } from '../Services/items.services';
import { SelectedItem } from '../Entity/selectedItem';
import { SuccessResponse } from '../Entity/successResponse';

@Component({
  selector: 'app-bill',
  templateUrl: './bill.component.html',
  styleUrls: ['./bill.component.scss']
})
export class BillComponent implements OnInit {

  cartItems : SelectedItem[] = []
  username : string | null = ''
  totalBillAmount : number = 0;

  constructor(private data: ItemsInCartService,private router: Router,private itemService : ItemService, private itemsInCartService : ItemsInCartService) { }

  ngOnInit(): void {
    this.cartItems = this.data.getCartItems();
  }

  getTotalAmount(){
    this.totalBillAmount = this.cartItems.reduce((acc, item) => acc + (item.productPrice * item.selectedQuantity), 0);
    return this.totalBillAmount;
  }

  deduceBalance(){  
    this.username = sessionStorage.getItem('username')
    if(this.username === null){
      alert('user not logged in')
      this.router.navigate(['login']);
    }
    else{
      this.itemService.deduceBill(this.username,this.cartItems,this.totalBillAmount).subscribe({
        next : (res : SuccessResponse) =>{
          console.log(res.message)
          alert("Payment Successful")
          this.itemsInCartService.clearCart()
        },
        error : (err:string)=>{
          console.log(err)
        },
        complete : ()=>{
          this.router.navigateByUrl('/shop')
        }
      })
    }
  }
}
