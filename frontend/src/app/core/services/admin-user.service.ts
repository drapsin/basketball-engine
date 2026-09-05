import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateAdminRequest, UserSummary } from '../models/user-summary.model';

@Injectable({ providedIn: 'root' })
export class AdminUserService {
  private baseUrl = `${environment.apiUrl}/Auth`;

  constructor(private http: HttpClient) {}

  getUsers(): Observable<UserSummary[]> {
    return this.http.get<UserSummary[]>(`${this.baseUrl}/users`);
  }

  approve(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/users/${id}/approve`, {});
  }

  reject(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/users/${id}/reject`, {});
  }

  createAdmin(payload: CreateAdminRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/create-admin`, payload);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/users/${id}`);
  }
}
