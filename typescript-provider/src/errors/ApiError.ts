export class ApiError extends Error {
  constructor(
    public readonly statusCode: number,
    message: string
  ) {
    super(message);
    this.name = 'ApiError';
  }

  get isNotFound(): boolean {
    return this.statusCode === 404;
  }

  get isForbidden(): boolean {
    return this.statusCode === 403;
  }

  get isServerError(): boolean {
    return this.statusCode >= 500;
  }
}
