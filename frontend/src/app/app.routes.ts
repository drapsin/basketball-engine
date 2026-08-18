import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { TeamList } from './features/teams/team-list/team-list';
import { Home } from './features/home/home';
import { TeamDetailComponent } from './features/teams/team-detail/team-detail';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'teams', component: TeamList },
  { path: 'teams/:id', component: TeamDetailComponent },
];
