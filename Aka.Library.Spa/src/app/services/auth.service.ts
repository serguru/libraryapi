import { environment } from './../../environments/environment';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Member } from '../members/interfaces/member';
import { tap } from 'rxjs/operators';

@Injectable()
export class AuthService {

  apiUrl: string;
  
  //  TR2
  private _isAuthenticated: boolean = false;
  get isAuthenticated(): boolean {
    return this._isAuthenticated;
  }
  set isAuthenticated(value: boolean) {
    if (this.isAuthenticated === value) {
      return;
    }
    this._isAuthenticated = value;
    this.loggedIn.next(this.isAuthenticated);
  }


  currentMember: Member = null;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}${environment.apiPath}/members`;
    this.isAuthenticated = false;
  }

  private loggedIn = new BehaviorSubject<boolean>(false);
  
  get isLoggedIn() {
    return this.loggedIn.asObservable();
  }

  login(memberId: number): Observable<Member> {
    return this.http.get<Member>(`${this.apiUrl}/${memberId}`)
      .pipe(
        tap(res => {
          this.isAuthenticated = res !== null;
          this.currentMember = res;
        })
      );
  }

  logout(): void {
    this.isAuthenticated = false;
    this.currentMember = null;
  }

}
