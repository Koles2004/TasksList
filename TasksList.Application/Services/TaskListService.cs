using TasksList.Application.Exceptions;
using TasksList.Application.Models;
using TasksList.Application.Repositories;
using TasksList.Domain.Entities;

namespace TasksList.Application.Services;

public class TaskListService(ITaskListRepository repository) : ITaskListService
{
    public Task<TaskList> CreateAsync(string name, string ownerId, CancellationToken cancellationToken = default)
    {
        var taskList = new TaskList
        {
            Name = name,
            OwnerId = ownerId
        };

        return repository.CreateAsync(taskList, cancellationToken);
    }

    public async Task<TaskList> GetByIdAsync(string id, string requestingUserId, CancellationToken cancellationToken = default)
    {
        var taskList = await GetAndVerifyAccessAsync(id, requestingUserId, cancellationToken);

        return taskList;
    }

    public async Task<PagedResult<TaskList>> GetAllByUserIdAsync(string requestingUserId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await repository.GetAllByUserIdAsync(requestingUserId, page, pageSize, cancellationToken);

        return new PagedResult<TaskList>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TaskList> UpdateAsync(string id, string name, string requestingUserId, CancellationToken cancellationToken = default)
    {
        var taskList = await GetAndVerifyAccessAsync(id, requestingUserId, cancellationToken);
        taskList.Name = name;

        return await repository.UpdateAsync(taskList, cancellationToken);
    }

    public async Task DeleteAsync(string id, string requestingUserId, CancellationToken cancellationToken = default)
    {
        var taskList = await GetAndVerifyExistsAsync(id, cancellationToken);

        if (taskList.OwnerId != requestingUserId)
            throw new ForbiddenException("Only the owner can delete a task list.");

        await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task AddSharedUserAsync(string id, string userId, string requestingUserId, CancellationToken cancellationToken = default)
    {
        await GetAndVerifyAccessAsync(id, requestingUserId, cancellationToken);
        await repository.AddSharedUserAsync(id, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetSharedUsersAsync(string id, string requestingUserId, CancellationToken cancellationToken = default)
    {
        var taskList = await GetAndVerifyAccessAsync(id, requestingUserId, cancellationToken);

        return taskList.SharedUserIds;
    }

    public async Task RemoveSharedUserAsync(string id, string userId, string requestingUserId, CancellationToken cancellationToken = default)
    {
        await GetAndVerifyAccessAsync(id, requestingUserId, cancellationToken);
        await repository.RemoveSharedUserAsync(id, userId, cancellationToken);
    }

    private async Task<TaskList> GetAndVerifyExistsAsync(string id, CancellationToken cancellationToken)
    {
        var taskList = await repository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"Task list '{id}' not found.");

        return taskList;
    }

    private async Task<TaskList> GetAndVerifyAccessAsync(string id, string requestingUserId, CancellationToken cancellationToken)
    {
        var taskList = await GetAndVerifyExistsAsync(id, cancellationToken);

        if (taskList.OwnerId != requestingUserId && !taskList.SharedUserIds.Contains(requestingUserId))
            throw new ForbiddenException("You do not have access to this task list.");

        return taskList;
    }
}
