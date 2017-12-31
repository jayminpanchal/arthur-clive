import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryInsertUpdateComponent } from './category-insert-update.component';

describe('CategoryInsertUpdateComponent', () => {
  let component: CategoryInsertUpdateComponent;
  let fixture: ComponentFixture<CategoryInsertUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CategoryInsertUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CategoryInsertUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
