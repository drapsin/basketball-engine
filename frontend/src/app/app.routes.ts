import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { TeamList } from './features/teams/team-list/team-list';
import { TeamDetailComponent } from './features/teams/team-detail/team-detail';
import { TeamForm } from './features/teams/team-form/team-form';
import { PlayerDetailComponent } from './features/players/player-detail/player-detail';
import { PlayerList } from './features/players/player-list/player-list';
import { PlayerForm } from './features/players/player-form/player-form';
import { ArenaList } from './features/arenas/arena-list/arena-list';
import { ArenaForm } from './features/arenas/arena-form/arena-form';
import { authGuard } from './core/guards/auth.guard';
import { CoachList } from './features/coaches/coach-list/coach-list';
import { CoachForm } from './features/coaches/coach-form/coach-form';
import { RefereeList } from './features/referees/referee-list/referee-list';
import { RefereeForm } from './features/referees/referee-form/referee-form';
import { GameList } from './features/games/game-list/game-list';
import { GameDetailComponent } from './features/games/game-detail/game-detail';
import { GameForm } from './features/games/game-form/game-form';
import { LiveGame } from './features/live-game/live-game/live-game';

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
  { path: 'players', component: PlayerList },
  {
    path: 'players/new',
    component: PlayerForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  { path: 'players/:id', component: PlayerDetailComponent },
  {
    path: 'players/:id/edit',
    component: PlayerForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  { path: 'arenas', component: ArenaList },
  {
    path: 'arenas/new',
    component: ArenaForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'arenas/:id/edit',
    component: ArenaForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  { path: 'coaches', component: CoachList },
  {
    path: 'coaches/new',
    component: CoachForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'coaches/:id/edit',
    component: CoachForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  { path: 'referees', component: RefereeList },
  {
    path: 'referees/new',
    component: RefereeForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'referees/:id/edit',
    component: RefereeForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  { path: 'games', component: GameList },
  { path: 'games/new', component: GameForm, canActivate: [authGuard], data: { roles: ['Admin'] } },
  { path: 'games/:id', component: GameDetailComponent },
  {
    path: 'games/:id/edit',
    component: GameForm,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  { path: 'games/:id/live', component: LiveGame },
];
