import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResult, LoginRequest, RegisterRequest } from '../models/auth.model';

const TOKEN_KEY = 'nba_auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private authState = signal<AuthResult | null>(this.loadFromStorage());

  currentAuth = computed(() => this.authState());
  isLoggedIn = computed(() => !!this.authState());
  currentRole = computed(() => this.authState()?.role ?? null);

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {}

  login(request: LoginRequest): Observable<AuthResult> {
    return this.http
      .post<AuthResult>(`${environment.apiUrl}/Auth/login`, request)
      .pipe(tap((result) => this.setAuth(result)));
  }

  register(request: RegisterRequest): Observable<AuthResult> {
    return this.http
      .post<AuthResult>(`${environment.apiUrl}/Auth/register`, request)
      .pipe(tap((result) => this.setAuth(result)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.authState.set(null);
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    return this.authState()?.token ?? null;
  }

  hasRole(...roles: string[]): boolean {
    const current = this.authState()?.role;
    return !!current && roles.includes(current);
  }

  private setAuth(result: AuthResult): void {
    localStorage.setItem(TOKEN_KEY, JSON.stringify(result));
    this.authState.set(result);
  }

  private loadFromStorage(): AuthResult | null {
    const raw = localStorage.getItem(TOKEN_KEY);
    if (!raw) return null;
    const parsed: AuthResult = JSON.parse(raw);
    if (new Date(parsed.expiresAt) <= new Date()) {
      localStorage.removeItem(TOKEN_KEY);
      return null;
    }
    return parsed;
  }
}
