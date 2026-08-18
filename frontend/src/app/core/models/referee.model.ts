export interface Referee {
  id: string;
  firstName: string;
  lastName: string;
  age: number;
  experience: string;
  licence: string;
  imageUrl: string | null;
  createdAt: string;
}

export interface RefereeCreate {
  firstName: string;
  lastName: string;
  age: number;
  experience: string;
  licence: string;
  imageUrl?: string | null;
}

export interface RefereeUpdate extends RefereeCreate {
  rowVersion?: string | null;
}
