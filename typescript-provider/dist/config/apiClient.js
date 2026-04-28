"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.createApiClient = createApiClient;
const axios_1 = __importDefault(require("axios"));
const ApiError_1 = require("../errors/ApiError");
function createApiClient(config) {
    const client = axios_1.default.create({
        baseURL: config.baseUrl,
        timeout: config.timeoutMs ?? 10000,
        headers: {
            'Content-Type': 'application/json',
        },
    });
    client.interceptors.response.use((response) => response, (error) => {
        const statusCode = error.response?.status ?? 500;
        const message = error.response?.data?.error ?? error.message ?? 'Unknown error';
        throw new ApiError_1.ApiError(statusCode, message);
    });
    return client;
}
//# sourceMappingURL=apiClient.js.map