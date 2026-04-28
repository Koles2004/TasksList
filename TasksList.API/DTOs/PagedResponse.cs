namespace TasksList.API.DTOs;

public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    long TotalCount,
    int Page,
    int PageSize);
