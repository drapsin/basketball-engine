import { Component, Input, Output, EventEmitter, signal, inject, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActionEventService } from '../../../core/services/action-event.service';
import { PlayerService } from '../../../core/services/player.service';
import { Player } from '../../../core/models/player.model';
import { SCORING_EVENT_TYPES, EventType } from '../../../core/models/event-type.enum';
import { Game } from '../../../core/models/game.model';

const QUARTER_LENGTH_SECONDS = 720; // 12-minute quarters, matches backend

@Component({
  selector: 'app-scoring-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './scoring-panel.html',
  styleUrl: './scoring-panel.scss',
})
export class ScoringPanel implements OnChanges {
  private playerService = inject(PlayerService);
  private actionEventService = inject(ActionEventService);

  @Input({ required: true }) game!: Game;
  @Input({ required: true }) currentQuarter!: number;
  @Input({ required: true }) currentClock!: string; // "m:ss" display format
  @Output() eventLogged = new EventEmitter<void>();

  eventTypes = SCORING_EVENT_TYPES;

  homeRoster = signal<Player[]>([]);
  awayRoster = signal<Player[]>([]);
  selectedPlayerId = signal<string | null>(null);
  selectedTeamId = signal<string | null>(null);
  onCourtPlayerIds = signal<Set<string>>(new Set());

  loading = signal(false);
  error = signal<string | null>(null);
  lastLogged = signal<string | null>(null);

  ngOnChanges(): void {
    if (this.game && this.homeRoster().length === 0) {
      this.playerService.getByTeamId(this.game.homeTeamId).subscribe((p) => this.homeRoster.set(p));
      this.playerService.getByTeamId(this.game.awayTeamId).subscribe((p) => this.awayRoster.set(p));
      this.loadOnCourtStatus();
    }
  }

  private parseGameTimeSeconds(gameTime: string): number {
    const parts = gameTime.split(':').map(Number);
    if (parts.length === 3) return parts[0] * 3600 + parts[1] * 60 + parts[2];
    if (parts.length === 2) return parts[0] * 60 + parts[1];
    return 0;
  }

  private elapsedSeconds(quarter: number, gameTime: string): number {
    const remaining = this.parseGameTimeSeconds(gameTime);
    const elapsedInQuarter = QUARTER_LENGTH_SECONDS - remaining;
    return (quarter - 1) * QUARTER_LENGTH_SECONDS + elapsedInQuarter;
  }

  loadOnCourtStatus(): void {
    if (!this.game) return;
    this.actionEventService.getByGameId(this.game.id).subscribe({
      next: (events) => {
        const subEvents = events
          .filter((e) => e.eventType === 'SubstituteIn' || e.eventType === 'SubstituteOut')
          .sort(
            (a, b) =>
              this.elapsedSeconds(a.quarter, a.gameTime) -
              this.elapsedSeconds(b.quarter, b.gameTime),
          );

        const onCourt = new Set<string>();
        for (const evt of subEvents) {
          if (evt.eventType === 'SubstituteIn') onCourt.add(evt.playerId);
          else onCourt.delete(evt.playerId);
        }
        this.onCourtPlayerIds.set(onCourt);
      },
      error: () => {
        // Non-critical — substitution tracking just won't reflect reality until this succeeds
      },
    });
  }

  selectPlayer(player: Player): void {
    this.selectedPlayerId.set(player.id);
    this.selectedTeamId.set(player.teamId);
  }

  isOnCourt(playerId: string): boolean {
    return this.onCourtPlayerIds().has(playerId);
  }

  isSelectedOnCourt(): boolean {
    const id = this.selectedPlayerId();
    return id ? this.isOnCourt(id) : false;
  }

  private toBackendClock(): string {
    const [m, s] = this.currentClock.split(':').map(Number);
    const totalMinutes = m ?? 0;
    const seconds = s ?? 0;
    const hh = Math.floor(totalMinutes / 60)
      .toString()
      .padStart(2, '0');
    const mm = (totalMinutes % 60).toString().padStart(2, '0');
    const ss = seconds.toString().padStart(2, '0');
    return `${hh}:${mm}:${ss}`;
  }

  logEvent(eventType: EventType): void {
    const playerId = this.selectedPlayerId();
    const teamId = this.selectedTeamId();

    if (!playerId || !teamId) {
      this.error.set('Select a player first.');
      return;
    }

    this.error.set(null);
    this.loading.set(true);

    this.actionEventService
      .create({
        gameId: this.game.id,
        playerId,
        teamId,
        quarter: this.currentQuarter,
        gameTime: this.toBackendClock(),
        eventType,
      })
      .subscribe({
        next: (created) => {
          this.loading.set(false);
          this.lastLogged.set(
            `${created.playerName} — ${this.eventTypes.find((e) => e.value === eventType)?.label}`,
          );
          this.eventLogged.emit();
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to log event.');
        },
      });
  }

  logSubstitution(): void {
    const playerId = this.selectedPlayerId();
    const teamId = this.selectedTeamId();

    if (!playerId || !teamId) {
      this.error.set('Select a player first.');
      return;
    }

    const eventType: EventType = this.isOnCourt(playerId) ? 'SubstituteOut' : 'SubstituteIn';

    this.error.set(null);
    this.loading.set(true);

    this.actionEventService
      .create({
        gameId: this.game.id,
        playerId,
        teamId,
        quarter: this.currentQuarter,
        gameTime: this.toBackendClock(),
        eventType,
      })
      .subscribe({
        next: (created) => {
          this.loading.set(false);
          this.lastLogged.set(
            `${created.playerName} — ${eventType === 'SubstituteIn' ? 'Sub In' : 'Sub Out'}`,
          );
          this.loadOnCourtStatus();
          this.eventLogged.emit();
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to log substitution.');
        },
      });
  }
}
