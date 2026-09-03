import { Component, Input, Output, EventEmitter, signal, inject, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActionEventService } from '../../../core/services/action-event.service';
import { PlayerService } from '../../../core/services/player.service';
import { Player } from '../../../core/models/player.model';
import { SCORING_EVENT_TYPES, EventType } from '../../../core/models/event-type.enum';
import { Game } from '../../../core/models/game.model';

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

  loading = signal(false);
  error = signal<string | null>(null);
  lastLogged = signal<string | null>(null);

  ngOnChanges(): void {
    if (this.game && this.homeRoster().length === 0) {
      this.playerService.getByTeamId(this.game.homeTeamId).subscribe((p) => this.homeRoster.set(p));
      this.playerService.getByTeamId(this.game.awayTeamId).subscribe((p) => this.awayRoster.set(p));
    }
  }

  selectPlayer(player: Player): void {
    this.selectedPlayerId.set(player.id);
    this.selectedTeamId.set(player.teamId);
  }

  private toBackendClock(): string {
    // currentClock is "m:ss" from the live page's ticking display; backend expects "hh:mm:ss"
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
}
