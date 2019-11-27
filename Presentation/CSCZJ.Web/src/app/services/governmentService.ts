import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders,HttpParams  } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { ConfigService } from "./configService";
import { LogService } from "./logService";

import { TableParams } from "../viewModels/common/TableOption";
import { ListResponse } from "../viewModels/Response/ListResponse";

import { GovernmentSelectList } from '../viewModels/Governments/government';


const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class GovernmentService{
    private apiUrl="";
    

    constructor( private http: HttpClient,
        private logService:LogService,
        private configService:ConfigService){ 
          this.apiUrl+=configService.getApiUrl()+"Systemmanage/Governments";
        }

    nameValidate(name:string):Observable<boolean>{
      const url = `${this.apiUrl}/Unique/${name}`;
      return this.http.get<boolean>(url).pipe(
        tap(_ => this.log(`fetched property id=${name}`)),
        catchError(this.handleError<boolean>(`getProperty id=${name}`))
      );    
    }

    autocompleteByName(name:string):Observable<ListResponse>{
      const url = `${this.apiUrl}/Autocomplete/${name}`;
      return this.http.get<ListResponse>(url).pipe(
        tap(_ => this.log(`fetched GovernmentSelectList`)),
        catchError(this.handleError<ListResponse>(`GovernmentSelectList`))
      );    
    }

    

    //获取单个资产
    // getPropertyById(id:number):Observable<Property>{
    //   const url = `${this.apiUrl}/${id}`;
    //   return this.http.get<Property>(url).pipe(
    //     tap(_ => this.log(`fetched property id=${id}`)),
    //     catchError(this.handleError<Property>(`getProperty id=${id}`))
    //   );    
    // }









  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return Observable.of(result as T);
    };
  }  

  /** Log a GovernmentService message with the MessageService */
  private log(message: string) {
    this.logService.add('GovernmentService: ' + message);
  }  
}