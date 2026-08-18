import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Referee, RefereeCreate, RefereeUpdate } from '../models/referee.model';

@Injectable({ providedIn: 'root' })
export class RefereeService {
  private baseUrl = `${environment.apiUrl}/Referee`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Referee[]> {
    return this.http.get<Referee[]>(this.baseUrl);
  }

  getById(id: string): Observable<Referee> {
    return this.http.get<Referee>(`${this.baseUrl}/${id}`);
  }

  create(dto: RefereeCreate): Observable<Referee> {
    return this.http.post<Referee>(this.baseUrl, dto);
  }

  update(id: string, dto: RefereeUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
