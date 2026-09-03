export interface ShootingSplit {
  made: number;
  attempted: number;
  percentage: number;
}

export interface PlayerCareerStats {
  playerId: string;
  playerName: string;
  teamName: string;
  gamesPlayed: number;

  totalPoints: number;
  pointsPerGame: number;

  totalRebounds: number;
  reboundsPerGame: number;

  totalAssists: number;
  assistsPerGame: number;

  totalSteals: number;
  stealsPerGame: number;

  totalBlocks: number;
  blocksPerGame: number;

  totalTurnovers: number;
  turnoversPerGame: number;

  freeThrows: ShootingSplit;
  twoPointers: ShootingSplit;
  threePointers: ShootingSplit;
}

export interface TeamCareerStats {
  teamId: string;
  teamName: string;
  gamesPlayed: number;

  totalPoints: number;
  pointsPerGame: number;

  totalRebounds: number;
  reboundsPerGame: number;

  totalAssists: number;
  assistsPerGame: number;

  totalSteals: number;
  stealsPerGame: number;

  totalBlocks: number;
  blocksPerGame: number;

  players: PlayerCareerStats[];
}

export type LeaderCategory = 'points' | 'rebounds' | 'assists' | 'steals' | 'blocks';

export interface LeagueLeader {
  playerId: string;
  playerName: string;
  teamName: string;
  gamesPlayed: number;
  value: number;
}
