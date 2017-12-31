import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CouponInsertUpdateComponent } from './coupon-insert-update.component';

describe('CategoryInsertUpdateComponent', () => {
  let component: CouponInsertUpdateComponent;
  let fixture: ComponentFixture<CouponInsertUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CouponInsertUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CouponInsertUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
