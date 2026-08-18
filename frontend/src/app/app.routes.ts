import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { TeamList } from './features/teams/team-list/team-list';
import { TeamDetailComponent } from './features/teams/team-detail/team-detail';
import { TeamForm } from './features/teams/team-form/team-form';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'teams', component: TeamList },
  { path: 'teams/new', component: TeamForm, canActivate: [authGuard], data: { roles: ['Admin'] } },
  { path: 'teams/:id', component: TeamDetailComponent },
  {
    path: 'teams/:id/edit',
    component: TeamForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
];
