import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CustomValidator } from 'src/app/utils/customValidator';

@Component({
  selector: 'app-add-item-dialog',
  templateUrl: './add-item-dialog.component.html',
  styleUrls: ['./add-item-dialog.component.scss']
})
export class AddItemDialogComponent implements OnInit {

  addItemsToShopForm = new FormGroup({
    productId: new FormControl('', [Validators.required,CustomValidator.number]),
    productName: new FormControl('', Validators.required),
    productDescription: new FormControl('', Validators.required),
    productPrice: new FormControl('', [Validators.required,CustomValidator.number]),
    productQty: new FormControl('', [Validators.required,CustomValidator.number]),
    productCategoryName: new FormControl('', Validators.required)
  });

  constructor(
    private dialogRef: MatDialogRef<AddItemDialogComponent>,

    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
  }

  onSubmitAddNewItem() {
    if (this.addItemsToShopForm.valid) {
      this.dialogRef.close(this.addItemsToShopForm.value);
    }
  }

  onAddNewItemFormCancel() {
    this.dialogRef.close();
  }

}
