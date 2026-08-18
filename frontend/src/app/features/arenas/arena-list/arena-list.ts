import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ArenaService } from '../../../core/services/arena.service';
import { AuthService } from '../../../core/services/auth.service';
import { Arena } from '../../../core/models/arena.model';

@Component({
  selector: 'app-arena-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './arena-list.html',
  styleUrl: './arena-list.scss',
})
export class ArenaList implements OnInit {
  private arenaService = inject(ArenaService);
  public authService = inject(AuthService);

  arenas = signal<Arena[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  deletingId = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.arenaService.getAll().subscribe({
      next: (arenas) => {
        this.arenas.set(arenas);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load arenas. Is the backend running?');
        this.loading.set(false);
      },
    });
  }

  onDelete(arena: Arena): void {
    const confirmed = window.confirm(`Delete ${arena.arenaName}?`);
    if (!confirmed) return;

    this.deletingId.set(arena.id);
    this.arenaService.delete(arena.id).subscribe({
      next: () => {
        this.arenas.update((list) => list.filter((a) => a.id !== arena.id));
        this.deletingId.set(null);
      },
      error: () => {
        this.error.set('Failed to delete arena — it may be in use by a team.');
        this.deletingId.set(null);
      },
    });
  }
}
