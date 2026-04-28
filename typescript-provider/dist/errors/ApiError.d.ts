export declare class ApiError extends Error {
    readonly statusCode: number;
    constructor(statusCode: number, message: string);
    get isNotFound(): boolean;
    get isForbidden(): boolean;
    get isServerError(): boolean;
}
//# sourceMappingURL=ApiError.d.ts.map