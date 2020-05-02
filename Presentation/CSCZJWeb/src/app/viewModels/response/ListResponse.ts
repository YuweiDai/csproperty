import { TablePageSize } from "../common/TableOption";

export class ListResponse{
    time?:number;
    paging?:TablePageSize;
    data?:any[];    
}