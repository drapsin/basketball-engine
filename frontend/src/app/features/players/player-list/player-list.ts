import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PlayerService } from '../../../core/services/player.service';
import { AuthService } from '../../../core/services/auth.service';
import { Player } from '../../../core/models/player.model';

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './player-list.html',
  styleUrl: './player-list.scss',
})
export class PlayerList implements OnInit {
  private playerService = inject(PlayerService);
  public authService = inject(AuthService);

  players = signal<Player[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  deletingId = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.playerService.getAll().subscribe({
      next: (players) => {
        this.players.set(players);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load players. Is the backend running?');
        this.loading.set(false);
      },
    });
  }

  onDelete(player: Player): void {
    const confirmed = window.confirm(`Delete ${player.firstName} ${player.lastName}?`);
    if (!confirmed) return;

    this.deletingId.set(player.id);
    this.playerService.delete(player.id).subscribe({
      next: () => {
        this.players.update((list) => list.filter((p) => p.id !== player.id));
        this.deletingId.set(null);
      },
      error: () => {
        this.error.set('Failed to delete player.');
        this.deletingId.set(null);
      },
    });
  }
}
