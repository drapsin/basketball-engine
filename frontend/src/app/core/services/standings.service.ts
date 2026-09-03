import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TeamStanding } from '../models/standing.model';

@Injectable({ providedIn: 'root' })
export class StandingsService {
  private baseUrl = `${environment.apiUrl}/Standings`;

  constructor(private http: HttpClient) {}

  getStandings(): Observable<TeamStanding[]> {
    return this.http.get<TeamStanding[]>(this.baseUrl);
  }
}
