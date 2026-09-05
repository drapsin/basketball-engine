import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Player, PlayerCreate, PlayerUpdate } from '../models/player.model';

export interface PagedPlayers {
  items: Player[];
  totalCount: number;
}

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

  getPaged(
    page: number,
    pageSize: number,
    search: string,
    teamId: string,
    position: string,
  ): Observable<PagedPlayers> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    if (teamId) params = params.set('teamId', teamId);
    if (position) params = params.set('position', position);

    return this.http.get<Player[]>(`${this.baseUrl}/paged`, { params, observe: 'response' }).pipe(
      map((response) => ({
        items: response.body ?? [],
        totalCount: Number(response.headers.get('X-Total-Count') ?? 0),
      })),
    );
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
