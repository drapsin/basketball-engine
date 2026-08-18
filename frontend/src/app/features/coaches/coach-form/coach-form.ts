import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { CoachService } from '../../../core/services/coach.service';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team.model';
import { Coach } from '../../../core/models/coach.model';

@Component({
  selector: 'app-coach-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './coach-form.html',
  styleUrl: './coach-form.scss',
})
export class CoachForm implements OnInit {
  private fb = inject(FormBuilder);
  private coachService = inject(CoachService);
  private teamService = inject(TeamService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  teams = signal<Team[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);

  private coachId: string | null = null;
  private currentRowVersion: string | null = null;

  form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    age: [45, [Validators.required, Validators.min(0), Validators.max(100)]],
    history: [''],
    teamId: ['', Validators.required],
    imageUrl: [''],
  });

  ngOnInit(): void {
    this.teamService.getAll().subscribe((teams) => this.teams.set(teams));

    this.coachId = this.route.snapshot.paramMap.get('id');
    if (this.coachId) {
      this.isEditMode.set(true);
      this.loading.set(true);
      this.coachService.getById(this.coachId).subscribe({
        next: (coach) => {
          this.form.patchValue({
            firstName: coach.firstName,
            lastName: coach.lastName,
            age: coach.age,
            history: coach.history,
            teamId: coach.teamId,
            imageUrl: coach.imageUrl ?? '',
          });
          this.currentRowVersion = null;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load coach.');
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

    const request: Observable<Coach | void> =
      this.isEditMode() && this.coachId
        ? this.coachService.update(this.coachId, { ...value, rowVersion: this.currentRowVersion })
        : this.coachService.create(value);

    request.subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/coaches']);
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
