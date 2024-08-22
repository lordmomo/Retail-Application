import { Injectable } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Injectable({
    providedIn : 'root'
})
export class CustomValidator{

    static number(control: AbstractControl): Validators | null {
        if (isNaN(control.value)) {
          return { notANumber: true };
        }
        return null;
      }
}
