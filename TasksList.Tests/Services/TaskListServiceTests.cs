using Moq;
using TasksList.Application.Exceptions;
using TasksList.Application.Repositories;
using TasksList.Application.Services;
using TasksList.Domain.Entities;

namespace TasksList.Tests.Services;

public class TaskListServiceTests
{
    private readonly Mock<ITaskListRepository> _repositoryMock;
    private readonly TaskListService _service;

    public TaskListServiceTests()
    {
        _repositoryMock = new Mock<ITaskListRepository>();
        _service = new TaskListService(_repositoryMock.Object);
    }
    
    // Helpers    

    private static TaskList MakeTaskList(string id = "list-1", string ownerId = "user-1",
        List<string>? sharedUserIds = null) =>
        new()
        {
            Id = id,
            Name = "My List",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            SharedUserIds = sharedUserIds ?? []
        };
    
    // CreateAsync    

    [Fact]
    public async Task CreateAsync_ReturnsCreatedTaskList()
    {
        var expected = MakeTaskList();
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _service.CreateAsync("My List", "user-1");

        Assert.Equal(expected, result);
        _repositoryMock.Verify(r => r.CreateAsync(
            It.Is<TaskList>(t => t.Name == "My List" && t.OwnerId == "user-1"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
    
    // GetByIdAsync    

    [Fact]
    public async Task GetByIdAsync_OwnerRequests_ReturnsTaskList()
    {
        var taskList = MakeTaskList();
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        var result = await _service.GetByIdAsync("list-1", "user-1");

        Assert.Equal(taskList, result);
    }

    [Fact]
    public async Task GetByIdAsync_SharedUserRequests_ReturnsTaskList()
    {
        var taskList = MakeTaskList(sharedUserIds: ["user-2"]);
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        var result = await _service.GetByIdAsync("list-1", "user-2");

        Assert.Equal(taskList, result);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync("missing", "user-1"));
    }

    [Fact]
    public async Task GetByIdAsync_UnauthorizedUser_ThrowsForbiddenException()
    {
        var taskList = MakeTaskList(ownerId: "user-1");
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetByIdAsync("list-1", "other-user"));
    }
    
    // GetAllByUserIdAsync    

    [Fact]
    public async Task GetAllByUserIdAsync_ReturnsPagedResult()
    {
        var items = new List<TaskList> { MakeTaskList(), MakeTaskList("list-2") };
        _repositoryMock
            .Setup(r => r.GetAllByUserIdAsync("user-1", 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<TaskList>)items, 2L));

        var result = await _service.GetAllByUserIdAsync("user-1", 1, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(items, result.Items);
    }
    
    // UpdateAsync    

    [Fact]
    public async Task UpdateAsync_OwnerUpdates_ReturnsUpdatedTaskList()
    {
        var taskList = MakeTaskList();
        var updated = MakeTaskList();
        updated.Name = "New Name";

        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var result = await _service.UpdateAsync("list-1", "New Name", "user-1");

        Assert.Equal("New Name", result.Name);
        _repositoryMock.Verify(r => r.UpdateAsync(
            It.Is<TaskList>(t => t.Name == "New Name"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UnauthorizedUser_ThrowsForbiddenException()
    {
        var taskList = MakeTaskList(ownerId: "user-1");
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.UpdateAsync("list-1", "New Name", "other-user"));
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateAsync("missing", "New Name", "user-1"));
    }
    
    // DeleteAsync    

    [Fact]
    public async Task DeleteAsync_OwnerDeletes_CallsRepository()
    {
        var taskList = MakeTaskList();
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await _service.DeleteAsync("list-1", "user-1");

        _repositoryMock.Verify(r => r.DeleteAsync("list-1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonOwnerRequests_ThrowsForbiddenException()
    {
        var taskList = MakeTaskList(ownerId: "user-1", sharedUserIds: ["user-2"]);
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.DeleteAsync("list-1", "user-2"));
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync("missing", "user-1"));
    }
    
    // AddSharedUserAsync    

    [Fact]
    public async Task AddSharedUserAsync_OwnerAddsUser_CallsRepository()
    {
        var taskList = MakeTaskList();
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await _service.AddSharedUserAsync("list-1", "user-2", "user-1");

        _repositoryMock.Verify(r => r.AddSharedUserAsync("list-1", "user-2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddSharedUserAsync_UnauthorizedUser_ThrowsForbiddenException()
    {
        var taskList = MakeTaskList(ownerId: "user-1");
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.AddSharedUserAsync("list-1", "user-3", "other-user"));
    }
    
    // GetSharedUsersAsync    

    [Fact]
    public async Task GetSharedUsersAsync_OwnerRequests_ReturnsSharedUserIds()
    {
        var taskList = MakeTaskList(sharedUserIds: ["user-2", "user-3"]);
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        var result = await _service.GetSharedUsersAsync("list-1", "user-1");

        Assert.Equal(2, result.Count);
        Assert.Contains("user-2", result);
        Assert.Contains("user-3", result);
    }

    [Fact]
    public async Task GetSharedUsersAsync_UnauthorizedUser_ThrowsForbiddenException()
    {
        var taskList = MakeTaskList(ownerId: "user-1");
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.GetSharedUsersAsync("list-1", "other-user"));
    }
        
    // RemoveSharedUserAsync    

    [Fact]
    public async Task RemoveSharedUserAsync_OwnerRemovesUser_CallsRepository()
    {
        var taskList = MakeTaskList(sharedUserIds: ["user-2"]);
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await _service.RemoveSharedUserAsync("list-1", "user-2", "user-1");

        _repositoryMock.Verify(r => r.RemoveSharedUserAsync("list-1", "user-2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveSharedUserAsync_UnauthorizedUser_ThrowsForbiddenException()
    {
        var taskList = MakeTaskList(ownerId: "user-1");
        _repositoryMock.Setup(r => r.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.RemoveSharedUserAsync("list-1", "user-2", "other-user"));
    }
}
