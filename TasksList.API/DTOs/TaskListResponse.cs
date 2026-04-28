namespace TasksList.API.DTOs;

public record TaskListResponse(
    string Id,
    string Name,
    string OwnerId,
    DateTime CreatedAt,
    List<string> SharedUserIds);
