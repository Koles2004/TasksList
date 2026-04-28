using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TasksList.Domain.Entities;

namespace TasksList.Infrastructure.Documents;

internal class TaskListDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string OwnerId { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public List<string> SharedUserIds { get; set; } = [];

    public static TaskListDocument FromDomain(TaskList taskList) => new()
    {
        Id = taskList.Id,
        Name = taskList.Name,
        OwnerId = taskList.OwnerId,
        CreatedAt = taskList.CreatedAt,
        SharedUserIds = taskList.SharedUserIds
    };

    public TaskList ToDomain() => new()
    {
        Id = Id,
        Name = Name,
        OwnerId = OwnerId,
        CreatedAt = CreatedAt,
        SharedUserIds = SharedUserIds
    };
}
