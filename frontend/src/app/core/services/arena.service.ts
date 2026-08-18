import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Arena, ArenaCreate, ArenaUpdate } from '../models/arena.model';

@Injectable({ providedIn: 'root' })
export class ArenaService {
  private baseUrl = `${environment.apiUrl}/Arena`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Arena[]> {
    return this.http.get<Arena[]>(this.baseUrl);
  }

  getById(id: string): Observable<Arena> {
    return this.http.get<Arena>(`${this.baseUrl}/${id}`);
  }

  create(dto: ArenaCreate): Observable<Arena> {
    return this.http.post<Arena>(this.baseUrl, dto);
  }

  update(id: string, dto: ArenaUpdate): Observable<Arena> {
    return this.http.put<Arena>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
