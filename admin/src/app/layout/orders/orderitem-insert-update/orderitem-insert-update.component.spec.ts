import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductInsertUpdateComponent } from './product-insert-update.component';

describe('ProductInsertUpdateComponent', () => {
  let component: ProductInsertUpdateComponent;
  let fixture: ComponentFixture<ProductInsertUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductInsertUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductInsertUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
