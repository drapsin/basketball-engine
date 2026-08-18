export interface Arena {
  id: string;
  arenaName: string;
  arenaLocation: string;
  capacity: number;
  createdAt: string;
}

export interface ArenaCreate {
  arenaName: string;
  arenaLocation: string;
  capacity: number;
}

export interface ArenaUpdate extends ArenaCreate {
  rowVersion?: string | null;
}
