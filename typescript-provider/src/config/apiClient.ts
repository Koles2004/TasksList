import axios, { AxiosInstance, AxiosError } from 'axios';
import { ApiError } from '../errors/ApiError';

export interface ApiClientConfig {
  baseUrl: string;
  timeoutMs?: number;
}

export function createApiClient(config: ApiClientConfig): AxiosInstance {
  const client = axios.create({
    baseURL: config.baseUrl,
    timeout: config.timeoutMs ?? 10000,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  client.interceptors.response.use(
    (response) => response,
    (error: AxiosError<{ error?: string }>) => {
      const statusCode = error.response?.status ?? 500;
      const message =
        error.response?.data?.error ?? error.message ?? 'Unknown error';
      throw new ApiError(statusCode, message);
    }
  );

  return client;
}
