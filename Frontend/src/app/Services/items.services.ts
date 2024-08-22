import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Item } from "../Entity/item";
import { SelectedItem } from "../Entity/selectedItem";
import { SuccessResponse } from "../Entity/successResponse";

@Injectable({
    providedIn: 'root'
})
export class ItemService{

    baseUrl : string = "http://localhost:5189/users/"

    constructor(private httpClient : HttpClient){

    }


    showAllItemsInShop(username : string) : Observable<Item[]>{
        return this.httpClient.get<Item[]>(`${this.baseUrl}${username}/shop/all-items-in-shop`)    
    }

    getSearchedItem(shopItem:string) : Observable<Item[]>{
        return this.httpClient.get<Item[]>(`${this.baseUrl}shop/item/${shopItem}`)
    }

    saveFavouriteItem(username:string,shopItem : SelectedItem){
        return this.httpClient.post<any>(`${this.baseUrl}${username}/favourite-item`,shopItem)
    }
    getFavouriteItem(username:string){
        return this.httpClient.get<any>(`${this.baseUrl}${username}/get-favourite-item`)
    }


    deduceBill(username : string, cartItems : SelectedItem[], totalAmount : number):Observable<SuccessResponse>{
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}${username}/shop/deduct-balance`,{username,cartItems,totalAmount})
    }

    addItemInShop(username:string, item : Item):Observable<SuccessResponse>{
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}${username}/shop/add-items-in-shop`,item)
    }
    
    editItemInShop(username:string, item : Item):Observable<SuccessResponse>{
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}${username}/shop/update-items-in-shop`,item)
    }

    removeItemInShop(username:string, itemId : number):Observable<SuccessResponse>{
        return this.httpClient.post<SuccessResponse>(`${this.baseUrl}${username}/shop/remove-items-in-shop`,itemId)
    }
}