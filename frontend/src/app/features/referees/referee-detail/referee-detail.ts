import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RefereeService } from '../../../core/services/referee.service';
import { AuthService } from '../../../core/services/auth.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { Referee } from '../../../core/models/referee.model';

@Component({
  selector: 'app-referee-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './referee-detail.html',
  styleUrl: './referee-detail.scss',
})
export class RefereeDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private refereeService = inject(RefereeService);
  private confirmDialog = inject(ConfirmDialogService);
  public authService = inject(AuthService);

  referee = signal<Referee | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  deleting = signal(false);

  private refereeId: string | null = null;

  ngOnInit(): void {
    this.refereeId = this.route.snapshot.paramMap.get('id');
    if (!this.refereeId) {
      this.error.set('No referee ID provided.');
      this.loading.set(false);
      return;
    }

    this.refereeService.getById(this.refereeId).subscribe({
      next: (referee) => {
        this.referee.set(referee);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load referee.');
        this.loading.set(false);
      },
    });
  }

  onDelete(): void {
    if (!this.refereeId) return;

    const r = this.referee();
    const label = r ? `${r.firstName} ${r.lastName}` : 'this referee';

    this.confirmDialog.confirm(`Delete ${label}?`, 'Delete Referee').subscribe((confirmed) => {
      if (!confirmed) return;

      this.deleting.set(true);
      this.refereeService.delete(this.refereeId!).subscribe({
        next: () => {
          this.router.navigate(['/referees']);
        },
        error: () => {
          this.deleting.set(false);
          this.error.set('Failed to delete referee.');
        },
      });
    });
  }
}
