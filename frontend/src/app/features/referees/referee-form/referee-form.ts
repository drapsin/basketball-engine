import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { RefereeService } from '../../../core/services/referee.service';
import { Referee } from '../../../core/models/referee.model';

@Component({
  selector: 'app-referee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './referee-form.html',
  styleUrl: './referee-form.scss',
})
export class RefereeForm implements OnInit {
  private fb = inject(FormBuilder);
  private refereeService = inject(RefereeService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);

  private refereeId: string | null = null;
  private currentRowVersion: string | null = null;

  form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    age: [40, [Validators.required, Validators.min(0), Validators.max(100)]],
    experience: ['', Validators.required],
    licence: ['', Validators.required],
    imageUrl: [''],
  });

  ngOnInit(): void {
    this.refereeId = this.route.snapshot.paramMap.get('id');
    if (this.refereeId) {
      this.isEditMode.set(true);
      this.loading.set(true);
      this.refereeService.getById(this.refereeId).subscribe({
        next: (referee) => {
          this.form.patchValue({
            firstName: referee.firstName,
            lastName: referee.lastName,
            age: referee.age,
            experience: referee.experience,
            licence: referee.licence,
            imageUrl: referee.imageUrl ?? '',
          });
          this.currentRowVersion = null;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load referee.');
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

    const request: Observable<Referee | void> =
      this.isEditMode() && this.refereeId
        ? this.refereeService.update(this.refereeId, {
            ...value,
            rowVersion: this.currentRowVersion,
          })
        : this.refereeService.create(value);

    request.subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/referees']);
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
