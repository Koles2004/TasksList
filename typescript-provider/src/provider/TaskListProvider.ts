import { AxiosInstance } from 'axios';
import {
  AddSharedUserRequest,
  CreateTaskListRequest,
  PagedResponse,
  TaskList,
  TaskListSummary,
  UpdateTaskListRequest,
} from '../types';

export interface GetPagedParams {
  userId: string;
  page?: number;
  pageSize?: number;
}

export class TaskListProvider {
  constructor(private readonly client: AxiosInstance) {}

  async create(request: CreateTaskListRequest): Promise<TaskList> {
    const { data } = await this.client.post<TaskList>('/task-lists', request);
    return data;
  }

  async getById(id: string, userId: string): Promise<TaskList> {
    const { data } = await this.client.get<TaskList>(`/task-lists/${id}`, {
      params: { userId },
    });
    return data;
  }

  async getPaged(params: GetPagedParams): Promise<PagedResponse<TaskListSummary>> {
    const { data } = await this.client.get<PagedResponse<TaskListSummary>>('/task-lists', {
      params: {
        userId: params.userId,
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 20,
      },
    });
    return data;
  }

  async update(id: string, userId: string, request: UpdateTaskListRequest): Promise<TaskList> {
    const { data } = await this.client.put<TaskList>(`/task-lists/${id}`, request, {
      params: { userId },
    });
    return data;
  }

  async delete(id: string, userId: string): Promise<void> {
    await this.client.delete(`/task-lists/${id}`, {
      params: { userId },
    });
  }

  async addSharedUser(id: string, userId: string, request: AddSharedUserRequest): Promise<void> {
    await this.client.post(`/task-lists/${id}/shares`, request, {
      params: { userId },
    });
  }

  async getSharedUsers(id: string, userId: string): Promise<string[]> {
    const { data } = await this.client.get<string[]>(`/task-lists/${id}/shares`, {
      params: { userId },
    });
    return data;
  }

  async removeSharedUser(id: string, sharedUserId: string, userId: string): Promise<void> {
    await this.client.delete(`/task-lists/${id}/shares/${sharedUserId}`, {
      params: { userId },
    });
  }
}
