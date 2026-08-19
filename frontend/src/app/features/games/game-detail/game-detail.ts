import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GameService } from '../../../core/services/game.service';
import { AuthService } from '../../../core/services/auth.service';
import { GameDetail } from '../../../core/models/game.model';

@Component({
  selector: 'app-game-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './game-detail.html',
  styleUrl: './game-detail.scss',
})
export class GameDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private gameService = inject(GameService);
  public authService = inject(AuthService);

  game = signal<GameDetail | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  deleting = signal(false);

  private gameId: string | null = null;

  ngOnInit(): void {
    this.gameId = this.route.snapshot.paramMap.get('id');
    if (!this.gameId) {
      this.error.set('No game ID provided.');
      this.loading.set(false);
      return;
    }

    this.gameService.getDetailById(this.gameId).subscribe({
      next: (game) => {
        this.game.set(game);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load game.');
        this.loading.set(false);
      },
    });
  }

  homeRoster() {
    const g = this.game();
    return g ? g.players.filter((p) => p.teamId === g.homeTeamId) : [];
  }

  awayRoster() {
    const g = this.game();
    return g ? g.players.filter((p) => p.teamId === g.awayTeamId) : [];
  }

  onDelete(): void {
    if (!this.gameId) return;

    const g = this.game();
    const label = g ? `${g.awayTeamName} @ ${g.homeTeamName}` : 'this game';
    const confirmed = window.confirm(`Delete ${label}? This cannot be undone.`);
    if (!confirmed) return;

    this.deleting.set(true);
    this.gameService.delete(this.gameId).subscribe({
      next: () => {
        this.router.navigate(['/games']);
      },
      error: () => {
        this.deleting.set(false);
        this.error.set('Failed to delete game.');
      },
    });
  }
}
