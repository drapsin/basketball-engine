import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Team, TeamDetail, TeamCreate, TeamUpdate } from '../models/team.model';
import { Conference } from '../models/enums';

@Injectable({ providedIn: 'root' })
export class TeamService {
  private baseUrl = `${environment.apiUrl}/Team`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl);
  }

  getById(id: string): Observable<Team> {
    return this.http.get<Team>(`${this.baseUrl}/${id}`);
  }

  getDetailById(id: string): Observable<TeamDetail> {
    return this.http.get<TeamDetail>(`${this.baseUrl}/${id}/detail`);
  }

  getByConference(conference: Conference): Observable<Team[]> {
    return this.http.get<Team[]>(`${this.baseUrl}/by-conference/${conference}`);
  }

  create(dto: TeamCreate): Observable<Team> {
    return this.http.post<Team>(this.baseUrl, dto);
  }

  update(id: string, dto: TeamUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
