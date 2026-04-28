export interface TaskList {
    id: string;
    name: string;
    ownerId: string;
    createdAt: string;
    sharedUserIds: string[];
}
export interface TaskListSummary {
    id: string;
    name: string;
}
export interface PagedResponse<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
}
export interface CreateTaskListRequest {
    name: string;
    ownerId: string;
}
export interface UpdateTaskListRequest {
    name: string;
}
export interface AddSharedUserRequest {
    userId: string;
}
//# sourceMappingURL=index.d.ts.map