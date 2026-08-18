import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ArenaService } from '../../../core/services/arena.service';
import { Arena } from '../../../core/models/arena.model';

@Component({
  selector: 'app-arena-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './arena-form.html',
  styleUrl: './arena-form.scss',
})
export class ArenaForm implements OnInit {
  private fb = inject(FormBuilder);
  private arenaService = inject(ArenaService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);

  private arenaId: string | null = null;
  private currentRowVersion: string | null = null;

  form = this.fb.nonNullable.group({
    arenaName: ['', Validators.required],
    arenaLocation: ['', Validators.required],
    capacity: [15000, [Validators.required, Validators.min(1), Validators.max(100000)]],
  });

  ngOnInit(): void {
    this.arenaId = this.route.snapshot.paramMap.get('id');
    if (this.arenaId) {
      this.isEditMode.set(true);
      this.loading.set(true);
      this.arenaService.getById(this.arenaId).subscribe({
        next: (arena) => {
          this.form.patchValue({
            arenaName: arena.arenaName,
            arenaLocation: arena.arenaLocation,
            capacity: arena.capacity,
          });
          this.currentRowVersion = null;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load arena.');
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

    const request: Observable<Arena | void> =
      this.isEditMode() && this.arenaId
        ? this.arenaService.update(this.arenaId, { ...value, rowVersion: this.currentRowVersion })
        : this.arenaService.create(value);

    request.subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/arenas']);
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
