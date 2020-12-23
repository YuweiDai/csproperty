import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyFilelistComponent } from './property-filelist.component';

describe('PropertyFilelistComponent', () => {
  let component: PropertyFilelistComponent;
  let fixture: ComponentFixture<PropertyFilelistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PropertyFilelistComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyFilelistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
