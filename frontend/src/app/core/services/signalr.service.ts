import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { GameState } from '../models/stats.model';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class SignalrService {
  private hubConnection: signalR.HubConnection | null = null;
  private currentGameId: string | null = null;

  gameState = signal<GameState | null>(null);
  connectionState = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');

  constructor(private authService: AuthService) {}

  async connect(gameId: string): Promise<void> {
    // Already connected to this exact game — no-op
    if (this.hubConnection && this.currentGameId === gameId) return;

    // Switching games or first connect — tear down any previous connection
    await this.disconnect();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        accessTokenFactory: () => this.authService.getToken() ?? '',
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveGameUpdate', (state: GameState) => {
      this.gameState.set(state);
    });

    this.hubConnection.onreconnecting(() => this.connectionState.set('connecting'));
    this.hubConnection.onreconnected(() => {
      this.connectionState.set('connected');
      this.hubConnection?.invoke('JoinGame', gameId);
    });
    this.hubConnection.onclose(() => this.connectionState.set('disconnected'));

    this.connectionState.set('connecting');
    await this.hubConnection.start();
    this.connectionState.set('connected');

    this.currentGameId = gameId;
    await this.hubConnection.invoke('JoinGame', gameId);
  }

  async disconnect(): Promise<void> {
    if (!this.hubConnection) return;

    if (this.currentGameId) {
      try {
        await this.hubConnection.invoke('LeaveGame', this.currentGameId);
      } catch {
        // connection may already be closed — safe to ignore
      }
    }

    await this.hubConnection.stop();
    this.hubConnection = null;
    this.currentGameId = null;
    this.gameState.set(null);
    this.connectionState.set('disconnected');
  }
}
