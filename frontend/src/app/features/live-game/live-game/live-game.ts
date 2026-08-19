import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { SignalrService } from '../../../core/services/signalr.service';
import { SimulationService } from '../../../core/services/simulation.service';
import { GameService } from '../../../core/services/game.service';
import { AuthService } from '../../../core/services/auth.service';
import { Game } from '../../../core/models/game.model';

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

  private gameId: string | null = null;

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

    this.refreshSimStatus();
    this.signalrService.connect(this.gameId);
  }

  ngOnDestroy(): void {
    this.signalrService.disconnect();
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
}
