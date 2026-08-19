import { Referee } from './referee.model';
import { Player } from './player.model';

export interface Game {
  id: string;
  gameDate: string; // ISO date string
  gameName: string;
  gameTime: string;
  gameResult: string | null;
  sponsor: string;
  homeTeamId: string;
  homeTeamName: string;
  awayTeamId: string;
  awayTeamName: string;
  arenaId: string;
  arenaName: string;
  createdAt: string;
}

export interface GameDetail extends Game {
  referees: Referee[];
  players: Player[];
}

export interface GameCreate {
  gameDate: string;
  gameName: string;
  gameTime: string;
  sponsor: string;
  homeTeamId: string;
  awayTeamId: string;
  arenaId: string;
  refereeIds: string[];
  playerIds: string[];
}

export interface GameUpdate extends GameCreate {
  gameResult?: string | null;
  rowVersion?: string | null;
}
