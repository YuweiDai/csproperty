import { Component, OnInit, ViewEncapsulation, Input, Output, EventEmitter, NgModule } from '@angular/core';
import { TableOption, TableParams } from 'src/app/viewModels/common/TableOption';


@Component({
  selector: 'app-ui-table',
  templateUrl: './ui-table.component.html',
  styleUrls: ['./ui-table.component.less'],
  encapsulation: ViewEncapsulation.None,
})
export class UiTableComponent implements OnInit {
  @Input() loading: boolean;
  @Input() dataset: any[];
  @Input() tableOption: TableOption;
  @Output() paramsChange: EventEmitter<TableParams> = new EventEmitter();
  q: string;
  sortString: string;

  pageSizes = [
    { value: '15', label: '15条/页' },
    { value: '50', label: '50条/页' },
    { value: '100', label: '100条/页' },
    { value: '200', label: '200条/页' },
    { value: '500', label: '500条/页' }
  ];

  constructor() { }

  ngOnInit() {
    this.paramsChange.emit(this.buildTableParams(true));
  }

  //生成表格查询参数
  buildTableParams(reset: boolean): TableParams {
    let tableParams: TableParams = {
      pageIndex: reset ? 1 : this.tableOption.pageSize.pageIndex,
      pageSize: this.tableOption.pageSize.pageSize,
      query: this.q == undefined ? "" : this.q,
      sort: this.sortString == undefined ? "" : this.sortString
    }

    console.log(tableParams);
    return tableParams;
  }

  //排序
  sort(sort: { key: string, value: string }): void {
    if (sort.value == "" || sort.value == null || sort.value == undefined) this.sortString = undefined;
    else this.sortString = sort.key + "," + sort.value + ";";
    this.paramsChange.emit(this.buildTableParams(true));
  }

  //搜索
  search(term: string): void {
    this.q = term;
    this.paramsChange.emit(this.buildTableParams(true));
  }

  //页码转跳
  pageIndexChange(): void {
    this.paramsChange.emit(this.buildTableParams(false));
  }

  //每页数变化
  pageSizeChange(): void {
    this.paramsChange.emit(this.buildTableParams(true));
  }
}
