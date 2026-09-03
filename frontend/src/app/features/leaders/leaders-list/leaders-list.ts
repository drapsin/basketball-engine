import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PlayerStatsService } from '../../../core/services/player-stats.service';
import { LeagueLeader, LeaderCategory } from '../../../core/models/player-stats.model';

@Component({
  selector: 'app-leaders-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './leaders-list.html',
  styleUrl: './leaders-list.scss',
})
export class LeadersList implements OnInit {
  private playerStatsService = inject(PlayerStatsService);

  categories: { value: LeaderCategory; label: string }[] = [
    { value: 'points', label: 'Points' },
    { value: 'rebounds', label: 'Rebounds' },
    { value: 'assists', label: 'Assists' },
    { value: 'steals', label: 'Steals' },
    { value: 'blocks', label: 'Blocks' },
  ];

  selectedCategory = signal<LeaderCategory>('points');
  leaders = signal<LeagueLeader[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  onCategoryChange(category: LeaderCategory): void {
    this.selectedCategory.set(category);
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.playerStatsService.getLeaders(this.selectedCategory(), 10).subscribe({
      next: (leaders) => {
        this.leaders.set(leaders);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load league leaders.');
        this.loading.set(false);
      },
    });
  }
}
