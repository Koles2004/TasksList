using Microsoft.AspNetCore.Mvc;
using TasksList.API.DTOs;
using TasksList.Application.Services;
using TasksList.Domain.Entities;

namespace TasksList.API.Controllers;

[ApiController]
[Route("task-lists")]
public class TaskListsController(ITaskListService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TaskListResponse>> Create(
        [FromBody] CreateTaskListRequest request,
        CancellationToken cancellationToken)
    {
        var taskList = await service.CreateAsync(request.Name, request.OwnerId, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = taskList.Id, userId = request.OwnerId }, ToResponse(taskList));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TaskListSummaryResponse>>> GetAllByUserId(
        [FromQuery] string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllByUserIdAsync(userId, page, pageSize, cancellationToken);
        var response = new PagedResponse<TaskListSummaryResponse>(
            result.Items.Select(x => new TaskListSummaryResponse(x.Id, x.Name)).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskListResponse>> GetById(
        string id,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        var taskList = await service.GetByIdAsync(id, userId, cancellationToken);

        return Ok(ToResponse(taskList));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskListResponse>> Update(
        string id,
        [FromBody] UpdateTaskListRequest request,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        var taskList = await service.UpdateAsync(id, request.Name, userId, cancellationToken);

        return Ok(ToResponse(taskList));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, userId, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id}/shares")]
    public async Task<IActionResult> AddSharedUser(
        string id,
        [FromBody] AddSharedUserRequest request,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        await service.AddSharedUserAsync(id, request.UserId, userId, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id}/shares")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetSharedUsers(
        string id,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        var users = await service.GetSharedUsersAsync(id, userId, cancellationToken);

        return Ok(users);
    }

    [HttpDelete("{id}/shares/{sharedUserId}")]
    public async Task<IActionResult> RemoveSharedUser(
        string id,
        string sharedUserId,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        await service.RemoveSharedUserAsync(id, sharedUserId, userId, cancellationToken);

        return NoContent();
    }

    private static TaskListResponse ToResponse(TaskList taskList) =>
        new(taskList.Id, taskList.Name, taskList.OwnerId, taskList.CreatedAt, taskList.SharedUserIds);
}
