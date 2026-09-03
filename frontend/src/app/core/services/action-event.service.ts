import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ActionEventDto, ActionEventCreate } from '../models/action-event.model';

@Injectable({ providedIn: 'root' })
export class ActionEventService {
  private baseUrl = `${environment.apiUrl}/ActionEvent`;

  constructor(private http: HttpClient) {}

  getByGameId(gameId: string): Observable<ActionEventDto[]> {
    return this.http.get<ActionEventDto[]>(`${this.baseUrl}/by-game/${gameId}`);
  }

  create(dto: ActionEventCreate): Observable<ActionEventDto> {
    return this.http.post<ActionEventDto>(this.baseUrl, dto);
  }
}
