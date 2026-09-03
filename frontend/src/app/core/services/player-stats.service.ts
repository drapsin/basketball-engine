import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  PlayerCareerStats,
  TeamCareerStats,
  LeagueLeader,
  LeaderCategory,
} from '../models/player-stats.model';

@Injectable({ providedIn: 'root' })
export class PlayerStatsService {
  private baseUrl = `${environment.apiUrl}/PlayerStats`;

  constructor(private http: HttpClient) {}

  getPlayerCareer(playerId: string): Observable<PlayerCareerStats> {
    return this.http.get<PlayerCareerStats>(`${this.baseUrl}/${playerId}/career`);
  }

  getTeamCareer(teamId: string): Observable<TeamCareerStats> {
    return this.http.get<TeamCareerStats>(`${this.baseUrl}/team/${teamId}/career`);
  }

  getLeaders(category: LeaderCategory, limit: number = 10): Observable<LeagueLeader[]> {
    return this.http.get<LeagueLeader[]>(`${this.baseUrl}/leaders`, {
      params: { category, limit: limit.toString() },
    });
  }
}
