"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ApiError = void 0;
class ApiError extends Error {
    constructor(statusCode, message) {
        super(message);
        this.statusCode = statusCode;
        this.name = 'ApiError';
    }
    get isNotFound() {
        return this.statusCode === 404;
    }
    get isForbidden() {
        return this.statusCode === 403;
    }
    get isServerError() {
        return this.statusCode >= 500;
    }
}
exports.ApiError = ApiError;
//# sourceMappingURL=ApiError.js.map