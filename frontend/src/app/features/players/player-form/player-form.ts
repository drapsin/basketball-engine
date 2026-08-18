import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { PlayerService } from '../../../core/services/player.service';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team.model';
import { Player } from '../../../core/models/player.model';

@Component({
  selector: 'app-player-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './player-form.html',
  styleUrl: './player-form.scss',
})
export class PlayerForm implements OnInit {
  private fb = inject(FormBuilder);
  private playerService = inject(PlayerService);
  private teamService = inject(TeamService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  teams = signal<Team[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);

  private playerId: string | null = null;
  private currentRowVersion: string | null = null;

  form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    age: [20, [Validators.required, Validators.min(0), Validators.max(100)]],
    position: ['', Validators.required],
    teamId: ['', Validators.required],
    height: [190, [Validators.required, Validators.min(150), Validators.max(230)]],
    weight: [90, [Validators.required, Validators.min(70), Validators.max(200)]],
    agent: [''],
    sponsor: [''],
    news: [''],
    imageUrl: [''],
  });

  ngOnInit(): void {
    this.teamService.getAll().subscribe((teams) => this.teams.set(teams));

    this.playerId = this.route.snapshot.paramMap.get('id');
    if (this.playerId) {
      this.isEditMode.set(true);
      this.loading.set(true);
      this.playerService.getById(this.playerId).subscribe({
        next: (player) => {
          this.form.patchValue({
            firstName: player.firstName,
            lastName: player.lastName,
            age: player.age,
            position: player.position,
            teamId: player.teamId,
            height: player.height,
            weight: player.weight,
            agent: player.agent,
            sponsor: player.sponsor,
            news: player.news,
            imageUrl: player.imageUrl ?? '',
          });
          this.currentRowVersion = null;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load player.');
          this.loading.set(false);
        },
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error.set(null);
    this.loading.set(true);
    const value = this.form.getRawValue();

    const request: Observable<Player | void> =
      this.isEditMode() && this.playerId
        ? this.playerService.update(this.playerId, { ...value, rowVersion: this.currentRowVersion })
        : this.playerService.create(value);

    request.subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/players']);
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
