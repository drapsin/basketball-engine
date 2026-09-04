import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SimulationService } from '../../../core/services/simulation.service';
import { GameService } from '../../../core/services/game.service';
import { SimulationStatus } from '../../../core/models/simulation.model';
import { Game } from '../../../core/models/game.model';

interface LiveGameRow {
  status: SimulationStatus;
  game: Game | null;
}

@Component({
  selector: 'app-live-games-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './live-games-list.html',
  styleUrl: './live-games-list.scss',
})
export class LiveGamesList implements OnInit {
  private simulationService = inject(SimulationService);
  private gameService = inject(GameService);

  rows = signal<LiveGameRow[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.simulationService.getActiveGames().subscribe({
      next: (statuses) => {
        if (statuses.length === 0) {
          this.rows.set([]);
          this.loading.set(false);
          return;
        }

        const requests = statuses.map((status) =>
          this.gameService.getById(status.gameId).pipe(catchError(() => of(null))),
        );

        forkJoin(requests).subscribe((games) => {
          this.rows.set(statuses.map((status, i) => ({ status, game: games[i] })));
          this.loading.set(false);
        });
      },
      error: () => {
        this.error.set('Failed to load live games.');
        this.loading.set(false);
      },
    });
  }
}
