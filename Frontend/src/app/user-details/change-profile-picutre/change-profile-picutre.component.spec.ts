import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeProfilePicutreComponent } from './change-profile-picutre.component';

describe('ChangeProfilePicutreComponent', () => {
  let component: ChangeProfilePicutreComponent;
  let fixture: ComponentFixture<ChangeProfilePicutreComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeProfilePicutreComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeProfilePicutreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
