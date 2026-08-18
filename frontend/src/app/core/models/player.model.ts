export interface Player {
  id: string;
  firstName: string;
  lastName: string;
  age: number;
  position: string;
  teamId: string;
  teamName: string;
  height: number;
  weight: number;
  agent: string;
  sponsor: string;
  news: string;
  imageUrl: string | null;
  createdAt: string;
}

export interface PlayerCreate {
  firstName: string;
  lastName: string;
  age: number;
  position: string;
  teamId: string;
  height: number;
  weight: number;
  agent: string;
  sponsor: string;
  news: string;
  imageUrl?: string | null;
}

export interface PlayerUpdate extends PlayerCreate {
  rowVersion?: string | null;
}
