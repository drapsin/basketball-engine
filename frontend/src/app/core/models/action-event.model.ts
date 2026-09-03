export interface ActionEventDto {
  id: string;
  gameId: string;
  playerId: string;
  playerName: string;
  teamId: string;
  teamName: string;
  quarter: number;
  gameTime: string; // "hh:mm:ss"
  eventType: string;
  createdAt: string;
}

export interface ActionEventCreate {
  gameId: string;
  playerId: string;
  teamId: string;
  quarter: number;
  gameTime: string; // "hh:mm:ss"
  eventType: string;
}
