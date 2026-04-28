import { AxiosInstance } from 'axios';
import { AddSharedUserRequest, CreateTaskListRequest, PagedResponse, TaskList, TaskListSummary, UpdateTaskListRequest } from '../types';
export interface GetPagedParams {
    userId: string;
    page?: number;
    pageSize?: number;
}
export declare class TaskListProvider {
    private readonly client;
    constructor(client: AxiosInstance);
    create(request: CreateTaskListRequest): Promise<TaskList>;
    getById(id: string, userId: string): Promise<TaskList>;
    getPaged(params: GetPagedParams): Promise<PagedResponse<TaskListSummary>>;
    update(id: string, userId: string, request: UpdateTaskListRequest): Promise<TaskList>;
    delete(id: string, userId: string): Promise<void>;
    addSharedUser(id: string, userId: string, request: AddSharedUserRequest): Promise<void>;
    getSharedUsers(id: string, userId: string): Promise<string[]>;
    removeSharedUser(id: string, sharedUserId: string, userId: string): Promise<void>;
}
//# sourceMappingURL=TaskListProvider.d.ts.map