import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team.model';
import { Conference, Division } from '../../../core/models/enums';
import { AuthService } from '../../../core/services/auth.service';

const PAGE_SIZE = 12;

@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './team-list.html',
  styleUrl: './team-list.scss',
})
export class TeamList implements OnInit {
  teams = signal<Team[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  search = '';
  conference = '';
  division = '';
  page = signal(1);
  pageSize = PAGE_SIZE;

  conferenceOptions: Conference[] = ['Eastern', 'Western'];
  divisionOptions: Division[] = [
    'Atlantic',
    'Central',
    'Southeast',
    'Northwest',
    'Pacific',
    'Southwest',
  ];

  filteredTeams = computed(() => {
    const term = this.search.trim().toLowerCase();
    return this.teams().filter((team) => {
      const matchesSearch =
        !term || team.name.toLowerCase().includes(term) || team.city.toLowerCase().includes(term);
      const matchesConference = !this.conference || team.conference === this.conference;
      const matchesDivision = !this.division || team.division === this.division;
      return matchesSearch && matchesConference && matchesDivision;
    });
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.filteredTeams().length / this.pageSize)));

  pagedTeams = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.filteredTeams().slice(start, start + this.pageSize);
  });

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

  onFilterChange(): void {
    this.page.set(1);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.page.set(page);
  }
}
