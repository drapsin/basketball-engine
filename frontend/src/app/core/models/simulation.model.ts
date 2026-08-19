export interface SimulationStatus {
  gameId: string;
  isPaused: boolean;
  quarter: number;
  gameClock: string; // "hh:mm:ss"
}
