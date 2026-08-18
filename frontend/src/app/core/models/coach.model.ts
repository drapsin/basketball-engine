export interface Coach {
  id: string;
  firstName: string;
  lastName: string;
  age: number;
  history: string;
  teamId: string;
  teamName: string;
  imageUrl: string | null;
  createdAt: string;
}

export interface CoachCreate {
  firstName: string;
  lastName: string;
  age: number;
  history: string;
  teamId: string;
  imageUrl?: string | null;
}

export interface CoachUpdate extends CoachCreate {
  rowVersion?: string | null;
}
