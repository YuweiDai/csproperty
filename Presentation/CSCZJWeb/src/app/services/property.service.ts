import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { LogService } from './log.service';
import { ConfigService } from './config.service';
import { tap, catchError } from 'rxjs/operators';
import { PropertyCreateModel, Property, ExportModel, PropertyRentModel, PropertyOffModel, SimplePropertyModel, SameIdPropertyModel } from '../viewModels/properties/property';
import { PropertyNameList } from '../viewModels/properties/propertyName';
import { TableParams } from '../viewModels/common/TableOption';
import { property_map } from '../viewModels/properties/property_map';
import { HighSearchProperty } from '../viewModels/properties/highSearchModel';
import { ListResponse } from '../viewModels/response/ListResponse';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class PropertyService {
  private apiUrl = "";


  constructor(private http: HttpClient,
    private logService: LogService,
    private configService: ConfigService) {
    this.apiUrl += configService.getApiUrl() + "properties";
  }

  nameValidate(name: string): Observable<boolean> {
    const url = `${this.apiUrl}/Unique/${name}`;

    return this.http.get<boolean>(url).pipe(
      tap(_ => this.log(`fetched property id=${name}`)),
      catchError(this.handleError<boolean>(`getProperty id=${name}`))
    );
  }

  createProperty(propertyCreateModel: PropertyCreateModel): Observable<Property> {
    const url = `${this.apiUrl}/create`;
    return this.http.post<Property>(url, propertyCreateModel, httpOptions).pipe(
      tap((property: Property) => this.log(`added property w/ id=${property.id}`)),
      catchError(this.handleError<Property>(`addProperty `))
    );
  }
  //导出资产
  exportProperty(exportModel: ExportModel) {

    const url = `${this.apiUrl}/Export`;


    return this.http.post(url, exportModel, { responseType: "arraybuffer" });


  }
  //新增出租
  createPropertyRentRecord(propertyRentModel: PropertyRentModel): Observable<PropertyRentModel> {
    const url = `${this.apiUrl}/Rent`;
    return this.http.post<PropertyRentModel>(url, propertyRentModel, httpOptions).pipe(
      tap((property: PropertyRentModel) => this.log(`added property Rent w/ id=${property.id}`)),
      catchError(this.handleError<PropertyRentModel>(`addProperty Rent`))
    );
  }
  //获取单个出租
  getRentById(id: number): Observable<PropertyRentModel> {
    const url = `${this.apiUrl}/Rent/${id}`;
    return this.http.get<PropertyRentModel>(url).pipe(
      tap(_ => this.log(`get Rent property id=${id}`)),
      catchError(this.handleError<PropertyRentModel>(`getRentById id=${id}`))
    );
  }
  //出租变更
  updatedRent(propertyRentModel: PropertyRentModel): Observable<PropertyRentModel> {
    const url = `${this.apiUrl}/UpdateRent/${propertyRentModel.id}`;
    return this.http.put<PropertyRentModel>(url, propertyRentModel, httpOptions).pipe(
      tap((property: PropertyRentModel) => this.log(`updated property w/ id=${propertyRentModel.id}`)),
      catchError(this.handleError<PropertyRentModel>(`update property `))
    );
  }


  createPropertyOffRecord(propertyOffModel: PropertyOffModel): Observable<PropertyOffModel> {
    const url = `${this.apiUrl}/Off`;
    return this.http.post<PropertyOffModel>(url, propertyOffModel, httpOptions).pipe(
      tap((property: PropertyOffModel) => this.log(`added property off w/ id=${property.id}`)),
      catchError(this.handleError<PropertyOffModel>(`addProperty off`))
    );
  }

  //获取单个资产
  getUpdatedPropertyById(id: number): Observable<PropertyCreateModel> {
    const url = `${this.apiUrl}/Update/${id}`;
    return this.http.get<PropertyCreateModel>(url).pipe(
      tap(_ => this.log(`get Update property id=${id}`)),
      catchError(this.handleError<PropertyCreateModel>(`getUpdateProperty id=${id}`))
    );
  }

  //资产变更
  updatedProperty(propertyCreateModel: PropertyCreateModel): Observable<Property> {
    const url = `${this.apiUrl}/${propertyCreateModel.id}`;
    return this.http.put<Property>(url, propertyCreateModel, httpOptions).pipe(
      tap((property: Property) => this.log(`updated property w/ id=${property.id}`)),
      catchError(this.handleError<Property>(`update property `))
    );
  }

  //获取单个资产
  getPropertyById(id: number, simple: boolean): Observable<Property> {
    var url = `${this.apiUrl}/${id}`;

    if (simple) url += '?simple=true';

    return this.http.get<Property>(url).pipe(
      tap(_ => this.log(`get property id=${id}`)),
      catchError(this.handleError<Property>(`getProperty id=${id}`))
    );
  }

  //获取可以处置的资产
  getProcessPropertyByName(name: string): Observable<SimplePropertyModel[]> {
    const url = `${this.apiUrl}/PropertyProcess/${name}`;
    return this.http.get<SimplePropertyModel[]>(url).pipe(
      tap(_ => this.log(`fetched process properties`)),
      catchError(this.handleError<SimplePropertyModel[]>(`fetched process properties`))
    );
  }


  //通过名称地址搜索资产
  getPropertiesBySearch(search: string): Observable<PropertyNameList[]> {

    return this.http.get<PropertyNameList[]>(this.apiUrl + "/search?search=" + search)
      .pipe(
        tap(response => { }),
        catchError(this.handleError('getPropertiesBySearch', []))
      );
  }

  //获取资产列表
  getAllProperties(params: TableParams): Observable<ListResponse> {
    let url = this.apiUrl;

    let requestParams = new URLSearchParams();
    if (!(params.query == "" || params.query == null || params.query == undefined)) requestParams.append('query', params.query)
    requestParams.append('pageIndex', params.pageIndex.toString())
    requestParams.append('pageSize', params.pageSize.toString())
    requestParams.append('showHidden', "true")
    if (!(params.sort == "" || params.sort == null || params.sort == undefined)) requestParams.append('sort', params.sort)
    requestParams.append('time', new Date().getTime().toString());

    return this.http.get<ListResponse>(url + "?" + requestParams.toString())
      .pipe(
        tap(response => { }),
        catchError(this.handleError('getAllProperties', {}))
      );
  }


  getAllPropertiesInMap(): Observable<property_map[]> {
    return this.http.get<property_map[]>(this.apiUrl + "/geo/bigdata")
      .pipe(
        tap(response => { }),
        catchError(this.handleError('getAllPropertiesInMap', []))
      );
  }

  //获取高级搜索资产
  getHighSearchProperties(highSearch: HighSearchProperty): Observable<property_map[]> {
    return this.http.post<property_map[]>(this.apiUrl + "/highSearch", highSearch)
      .pipe(
        tap(response => { }),
        catchError(this.handleError('gethighSearchPropertiesInMap', []))
      );

  }


  getPropertiesBySameNumberId(numberId: string, typeId: string, id: number): Observable<SameIdPropertyModel[]> {

    let url = this.apiUrl + "/samenumber";

    let requestParams = new URLSearchParams();
    requestParams.append('numberId', numberId);
    requestParams.append('typeId', typeId);
    requestParams.append('id', id.toString());

    return this.http.get<SameIdPropertyModel[]>(url + "?" + requestParams.toString())
      .pipe(
        tap(response => { }),
        catchError(this.handleError('gethighSearchPropertiesInMap', []))
      );
  }

  //获取出租列表



  getUsers(pageIndex: number = 1, pageSize: number = 10, sortField: string, sortOrder: string, tabKey: string): Observable<ListResponse> {
    let randomUserUrl = this.apiUrl + "/rentlist";

    let params = new HttpParams()
      .append('page', `${pageIndex}`)
      .append('results', `${pageSize}`)
      .append('sortField', sortField)
      .append('sortOrder', sortOrder)
      .append('tabKey', tabKey)
      ;

    return this.http.get<ListResponse>(`${randomUserUrl}`, {
      params
    });
  }


  getPatrols(pageIndex: number = 1, pageSize: number = 10, sortField: string, sortOrder: string, tabKey: string): Observable<ListResponse> {
    let randomUserUrl = this.apiUrl + "/patrollist";

    let params = new HttpParams()
      .append('page', `${pageIndex}`)
      .append('results', `${pageSize}`)
      .append('sortField', sortField)
      .append('sortOrder', sortOrder)
      .append('tabKey', tabKey)
      ;

    return this.http.get<ListResponse>(`${randomUserUrl}`, {
      params
    });
  }


  //导出出租信息
  exportToExl(monthlist: string[]) {
    const url = `${this.apiUrl}/ExportRents`;

    return this.http.post(url, monthlist, { responseType: "arraybuffer" });
  }


  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  /** Log a PropertyService message with the MessageService */
  private log(message: string) {
    this.logService.add('PropertyService: ' + message);
  }
}
