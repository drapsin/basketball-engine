import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CoachService } from '../../../core/services/coach.service';
import { AuthService } from '../../../core/services/auth.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { Coach } from '../../../core/models/coach.model';

@Component({
  selector: 'app-coach-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './coach-detail.html',
  styleUrl: './coach-detail.scss',
})
export class CoachDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private coachService = inject(CoachService);
  private confirmDialog = inject(ConfirmDialogService);
  public authService = inject(AuthService);

  coach = signal<Coach | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  deleting = signal(false);

  private coachId: string | null = null;

  ngOnInit(): void {
    this.coachId = this.route.snapshot.paramMap.get('id');
    if (!this.coachId) {
      this.error.set('No coach ID provided.');
      this.loading.set(false);
      return;
    }

    this.coachService.getById(this.coachId).subscribe({
      next: (coach) => {
        this.coach.set(coach);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load coach.');
        this.loading.set(false);
      },
    });
  }

  onDelete(): void {
    if (!this.coachId) return;

    const c = this.coach();
    const label = c ? `${c.firstName} ${c.lastName}` : 'this coach';

    this.confirmDialog.confirm(`Delete ${label}?`, 'Delete Coach').subscribe((confirmed) => {
      if (!confirmed) return;

      this.deleting.set(true);
      this.coachService.delete(this.coachId!).subscribe({
        next: () => {
          this.router.navigate(['/coaches']);
        },
        error: () => {
          this.deleting.set(false);
          this.error.set('Failed to delete coach.');
        },
      });
    });
  }
}
