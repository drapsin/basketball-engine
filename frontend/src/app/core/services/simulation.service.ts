import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SimulationStatus } from '../models/simulation.model';

@Injectable({ providedIn: 'root' })
export class SimulationService {
  private baseUrl = `${environment.apiUrl}/Simulation`;

  constructor(private http: HttpClient) {}

  start(gameId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${gameId}/start`, {});
  }

  pause(gameId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${gameId}/pause`, {});
  }

  resume(gameId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${gameId}/resume`, {});
  }

  stop(gameId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${gameId}/stop`, {});
  }

  getStatus(gameId: string): Observable<SimulationStatus> {
    return this.http.get<SimulationStatus>(`${this.baseUrl}/${gameId}/status`);
  }
}
