import { Conference, Division } from './enums';
import { Player } from './player.model';

export interface Team {
  id: string;
  name: string;
  city: string;
  site: string;
  sponsor: string;
  news: string;
  ranking: string;
  contact: string;
  conference: Conference;
  division: Division;
  arenaId: string;
  arenaName: string;
  playerCount: number;
  imageUrl: string | null;
  createdAt: string;
}

export interface TeamDetail extends Team {
  players: Player[];
}

export interface TeamCreate {
  name: string;
  city: string;
  site: string;
  sponsor: string;
  news: string;
  ranking: string;
  contact: string;
  conference: Conference;
  division: Division;
  arenaId: string;
  imageUrl?: string | null;
}

export interface TeamUpdate extends TeamCreate {
  rowVersion?: string | null;
}
