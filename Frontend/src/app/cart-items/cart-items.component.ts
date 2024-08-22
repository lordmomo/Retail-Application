import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ItemsInCartService } from '../Services/itemsInCart.services';
import { Item } from '../Entity/item';
import { ItemService } from '../Services/items.services';
import { SelectedItem } from '../Entity/selectedItem';

@Component({
  selector: 'app-cart-items',
  templateUrl: './cart-items.component.html',
  styleUrls: ['./cart-items.component.scss']
})
export class CartItemsComponent implements OnInit {

  cartItems: SelectedItem[] = [];

  constructor( private itemsInCartService :ItemsInCartService, private router: Router) { }

  ngOnInit(): void {

    this.cartItems = this.itemsInCartService.getCartItems()
    // console.log(this.cartItems)
  }

  removeItem(productId:number){
    this.itemsInCartService.removeItemFromCart(productId);
    this.cartItems = this.itemsInCartService.getCartItems(); 
  }

  checkoutBill(){
    this.router.navigateByUrl('bill')
  } 

}
