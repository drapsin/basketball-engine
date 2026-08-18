import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TeamService } from '../../../core/services/team.service';
import { AuthService } from '../../../core/services/auth.service';
import { TeamDetail } from '../../../core/models/team.model';

@Component({
  selector: 'app-team-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './team-detail.html',
  styleUrl: './team-detail.scss',
})
export class TeamDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private teamService = inject(TeamService);
  public authService = inject(AuthService);

  team = signal<TeamDetail | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  deleting = signal(false);

  private teamId: string | null = null;

  ngOnInit(): void {
    this.teamId = this.route.snapshot.paramMap.get('id');
    if (!this.teamId) {
      this.error.set('No team ID provided.');
      this.loading.set(false);
      return;
    }

    this.teamService.getDetailById(this.teamId).subscribe({
      next: (team) => {
        this.team.set(team);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load team.');
        this.loading.set(false);
      },
    });
  }

  onDelete(): void {
    if (!this.teamId) return;

    const teamName = this.team()?.name ?? 'this team';
    const confirmed = window.confirm(`Delete ${teamName}? This cannot be undone.`);
    if (!confirmed) return;

    this.deleting.set(true);
    this.teamService.delete(this.teamId).subscribe({
      next: () => {
        this.router.navigate(['/teams']);
      },
      error: (err) => {
        this.deleting.set(false);
        this.error.set(
          err.status === 400
            ? 'Cannot delete this team — it may have related data (players, games) blocking deletion.'
            : 'Failed to delete team.',
        );
      },
    });
  }
}
