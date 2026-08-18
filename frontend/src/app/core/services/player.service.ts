import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Player, PlayerCreate, PlayerUpdate } from '../models/player.model';

@Injectable({ providedIn: 'root' })
export class PlayerService {
  private baseUrl = `${environment.apiUrl}/Player`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl);
  }

  getById(id: string): Observable<Player> {
    return this.http.get<Player>(`${this.baseUrl}/${id}`);
  }

  getByTeamId(teamId: string): Observable<Player[]> {
    return this.http.get<Player[]>(`${this.baseUrl}/by-team/${teamId}`);
  }

  create(dto: PlayerCreate): Observable<Player> {
    return this.http.post<Player>(this.baseUrl, dto);
  }

  update(id: string, dto: PlayerUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
