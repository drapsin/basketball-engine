import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Game, GameDetail, GameCreate, GameUpdate } from '../models/game.model';
import { GameState } from '../models/stats.model';

@Injectable({ providedIn: 'root' })
export class GameService {
  private baseUrl = `${environment.apiUrl}/Game`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Game[]> {
    return this.http.get<Game[]>(this.baseUrl);
  }

  getById(id: string): Observable<Game> {
    return this.http.get<Game>(`${this.baseUrl}/${id}`);
  }

  getDetailById(id: string): Observable<GameDetail> {
    return this.http.get<GameDetail>(`${this.baseUrl}/${id}/detail`);
  }

  getByTeamId(teamId: string): Observable<Game[]> {
    return this.http.get<Game[]>(`${this.baseUrl}/by-team/${teamId}`);
  }

  create(dto: GameCreate): Observable<Game> {
    return this.http.post<Game>(this.baseUrl, dto);
  }

  update(id: string, dto: GameUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getState(id: string): Observable<GameState> {
    return this.http.get<GameState>(`${this.baseUrl}/${id}/state`);
  }
}
