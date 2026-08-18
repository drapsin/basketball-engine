import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CoachService } from '../../../core/services/coach.service';
import { AuthService } from '../../../core/services/auth.service';
import { Coach } from '../../../core/models/coach.model';

@Component({
  selector: 'app-coach-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './coach-list.html',
  styleUrl: './coach-list.scss',
})
export class CoachList implements OnInit {
  private coachService = inject(CoachService);
  public authService = inject(AuthService);

  coaches = signal<Coach[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  deletingId = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.coachService.getAll().subscribe({
      next: (coaches) => {
        this.coaches.set(coaches);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load coaches. Is the backend running?');
        this.loading.set(false);
      },
    });
  }

  onDelete(coach: Coach): void {
    const confirmed = window.confirm(`Delete ${coach.firstName} ${coach.lastName}?`);
    if (!confirmed) return;

    this.deletingId.set(coach.id);
    this.coachService.delete(coach.id).subscribe({
      next: () => {
        this.coaches.update((list) => list.filter((c) => c.id !== coach.id));
        this.deletingId.set(null);
      },
      error: () => {
        this.error.set('Failed to delete coach.');
        this.deletingId.set(null);
      },
    });
  }
}
