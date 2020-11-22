import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyOffComponent } from './property-off.component';

describe('PropertyOffComponent', () => {
  let component: PropertyOffComponent;
  let fixture: ComponentFixture<PropertyOffComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PropertyOffComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyOffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
