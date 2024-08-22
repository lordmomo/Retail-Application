import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ItemService } from '../Services/items.services';
import { Item } from '../Entity/item';
import { Router } from '@angular/router';
import { ItemsInCartService } from '../Services/itemsInCart.services';
import { SelectedItem } from '../Entity/selectedItem';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidator } from '../utils/customValidator';
import { AdminGuardService } from '../Services/adminGuard.services';
import { MatDialog } from '@angular/material/dialog';
import { AddItemDialogComponent } from './add-item-dialog/add-item-dialog.component';
import { EditItemDialogComponent } from './edit-item-dialog/edit-item-dialog.component';
import { RemoveItemDialogComponent } from './remove-item-dialog/remove-item-dialog.component';


@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {

  constructor(private itemService: ItemService, private router: Router,
     private itemsInCartService: ItemsInCartService,private adminGuardService : AdminGuardService,private dialog: MatDialog) { }

  showCart: boolean = false
  searchItem: string = ''

  favouriteItem !: SelectedItem ;
  showFavourites = false;
  
  itemToEdit !: Item ;

  shopItems: SelectedItem[] = []
  shownShopItems: SelectedItem[] = []
  sessionUsername: string | null = '';

  newItemToAdd !: Item ;

  isRoleAdmin : boolean = false;

  cartItems: SelectedItem[] = []

  @Output() searchTextEvent: EventEmitter<string> = new EventEmitter<string>();

  ngOnInit(): void {
    this.sessionUsername = sessionStorage.getItem('username');
    if (!this.sessionUsername) {
      alert("user not logged in")
    }
    else {
      this.fetchShopItems();
      this.checkForAdminRole();
      this.cartItems = this.itemsInCartService.getCartItems();
    }
  }

  fetchShopItems() {
    if(this.sessionUsername){
      this.itemService.showAllItemsInShop(this.sessionUsername).subscribe({
        next: (value: Item[]) => {
          this.shopItems = value.map(item => ({ ...item, selectedQuantity: 1 }));
          // console.log(" shop items",this.shopItems)

          this.shownShopItems = [...this.shopItems];
          // console.log("shown shop items",this.shownShopItems)
        },
        error: (err) => {
          console.log(err.error);
        }
      });
    }
    else{
      console.log("user not logged in.");
      this.router.navigateByUrl('login');
    }
   
  }

  checkForAdminRole(){
    if(this.adminGuardService.canActivate()){
      this.isRoleAdmin =true;
      return;
    }
    this.isRoleAdmin = false;
  }


  addToCart(product: SelectedItem) {
    this.cartItems = this.itemsInCartService.getCartItems()
    this.cartItems.push(product)
    this.itemsInCartService.setCartItems(this.cartItems);
    alert(`Item added to cart: ${product.productName}`);

  }

  saveFavourite(product: SelectedItem){
    this.favouriteItem = product
    const username = sessionStorage.getItem('username');
    if(username){
      this.itemService.saveFavouriteItem(username,this.favouriteItem).subscribe({
        next : (res)=>{
          alert(`${product.productName}: `+res)
          console.log(res);
        },
        error : (err)=>{
          console.log(err)
        }
      })
    }
    
  }

  getFavourite(){
    // this.favouriteItem = product
      const username = sessionStorage.getItem('username');
      if(username){
        this.itemService.getFavouriteItem(username).subscribe({
          next : (res :Item[])=>{  
            this.shopItems = res.map(item => ({ ...item, selectedQuantity: 1 }));
            this.shownShopItems = [...this.shopItems];
          },
          error : (err)=>{
            console.log(err)
          }
        })
      }
  }

  toggleView() {
    this.showFavourites = !this.showFavourites;
    if (this.showFavourites) {
      this.getFavourite();
    } else {
      this.fetchShopItems();
    }
  }


  onItemSearchButtonClicked() {
    if (!this.searchItem) {
      this.shownShopItems = [...this.shopItems]
    }
    else {
      this.searchItem = this.searchItem.charAt(0).toUpperCase() + this.searchItem.slice(1);
      this.itemService.getSearchedItem(this.searchItem).subscribe({
        next: (value) => {
          if (value === null || value.length === 0) {
            this.shownShopItems = []; 
            alert('product not found')
          }
          else{
            this.shownShopItems = value.map(item => ({ ...item, selectedQuantity: 1 }))
          }
        },
        error: (err) => {
          console.log('Error searching item:', err);

        }
      });
    }

  }

  onSearch(){
    this.onItemSearchButtonClicked();
  }

  search(event: Event) {
    this.searchItem = (<HTMLInputElement>event.target).value;
    // console.log(this.searchItem)
  }

  toogleCardItems() {
    this.showCart = !this.showCart
    if (this.showCart)
      this.router.navigateByUrl('shop/cart');
    else
      return
  }

  addItemInShop() {
    if(this.adminGuardService.canActivate()){
      const dialogRef = this.dialog.open(AddItemDialogComponent);

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          console.log('New Item:', result);

          this.onSubmitAddNewItem(result)
        }
      });
      return;
    }
  }

  onSubmitAddNewItem(result : Item){
      this.newItemToAdd = result;
      if(this.sessionUsername){
        this.itemService.addItemInShop(this.sessionUsername,this.newItemToAdd).subscribe(
          {
            next : (res) =>{
              console.log(res);
            },
            error : (err) =>{
              console.log(err);
            },
            complete :() =>{
              this.fetchShopItems();
              this.router.navigateByUrl("shop");
            }
          }
        )
      }
  }



  editItem(item : Item){

    if(this.adminGuardService.canActivate()){
      const dialogRef = this.dialog.open(EditItemDialogComponent, {
        data: { ...item }
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          console.log('Edited Item:', result);
          this.onSubmitEditItem(result);
        }
      });
      return;
    }

  }

  onSubmitEditItem(result : Item){
      this.itemToEdit = result;
      if(this.sessionUsername){
        this.itemService.editItemInShop(this.sessionUsername,this.itemToEdit).subscribe(
          {
            next : (res) =>{
              console.log(res);
            },
            error : (err) =>{
              console.log(err);
            },
            complete :() =>{
              this.fetchShopItems();
              this.router.navigateByUrl("shop");
            }
          }
        );
      }
  }



  deleteItem(itemId : number){
    if(this.adminGuardService.canActivate()){
      const dialogRef = this.dialog.open(RemoveItemDialogComponent);

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          console.log('New Item:', result);

          this.onSubmitDeleteItem(itemId)
        }
      });
      return;
    }
    

  }
  onSubmitDeleteItem(itemId : number){
    if(this.adminGuardService.canActivate() && this.sessionUsername){

      
      this.itemService.removeItemInShop(this.sessionUsername,itemId).subscribe({
        next : (res) =>{
          console.log(res);
        },
        error : (err) =>{
          console.log(err);
        },
        complete :() =>{
          this.fetchShopItems();
          this.router.navigateByUrl("shop");
        }
      });
    }
  }
}