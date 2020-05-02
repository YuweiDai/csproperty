import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyRentlistComponent } from './property-rentlist.component';

describe('PropertyRentlistComponent', () => {
  let component: PropertyRentlistComponent;
  let fixture: ComponentFixture<PropertyRentlistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PropertyRentlistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyRentlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
