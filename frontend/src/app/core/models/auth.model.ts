export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  role: 'Manager';
}

export interface RegisterResponse {
  message: string;
}

export interface AuthResult {
  token: string;
  email: string;
  role: string;
  expiresAt: string;
}
