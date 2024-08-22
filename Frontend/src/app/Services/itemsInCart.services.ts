import { Injectable } from "@angular/core";
import { Item } from "../Entity/item";
import { SelectedItem } from "../Entity/selectedItem";

@Injectable({
    providedIn: 'root'
})
export class ItemsInCartService {

    cartItems: SelectedItem[] = []

    private readonly CART_KEY = 'userCart';

    constructor(

    ) {
        this.cartItems = this.getCartItemsFromSessionStorage();

    }


    getCartItems(): SelectedItem[] {
        return this.cartItems;

    }

    setCartItems(itemsList: SelectedItem[]) {
        this.cartItems = itemsList;
        this.saveCartItemsToSessionStorage();
    }

    removeItemFromCart(productId: number): void {
        this.cartItems = this.cartItems.filter(item => item.productId !== productId);
        this.saveCartItemsToSessionStorage();
    }


    clearCart(): void {
        sessionStorage.removeItem(this.CART_KEY);
        this.cartItems = [];
    }

    private getCartItemsFromSessionStorage(): SelectedItem[] {
        const cartItemsJson = sessionStorage.getItem(this.CART_KEY);
        return cartItemsJson ? JSON.parse(cartItemsJson) : [];
    }

    private saveCartItemsToSessionStorage(): void {
        const itemsJson = JSON.stringify(this.cartItems);
        sessionStorage.setItem(this.CART_KEY, itemsJson);
    }
}