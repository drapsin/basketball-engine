import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ArenaService } from '../../../core/services/arena.service';
import { AuthService } from '../../../core/services/auth.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { Arena } from '../../../core/models/arena.model';

@Component({
  selector: 'app-arena-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './arena-detail.html',
  styleUrl: './arena-detail.scss',
})
export class ArenaDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private arenaService = inject(ArenaService);
  private confirmDialog = inject(ConfirmDialogService);
  public authService = inject(AuthService);

  arena = signal<Arena | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  deleting = signal(false);

  private arenaId: string | null = null;

  ngOnInit(): void {
    this.arenaId = this.route.snapshot.paramMap.get('id');
    if (!this.arenaId) {
      this.error.set('No arena ID provided.');
      this.loading.set(false);
      return;
    }

    this.arenaService.getById(this.arenaId).subscribe({
      next: (arena) => {
        this.arena.set(arena);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load arena.');
        this.loading.set(false);
      },
    });
  }

  onDelete(): void {
    if (!this.arenaId) return;

    const label = this.arena()?.arenaName ?? 'this arena';

    this.confirmDialog.confirm(`Delete ${label}?`, 'Delete Arena').subscribe((confirmed) => {
      if (!confirmed) return;

      this.deleting.set(true);
      this.arenaService.delete(this.arenaId!).subscribe({
        next: () => {
          this.router.navigate(['/arenas']);
        },
        error: () => {
          this.deleting.set(false);
          this.error.set('Failed to delete arena — it may be in use by a team.');
        },
      });
    });
  }
}
