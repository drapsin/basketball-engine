import { Component, OnInit, OnDestroy, signal, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { SignalrService } from '../../../core/services/signalr.service';
import { SimulationService } from '../../../core/services/simulation.service';
import { GameService } from '../../../core/services/game.service';
import { AuthService } from '../../../core/services/auth.service';
import { Game } from '../../../core/models/game.model';
import { GameState } from '../../../core/models/stats.model';

type SimState = 'unknown' | 'stopped' | 'running' | 'paused';

@Component({
  selector: 'app-live-game',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './live-game.html',
  styleUrl: './live-game.scss',
})
export class LiveGame implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private gameService = inject(GameService);
  private simulationService = inject(SimulationService);
  public signalrService = inject(SignalrService);
  public authService = inject(AuthService);

  game = signal<Game | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  actionError = signal<string | null>(null);
  simState = signal<SimState>('unknown');
  actionLoading = signal(false);
  displayClock = signal<string>('12:00');

  // Fallback state fetched via REST, shown until/unless SignalR pushes something live
  initialState = signal<GameState | null>(null);

  private gameId: string | null = null;
  private statusPollHandle: ReturnType<typeof setInterval> | null = null;
  private clockTickHandle: ReturnType<typeof setInterval> | null = null;
  private clockSecondsRemaining = 0;

  constructor() {
    // Resync the local ticking clock every time a real update arrives via SignalR,
    // so it can't drift away from the server's actual value indefinitely.
    effect(() => {
      const state = this.signalrService.gameState();
      if (state) {
        this.syncClock(state.gameClock);
      }
    });
  }

  ngOnInit(): void {
    this.gameId = this.route.snapshot.paramMap.get('id');
    if (!this.gameId) {
      this.error.set('No game ID provided.');
      this.loading.set(false);
      return;
    }

    this.gameService.getById(this.gameId).subscribe({
      next: (game) => {
        this.game.set(game);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load game.');
        this.loading.set(false);
      },
    });

    // Fetch current state immediately so paused/finished games show real data right away
    this.gameService.getState(this.gameId).subscribe({
      next: (state) => {
        this.initialState.set(state);
        this.syncClock(state.gameClock);
      },
      error: () => {
        // No events yet for this game — nothing to show until live data arrives, that's fine
      },
    });

    this.refreshSimStatus();
    this.statusPollHandle = setInterval(() => this.refreshSimStatus(), 5000);
    this.startClockTicking();
    this.signalrService.connect(this.gameId);
  }

  ngOnDestroy(): void {
    this.signalrService.disconnect();
    if (this.statusPollHandle) clearInterval(this.statusPollHandle);
    if (this.clockTickHandle) clearInterval(this.clockTickHandle);
  }

  // The state actually shown: prefer live SignalR data once it arrives, fall back to the REST snapshot
  currentState(): GameState | null {
    return this.signalrService.gameState() ?? this.initialState();
  }

  private parseClockToSeconds(clock: string): number {
    const parts = clock.split(':').map(Number);
    if (parts.length === 3) return parts[0] * 3600 + parts[1] * 60 + parts[2];
    if (parts.length === 2) return parts[0] * 60 + parts[1];
    return 0;
  }

  private formatSecondsToClock(totalSeconds: number): string {
    const s = Math.max(0, totalSeconds);
    const m = Math.floor(s / 60);
    const sec = s % 60;
    return `${m}:${sec.toString().padStart(2, '0')}`;
  }

  private syncClock(clock: string): void {
    this.clockSecondsRemaining = this.parseClockToSeconds(clock);
    this.displayClock.set(this.formatSecondsToClock(this.clockSecondsRemaining));
  }

  private startClockTicking(): void {
    if (this.clockTickHandle) return;
    this.clockTickHandle = setInterval(() => {
      if (this.simState() === 'running' && this.clockSecondsRemaining > 0) {
        this.clockSecondsRemaining--;
        this.displayClock.set(this.formatSecondsToClock(this.clockSecondsRemaining));
      }
    }, 1000);
  }

  refreshSimStatus(): void {
    if (!this.gameId) return;
    this.simulationService.getStatus(this.gameId).subscribe({
      next: (status) => {
        this.simState.set(status.isPaused ? 'paused' : 'running');
      },
      error: () => {
        this.simState.set('stopped');
      },
    });
  }

  private runAction(action: Observable<{ message: string }>): void {
    this.actionError.set(null);
    this.actionLoading.set(true);
    action.subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.refreshSimStatus();
      },
      error: (err) => {
        this.actionLoading.set(false);
        this.actionError.set(err.error?.message ?? 'Action failed.');
      },
    });
  }

  onStart(): void {
    if (!this.gameId) return;
    this.runAction(this.simulationService.start(this.gameId));
  }

  onPause(): void {
    if (!this.gameId) return;
    this.runAction(this.simulationService.pause(this.gameId));
  }

  onResume(): void {
    if (!this.gameId) return;
    this.runAction(this.simulationService.resume(this.gameId));
  }

  onStop(): void {
    if (!this.gameId) return;
    this.runAction(this.simulationService.stop(this.gameId));
  }

  formatEventType(eventType: string | number): string {
    return String(eventType).replace(/([a-z])([A-Z])/g, '$1 $2');
  }
}
