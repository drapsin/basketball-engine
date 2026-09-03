import { Conference, Division } from './enums';

export interface TeamStanding {
  conferenceRank: number;
  divisionRank: number;
  teamId: string;
  teamName: string;
  logoUrl: string | null;
  conference: Conference;
  division: Division;
  wins: number;
  losses: number;
  winPercentage: number;
  streak: string;
  isProjected: boolean;
}
