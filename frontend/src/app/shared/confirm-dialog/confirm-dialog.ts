import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmDialogService } from './confirm-dialog.service';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss',
})
export class ConfirmDialog {
  dialogService = inject(ConfirmDialogService);

  onConfirm(): void {
    this.dialogService.respond(true);
  }

  onCancel(): void {
    this.dialogService.respond(false);
  }
}
