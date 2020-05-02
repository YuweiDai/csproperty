import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyPatrollistComponent } from './property-patrollist.component';

describe('PropertyPatrollistComponent', () => {
  let component: PropertyPatrollistComponent;
  let fixture: ComponentFixture<PropertyPatrollistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PropertyPatrollistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyPatrollistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
