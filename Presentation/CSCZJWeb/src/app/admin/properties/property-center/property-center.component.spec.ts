import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyCenterComponent } from './property-center.component';

describe('PropertyCenterComponent', () => {
  let component: PropertyCenterComponent;
  let fixture: ComponentFixture<PropertyCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PropertyCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
