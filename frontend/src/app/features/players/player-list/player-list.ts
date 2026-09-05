import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PlayerService } from '../../../core/services/player.service';
import { TeamService } from '../../../core/services/team.service';
import { AuthService } from '../../../core/services/auth.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { Player } from '../../../core/models/player.model';
import { Team } from '../../../core/models/team.model';

const PAGE_SIZE = 25;

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './player-list.html',
  styleUrl: './player-list.scss',
})
export class PlayerList implements OnInit, OnDestroy {
  private playerService = inject(PlayerService);
  private teamService = inject(TeamService);
  private confirmDialog = inject(ConfirmDialogService);
  public authService = inject(AuthService);

  players = signal<Player[]>([]);
  teams = signal<Team[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  deletingId = signal<string | null>(null);

  page = signal(1);
  pageSize = PAGE_SIZE;
  totalCount = signal(0);

  search = '';
  teamId = '';
  position = '';

  private searchDebounce?: ReturnType<typeof setTimeout>;

  positionOptions = ['G', 'F', 'C', 'G-F', 'F-C'];

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize));
  }

  ngOnInit(): void {
    this.teamService.getAll().subscribe({
      next: (teams) => this.teams.set(teams),
      error: () => {},
    });
    this.load();
  }

  ngOnDestroy(): void {
    clearTimeout(this.searchDebounce);
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.playerService
      .getPaged(this.page(), this.pageSize, this.search, this.teamId, this.position)
      .subscribe({
        next: ({ items, totalCount }) => {
          this.players.set(items);
          this.totalCount.set(totalCount);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load players. Is the backend running?');
          this.loading.set(false);
        },
      });
  }

  onSearchChange(): void {
    clearTimeout(this.searchDebounce);
    this.searchDebounce = setTimeout(() => {
      this.page.set(1);
      this.load();
    }, 300);
  }

  onFilterChange(): void {
    this.page.set(1);
    this.load();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.page.set(page);
    this.load();
  }

  onDelete(player: Player): void {
    this.confirmDialog
      .confirm(`Delete ${player.firstName} ${player.lastName}?`, 'Delete Player')
      .subscribe((confirmed) => {
        if (!confirmed) return;

        this.deletingId.set(player.id);
        this.playerService.delete(player.id).subscribe({
          next: () => {
            this.deletingId.set(null);
            this.load();
          },
          error: () => {
            this.error.set('Failed to delete player.');
            this.deletingId.set(null);
          },
        });
      });
  }
}
