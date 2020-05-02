import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { LogService } from './log.service';
import { ConfigService } from './config.service';
import { Observable, of } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { ListResponse } from '../viewModels/response/ListResponse';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class GovernmentService {
  private apiUrl = "";


  constructor(private http: HttpClient,
    private logService: LogService,
    private configService: ConfigService) {
    this.apiUrl += configService.getApiUrl() + "Systemmanage/Governments";
  }

  nameValidate(name: string): Observable<boolean> {
    const url = `${this.apiUrl}/Unique/${name}`;
    return this.http.get<boolean>(url).pipe(
      tap(_ => this.log(`fetched property id=${name}`)),
      catchError(this.handleError<boolean>(`getProperty id=${name}`))
    );
  }

  autocompleteByName(name: string): Observable<ListResponse> {
    const url = `${this.apiUrl}/Autocomplete/${name}`;
    return this.http.get<ListResponse>(url).pipe(
      tap(_ => this.log(`fetched GovernmentSelectList`)),
      catchError(this.handleError<ListResponse>(`GovernmentSelectList`))
    );
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

  /** Log a GovernmentService message with the MessageService */
  private log(message: string) {
    this.logService.add('GovernmentService: ' + message);
  }
}
