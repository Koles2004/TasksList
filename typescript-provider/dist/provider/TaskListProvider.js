"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TaskListProvider = void 0;
class TaskListProvider {
    constructor(client) {
        this.client = client;
    }
    async create(request) {
        const { data } = await this.client.post('/task-lists', request);
        return data;
    }
    async getById(id, userId) {
        const { data } = await this.client.get(`/task-lists/${id}`, {
            params: { userId },
        });
        return data;
    }
    async getPaged(params) {
        const { data } = await this.client.get('/task-lists', {
            params: {
                userId: params.userId,
                page: params.page ?? 1,
                pageSize: params.pageSize ?? 20,
            },
        });
        return data;
    }
    async update(id, userId, request) {
        const { data } = await this.client.put(`/task-lists/${id}`, request, {
            params: { userId },
        });
        return data;
    }
    async delete(id, userId) {
        await this.client.delete(`/task-lists/${id}`, {
            params: { userId },
        });
    }
    async addSharedUser(id, userId, request) {
        await this.client.post(`/task-lists/${id}/shares`, request, {
            params: { userId },
        });
    }
    async getSharedUsers(id, userId) {
        const { data } = await this.client.get(`/task-lists/${id}/shares`, {
            params: { userId },
        });
        return data;
    }
    async removeSharedUser(id, sharedUserId, userId) {
        await this.client.delete(`/task-lists/${id}/shares/${sharedUserId}`, {
            params: { userId },
        });
    }
}
exports.TaskListProvider = TaskListProvider;
//# sourceMappingURL=TaskListProvider.js.map