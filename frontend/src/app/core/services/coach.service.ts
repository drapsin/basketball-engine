import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Coach, CoachCreate, CoachUpdate } from '../models/coach.model';

@Injectable({ providedIn: 'root' })
export class CoachService {
  private baseUrl = `${environment.apiUrl}/Coach`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Coach[]> {
    return this.http.get<Coach[]>(this.baseUrl);
  }

  getById(id: string): Observable<Coach> {
    return this.http.get<Coach>(`${this.baseUrl}/${id}`);
  }

  getByTeamId(teamId: string): Observable<Coach[]> {
    return this.http.get<Coach[]>(`${this.baseUrl}/by-team/${teamId}`);
  }

  create(dto: CoachCreate): Observable<Coach> {
    return this.http.post<Coach>(this.baseUrl, dto);
  }

  update(id: string, dto: CoachUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
