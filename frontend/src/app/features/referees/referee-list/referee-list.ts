import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RefereeService } from '../../../core/services/referee.service';
import { AuthService } from '../../../core/services/auth.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { Referee } from '../../../core/models/referee.model';

@Component({
  selector: 'app-referee-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './referee-list.html',
  styleUrl: './referee-list.scss',
})
export class RefereeList implements OnInit {
  private refereeService = inject(RefereeService);
  private confirmDialog = inject(ConfirmDialogService);
  public authService = inject(AuthService);

  referees = signal<Referee[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  deletingId = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.refereeService.getAll().subscribe({
      next: (referees) => {
        this.referees.set(referees);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load referees. Is the backend running?');
        this.loading.set(false);
      },
    });
  }

  onDelete(referee: Referee): void {
    this.confirmDialog
      .confirm(`Delete ${referee.firstName} ${referee.lastName}?`, 'Delete Referee')
      .subscribe((confirmed) => {
        if (!confirmed) return;

        this.deletingId.set(referee.id);
        this.refereeService.delete(referee.id).subscribe({
          next: () => {
            this.referees.update((list) => list.filter((r) => r.id !== referee.id));
            this.deletingId.set(null);
          },
          error: () => {
            this.error.set('Failed to delete referee.');
            this.deletingId.set(null);
          },
        });
      });
  }
}
