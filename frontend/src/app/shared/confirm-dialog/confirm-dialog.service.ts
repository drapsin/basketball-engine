import { Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';

interface ConfirmState {
  title: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  state = signal<ConfirmState | null>(null);
  private resultSubject: Subject<boolean> | null = null;

  confirm(message: string, title = 'Confirm'): Subject<boolean> {
    this.resultSubject = new Subject<boolean>();
    this.state.set({ title, message });
    return this.resultSubject;
  }

  respond(confirmed: boolean): void {
    this.resultSubject?.next(confirmed);
    this.resultSubject?.complete();
    this.resultSubject = null;
    this.state.set(null);
  }
}
