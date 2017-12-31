import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryitemComponent } from './categoryitem.component';

describe('CategoryitemComponent', () => {
  let component: CategoryitemComponent;
  let fixture: ComponentFixture<CategoryitemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CategoryitemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CategoryitemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
