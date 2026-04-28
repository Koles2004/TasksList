using TasksList.Domain.Entities;

namespace TasksList.Application.Repositories;

public interface ITaskListRepository
{
    Task<TaskList> CreateAsync(TaskList taskList, CancellationToken cancellationToken = default);

    Task<TaskList?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<TaskList> Items, long TotalCount)> GetAllByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<TaskList> UpdateAsync(TaskList taskList, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task AddSharedUserAsync(string id, string userId, CancellationToken cancellationToken = default);

    Task RemoveSharedUserAsync(string id, string userId, CancellationToken cancellationToken = default);
}
