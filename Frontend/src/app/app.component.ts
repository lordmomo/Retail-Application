import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'shop';

  selectedOption: string | null = null;

  handleDrawerSelection(option: string): void {
    this.selectedOption = option;
  }
}
