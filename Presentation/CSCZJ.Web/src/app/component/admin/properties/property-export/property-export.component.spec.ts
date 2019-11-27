import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyExportComponent } from './property-export.component';

describe('PropertyExportComponent', () => {
  let component: PropertyExportComponent;
  let fixture: ComponentFixture<PropertyExportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PropertyExportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
