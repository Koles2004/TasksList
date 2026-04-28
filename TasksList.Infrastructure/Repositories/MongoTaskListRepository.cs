using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TasksList.Application.Repositories;
using TasksList.Domain.Entities;
using TasksList.Infrastructure.Documents;
using TasksList.Infrastructure.Settings;

namespace TasksList.Infrastructure.Repositories;

public class MongoTaskListRepository : ITaskListRepository
{
    private readonly IMongoCollection<TaskListDocument> _collection;

    public MongoTaskListRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<TaskListDocument>(settings.Value.TaskListsCollectionName);
    }

    public async Task<TaskList> CreateAsync(TaskList taskList, CancellationToken cancellationToken = default)
    {
        var doc = TaskListDocument.FromDomain(taskList);
        doc.Id = ObjectId.GenerateNewId().ToString();
        doc.CreatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(doc, cancellationToken: cancellationToken);

        return doc.ToDomain();
    }

    public async Task<TaskList?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var doc = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

        return doc?.ToDomain();
    }

    public async Task<(IReadOnlyList<TaskList> Items, long TotalCount)> GetAllByUserIdAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TaskListDocument>.Filter.Or(
            Builders<TaskListDocument>.Filter.Eq(x => x.OwnerId, userId),
            Builders<TaskListDocument>.Filter.AnyEq(x => x.SharedUserIds, userId)
        );

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await _collection
            .Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return (items.Select(x => x.ToDomain()).ToList(), totalCount);
    }

    public async Task<TaskList> UpdateAsync(TaskList taskList, CancellationToken cancellationToken = default)
    {
        var update = Builders<TaskListDocument>.Update
            .Set(x => x.Name, taskList.Name);

        await _collection.UpdateOneAsync(x => x.Id == taskList.Id, update, cancellationToken: cancellationToken);

        return (await GetByIdAsync(taskList.Id, cancellationToken))!;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddSharedUserAsync(string id, string userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<TaskListDocument>.Update.AddToSet(x => x.SharedUserIds, userId);
        await _collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }

    public async Task RemoveSharedUserAsync(string id, string userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<TaskListDocument>.Update.Pull(x => x.SharedUserIds, userId);
        await _collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }
}
