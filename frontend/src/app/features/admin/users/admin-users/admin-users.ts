import { Component, OnInit, computed, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminUserService } from '../../../../core/services/admin-user.service';
import { ConfirmDialogService } from '../../../../shared/confirm-dialog/confirm-dialog.service';
import { UserSummary } from '../../../../core/models/user-summary.model';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-users.html',
})
export class AdminUsers implements OnInit {
  private adminUserService = inject(AdminUserService);
  private confirmDialog = inject(ConfirmDialogService);
  private fb = inject(FormBuilder);

  users = signal<UserSummary[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);
  actionInProgressId = signal<string | null>(null);

  pendingUsers = computed(() => this.users().filter((u) => u.approvalStatus === 'Pending'));
  allUsers = computed(() => [...this.users()].sort((a, b) => a.email.localeCompare(b.email)));

  createAdminForm: FormGroup;
  createAdminSubmitting = signal(false);
  createAdminError = signal<string | null>(null);
  createAdminSuccess = signal<string | null>(null);

  constructor() {
    this.createAdminForm = this.fb.nonNullable.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.adminUserService.getUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load users.');
        this.loading.set(false);
      },
    });
  }

  approve(user: UserSummary): void {
    this.actionInProgressId.set(user.id);
    this.adminUserService.approve(user.id).subscribe({
      next: () => {
        this.actionInProgressId.set(null);
        this.loadUsers();
      },
      error: () => {
        this.actionInProgressId.set(null);
        this.errorMessage.set(`Failed to approve ${user.email}.`);
      },
    });
  }

  reject(user: UserSummary): void {
    this.actionInProgressId.set(user.id);
    this.adminUserService.reject(user.id).subscribe({
      next: () => {
        this.actionInProgressId.set(null);
        this.loadUsers();
      },
      error: () => {
        this.actionInProgressId.set(null);
        this.errorMessage.set(`Failed to reject ${user.email}.`);
      },
    });
  }

  deleteUser(user: UserSummary): void {
    if (user.isSeededAdmin) {
      return;
    }
    this.confirmDialog
      .confirm(`Delete user ${user.email}? This cannot be undone.`, 'Delete User')
      .subscribe((confirmed) => {
        if (!confirmed) return;

        this.actionInProgressId.set(user.id);
        this.adminUserService.deleteUser(user.id).subscribe({
          next: () => {
            this.actionInProgressId.set(null);
            this.loadUsers();
          },
          error: () => {
            this.actionInProgressId.set(null);
            this.errorMessage.set(`Failed to delete ${user.email}.`);
          },
        });
      });
  }

  submitCreateAdmin(): void {
    if (this.createAdminForm.invalid) {
      this.createAdminForm.markAllAsTouched();
      return;
    }
    this.createAdminSubmitting.set(true);
    this.createAdminError.set(null);
    this.createAdminSuccess.set(null);

    this.adminUserService.createAdmin(this.createAdminForm.getRawValue()).subscribe({
      next: () => {
        this.createAdminSubmitting.set(false);
        this.createAdminSuccess.set('Admin account created.');
        this.createAdminForm.reset();
        this.loadUsers();
      },
      error: (err) => {
        this.createAdminSubmitting.set(false);
        this.createAdminError.set(err?.error?.message ?? 'Failed to create admin account.');
      },
    });
  }
}
