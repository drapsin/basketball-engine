import { EventType } from './event-type.enum';

export interface ShootingSplit {
  made: number;
  attempted: number;
  percentage: number;
}

export interface PlayerBoxScore {
  playerId: string;
  playerName: string;
  position: string;
  minutesPlayed: string;

  points: number;
  offensiveRebounds: number;
  defensiveRebounds: number;
  rebounds: number;
  assists: number;
  steals: number;
  blocks: number;
  turnovers: number;
  personalFouls: number;

  freeThrows: ShootingSplit;
  twoPointers: ShootingSplit;
  threePointers: ShootingSplit;
}

export interface PlayByPlayEntry {
  quarter: number;
  gameTime: string; // "hh:mm:ss" — TimeSpan serialized by System.Text.Json
  playerName: string;
  teamName: string;
  eventType: EventType;
  runningHomeScore: number;
  runningAwayScore: number;
}

export interface GameState {
  gameId: string;
  quarter: number;
  gameClock: string; // "hh:mm:ss"
  homeScore: number;
  awayScore: number;
  lastEvent: PlayByPlayEntry | null;
  homeTeamStats: PlayerBoxScore[];
  awayTeamStats: PlayerBoxScore[];
}
