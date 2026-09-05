export type ApprovalStatus = 'Pending' | 'Approved' | 'Rejected';

export interface UserSummary {
  id: string;
  email: string;
  role: string;
  approvalStatus: ApprovalStatus;
  isSeededAdmin: boolean;
}

export interface CreateAdminRequest {
  email: string;
  password: string;
}
