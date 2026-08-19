import { Component, OnInit, signal, inject, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { GameService } from '../../../core/services/game.service';
import { TeamService } from '../../../core/services/team.service';
import { PlayerService } from '../../../core/services/player.service';
import { RefereeService } from '../../../core/services/referee.service';
import { ArenaService } from '../../../core/services/arena.service';
import { Team } from '../../../core/models/team.model';
import { Player } from '../../../core/models/player.model';
import { Referee } from '../../../core/models/referee.model';
import { Arena } from '../../../core/models/arena.model';
import { Game } from '../../../core/models/game.model';

const SPONSOR_STORAGE_KEY = 'nba_sponsor_suggestions';
const MAX_REFEREES = 3;

function differentTeamsValidator(control: AbstractControl): ValidationErrors | null {
  const home = control.get('homeTeamId')?.value;
  const away = control.get('awayTeamId')?.value;
  if (home && away && home === away) {
    return { sameTeam: true };
  }
  return null;
}

@Component({
  selector: 'app-game-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './game-form.html',
  styleUrl: './game-form.scss',
})
export class GameForm implements OnInit {
  private fb = inject(FormBuilder);
  private gameService = inject(GameService);
  private teamService = inject(TeamService);
  private playerService = inject(PlayerService);
  private refereeService = inject(RefereeService);
  private arenaService = inject(ArenaService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  teams = signal<Team[]>([]);
  arenas = signal<Arena[]>([]);
  referees = signal<Referee[]>([]);

  homeRoster = signal<Player[]>([]);
  awayRoster = signal<Player[]>([]);

  selectedRefereeIds = signal<Set<string>>(new Set());
  selectedPlayerIds = signal<Set<string>>(new Set());

  sponsorSuggestions = signal<string[]>([]);

  loading = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);

  readonly maxReferees = MAX_REFEREES;
  readonly todayStr = new Date().toISOString().substring(0, 10);

  private gameId: string | null = null;
  private currentRowVersion: string | null = null;
  private previousHomeRosterIds: string[] = [];
  private previousAwayRosterIds: string[] = [];

  form = this.fb.nonNullable.group(
    {
      gameDate: [this.todayStr, Validators.required],
      gameName: ['', Validators.required],
      gameTime: ['21:30', Validators.required],
      sponsor: [''],
      homeTeamId: ['', Validators.required],
      awayTeamId: ['', Validators.required],
      arenaId: ['', Validators.required],
      gameResult: [''],
    },
    { validators: differentTeamsValidator },
  );

  constructor() {
    this.form.controls.homeTeamId.valueChanges.subscribe((teamId) => {
      this.loadRoster(teamId, this.homeRoster, 'home');
    });
    this.form.controls.awayTeamId.valueChanges.subscribe((teamId) => {
      this.loadRoster(teamId, this.awayRoster, 'away');
    });
  }

  ngOnInit(): void {
    this.teamService.getAll().subscribe((teams) => this.teams.set(teams));
    this.arenaService.getAll().subscribe((arenas) => this.arenas.set(arenas));
    this.refereeService.getAll().subscribe((referees) => this.referees.set(referees));
    this.loadSponsorSuggestions();

    this.gameId = this.route.snapshot.paramMap.get('id');
    if (this.gameId) {
      this.isEditMode.set(true);
      this.loading.set(true);
      this.gameService.getDetailById(this.gameId).subscribe({
        next: (game) => {
          this.selectedRefereeIds.set(new Set(game.referees.map((r) => r.id)));
          this.selectedPlayerIds.set(new Set(game.players.map((p) => p.id)));
          this.form.patchValue({
            gameDate: game.gameDate.substring(0, 10),
            gameName: game.gameName,
            gameTime: game.gameTime,
            sponsor: game.sponsor,
            homeTeamId: game.homeTeamId,
            awayTeamId: game.awayTeamId,
            arenaId: game.arenaId,
            gameResult: game.gameResult ?? '',
          });
          this.currentRowVersion = null;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load game.');
          this.loading.set(false);
        },
      });
    }
  }

  private loadRoster(
    teamId: string,
    target: WritableSignal<Player[]>,
    side: 'home' | 'away',
  ): void {
    if (!teamId) {
      target.set([]);
      return;
    }

    this.playerService.getByTeamId(teamId).subscribe((players) => {
      target.set(players);

      const previousIds = side === 'home' ? this.previousHomeRosterIds : this.previousAwayRosterIds;

      this.selectedPlayerIds.update((set) => {
        const next = new Set(set);
        previousIds.forEach((id) => next.delete(id));
        if (!this.isEditMode()) {
          players.forEach((p) => next.add(p.id));
        }
        return next;
      });

      if (side === 'home') {
        this.previousHomeRosterIds = players.map((p) => p.id);
      } else {
        this.previousAwayRosterIds = players.map((p) => p.id);
      }
    });
  }

  toggleReferee(id: string, checked: boolean): void {
    if (checked && this.selectedRefereeIds().size >= MAX_REFEREES) {
      return;
    }
    this.selectedRefereeIds.update((set) => {
      const next = new Set(set);
      checked ? next.add(id) : next.delete(id);
      return next;
    });
  }

  togglePlayer(id: string, checked: boolean): void {
    this.selectedPlayerIds.update((set) => {
      const next = new Set(set);
      checked ? next.add(id) : next.delete(id);
      return next;
    });
  }

  private loadSponsorSuggestions(): void {
    try {
      const raw = localStorage.getItem(SPONSOR_STORAGE_KEY);
      this.sponsorSuggestions.set(raw ? JSON.parse(raw) : []);
    } catch {
      this.sponsorSuggestions.set([]);
    }
  }

  private saveSponsorSuggestion(value: string): void {
    const trimmed = value.trim();
    if (!trimmed) return;

    const current = this.sponsorSuggestions();
    const next = [
      trimmed,
      ...current.filter((s) => s.toLowerCase() !== trimmed.toLowerCase()),
    ].slice(0, 15);
    this.sponsorSuggestions.set(next);
    localStorage.setItem(SPONSOR_STORAGE_KEY, JSON.stringify(next));
  }

  clearSponsorSuggestions(): void {
    this.sponsorSuggestions.set([]);
    localStorage.removeItem(SPONSOR_STORAGE_KEY);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error.set(null);
    this.loading.set(true);
    const value = this.form.getRawValue();

    const payload = {
      gameDate: value.gameDate,
      gameName: value.gameName,
      gameTime: value.gameTime,
      sponsor: value.sponsor,
      homeTeamId: value.homeTeamId,
      awayTeamId: value.awayTeamId,
      arenaId: value.arenaId,
      refereeIds: Array.from(this.selectedRefereeIds()),
      playerIds: Array.from(this.selectedPlayerIds()),
    };

    const request: Observable<Game | void> =
      this.isEditMode() && this.gameId
        ? this.gameService.update(this.gameId, {
            ...payload,
            gameResult: value.gameResult || null,
            rowVersion: this.currentRowVersion,
          })
        : this.gameService.create(payload);

    request.subscribe({
      next: () => {
        this.loading.set(false);
        this.saveSponsorSuggestion(value.sponsor);
        this.router.navigate(['/games']);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(
          err.status === 400 ? 'Please check the form for errors.' : 'Something went wrong.',
        );
      },
    });
  }
}
