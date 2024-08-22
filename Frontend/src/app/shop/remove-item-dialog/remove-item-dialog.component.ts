import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-remove-item-dialog',
  templateUrl: './remove-item-dialog.component.html',
  styleUrls: ['./remove-item-dialog.component.scss']
})
export class RemoveItemDialogComponent implements OnInit {


  constructor(
    private dialogRef: MatDialogRef<RemoveItemDialogComponent>  ) { }

  ngOnInit(): void {
  }


  onSubmitDeleteItem(){
    this.dialogRef.close(true);
  }

  onDeleteItemformCancel(){
    this.dialogRef.close(false);
  }
}
