import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PlayerService } from '../../../core/services/player.service';
import { AuthService } from '../../../core/services/auth.service';
import { Player } from '../../../core/models/player.model';

@Component({
  selector: 'app-player-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './player-detail.html',
  styleUrl: './player-detail.scss',
})
export class PlayerDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private playerService = inject(PlayerService);
  public authService = inject(AuthService);

  player = signal<Player | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  deleting = signal(false);

  private playerId: string | null = null;

  ngOnInit(): void {
    this.playerId = this.route.snapshot.paramMap.get('id');
    if (!this.playerId) {
      this.error.set('No player ID provided.');
      this.loading.set(false);
      return;
    }

    this.playerService.getById(this.playerId).subscribe({
      next: (player) => {
        this.player.set(player);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load player.');
        this.loading.set(false);
      },
    });
  }

  onDelete(): void {
    if (!this.playerId) return;

    const name = this.player()
      ? `${this.player()!.firstName} ${this.player()!.lastName}`
      : 'this player';
    const confirmed = window.confirm(`Delete ${name}? This cannot be undone.`);
    if (!confirmed) return;

    this.deleting.set(true);
    this.playerService.delete(this.playerId).subscribe({
      next: () => {
        this.router.navigate(['/players']);
      },
      error: () => {
        this.deleting.set(false);
        this.error.set('Failed to delete player.');
      },
    });
  }
}
