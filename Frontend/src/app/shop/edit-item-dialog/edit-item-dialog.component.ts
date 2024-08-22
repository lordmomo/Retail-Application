import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CustomValidator } from 'src/app/utils/customValidator';

@Component({
  selector: 'app-edit-item-dialog',
  templateUrl: './edit-item-dialog.component.html',
  styleUrls: ['./edit-item-dialog.component.scss']
})
export class EditItemDialogComponent implements OnInit {

  editItemsOfShopForm = new FormGroup({
    productId: new FormControl('', [Validators.required,CustomValidator.number]),
    productName: new FormControl('', Validators.required),
    productDescription: new FormControl('', Validators.required),
    productPrice: new FormControl('', [Validators.required,CustomValidator.number]),
    productQty: new FormControl('', [Validators.required,CustomValidator.number]),
    productCategoryName: new FormControl('', Validators.required)
  });


  constructor(
    private dialogRef: MatDialogRef<EditItemDialogComponent>,

    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb : FormBuilder
  ) {

    this.editItemsOfShopForm = this.fb.group({
      productId: [data.productId, [Validators.required, CustomValidator.number]],
      productName: [data.productName, Validators.required],
      productDescription: [data.productDescription, Validators.required],
      productPrice: [data.productPrice, [Validators.required, CustomValidator.number]],
      productQty: [data.productQty, [Validators.required,CustomValidator.number]],
      productCategoryName: [data.productCategoryName, Validators.required]
    });

   }

  ngOnInit(): void {
  }

  onSubmitEditItem() {
    if (this.editItemsOfShopForm.valid) {
      this.dialogRef.close(this.editItemsOfShopForm.value);
    }
  }

  onEditItemFormCancel() {
    this.dialogRef.close();
  }

}
