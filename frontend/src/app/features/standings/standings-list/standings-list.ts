import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { StandingsService } from '../../../core/services/standings.service';
import { TeamStanding } from '../../../core/models/standing.model';
import { Conference } from '../../../core/models/enums';

@Component({
  selector: 'app-standings-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './standings-list.html',
  styleUrl: './standings-list.scss',
})
export class StandingsList implements OnInit {
  private standingsService = inject(StandingsService);

  standings = signal<TeamStanding[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  selectedConference = signal<Conference>('Eastern');

  filteredStandings = computed(() =>
    this.standings()
      .filter((s) => s.conference === this.selectedConference())
      .sort((a, b) => a.conferenceRank - b.conferenceRank),
  );

  ngOnInit(): void {
    this.standingsService.getStandings().subscribe({
      next: (standings) => {
        this.standings.set(standings);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load standings.');
        this.loading.set(false);
      },
    });
  }

  onConferenceChange(conference: Conference): void {
    this.selectedConference.set(conference);
  }
}
