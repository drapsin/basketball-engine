import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ArenaService } from '../../../core/services/arena.service';
import { Arena } from '../../../core/models/arena.model';
import { Conference, Division } from '../../../core/models/enums';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team.model';

@Component({
  selector: 'app-team-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './team-form.html',
  styleUrl: './team-form.scss',
})
export class TeamForm implements OnInit {
  private fb = inject(FormBuilder);
  private teamService = inject(TeamService);
  private arenaService = inject(ArenaService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  arenas = signal<Arena[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);

  conferences: Conference[] = ['Eastern', 'Western'];
  divisions: Division[] = ['Atlantic', 'Central', 'Southeast', 'Northwest', 'Pacific', 'Southwest'];

  private teamId: string | null = null;
  private currentRowVersion: string | null = null;

  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    city: ['', Validators.required],
    site: [''],
    sponsor: [''],
    news: [''],
    ranking: [''],
    contact: [''],
    conference: ['Eastern' as Conference, Validators.required],
    division: ['Atlantic' as Division, Validators.required],
    arenaId: ['', Validators.required],
    imageUrl: [''],
  });

  ngOnInit(): void {
    this.arenaService.getAll().subscribe((arenas) => this.arenas.set(arenas));

    this.teamId = this.route.snapshot.paramMap.get('id');
    if (this.teamId) {
      this.isEditMode.set(true);
      this.loading.set(true);
      this.teamService.getById(this.teamId).subscribe({
        next: (team) => {
          this.form.patchValue({
            name: team.name,
            city: team.city,
            site: team.site,
            sponsor: team.sponsor,
            news: team.news,
            ranking: team.ranking,
            contact: team.contact,
            conference: team.conference,
            division: team.division,
            arenaId: team.arenaId,
            imageUrl: team.imageUrl ?? '',
          });
          this.currentRowVersion = null;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load team.');
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

    const request: Observable<Team | void> =
      this.isEditMode() && this.teamId
        ? this.teamService.update(this.teamId, { ...value, rowVersion: this.currentRowVersion })
        : this.teamService.create(value);

    request.subscribe({
      next: (result) => {
        this.loading.set(false);
        const targetId = this.isEditMode() && this.teamId ? this.teamId : (result as Team).id;
        this.router.navigate(['/teams', targetId]);
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
