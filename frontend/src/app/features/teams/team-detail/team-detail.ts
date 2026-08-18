import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TeamService } from '../../../core/services/team.service';
import { TeamDetail } from '../../../core/models/team.model';

@Component({
  selector: 'app-team-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './team-detail.html',
  styleUrl: './team-detail.scss',
})
export class TeamDetailComponent implements OnInit {
  team = signal<TeamDetail | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
    private teamService: TeamService,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('No team ID provided.');
      this.loading.set(false);
      return;
    }

    this.teamService.getDetailById(id).subscribe({
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
}
