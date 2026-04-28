using TasksList.Application.Models;
using TasksList.Domain.Entities;

namespace TasksList.Application.Services;

public interface ITaskListService
{
    Task<TaskList> CreateAsync(string name, string ownerId, CancellationToken cancellationToken = default);

    Task<TaskList> GetByIdAsync(string id, string requestingUserId, CancellationToken cancellationToken = default);

    Task<PagedResult<TaskList>> GetAllByUserIdAsync(string requestingUserId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<TaskList> UpdateAsync(string id, string name, string requestingUserId, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, string requestingUserId, CancellationToken cancellationToken = default);

    Task AddSharedUserAsync(string id, string userId, string requestingUserId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetSharedUsersAsync(string id, string requestingUserId, CancellationToken cancellationToken = default);

    Task RemoveSharedUserAsync(string id, string userId, string requestingUserId, CancellationToken cancellationToken = default);
}
