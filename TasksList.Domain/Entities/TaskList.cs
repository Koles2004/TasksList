namespace TasksList.Domain.Entities;

public class TaskList
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string OwnerId { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public List<string> SharedUserIds { get; set; } = [];
}
