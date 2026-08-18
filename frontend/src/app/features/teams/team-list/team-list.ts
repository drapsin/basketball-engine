import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './team-list.html',
  styleUrl: './team-list.scss',
})
export class TeamList implements OnInit {
  teams = signal<Team[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private teamService: TeamService,
    public authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.teamService.getAll().subscribe({
      next: (teams) => {
        this.teams.set(teams);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load teams. Is the backend running?');
        this.loading.set(false);
      },
    });
  }
}
