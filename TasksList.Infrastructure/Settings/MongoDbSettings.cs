namespace TasksList.Infrastructure.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = default!;

    public string DatabaseName { get; set; } = default!;

    public string TaskListsCollectionName { get; set; } = default!;
}
