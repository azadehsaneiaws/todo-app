using TodoApp.Api.Contracts;
using TodoApp.Api.Repositories;
using TodoApp.Api.Services;

namespace TodoApp.Api.Tests.Services;

public class TodoServiceTests
{
    // The in-memory repository is trivial and side-effect free, so the tests
    // exercise the service against the real thing rather than a mock.
    private static TodoService CreateService() => new(new InMemoryTodoRepository());

    [Fact]
    public void GetAll_OnAFreshList_IsEmpty()
    {
        var service = CreateService();

        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void Create_AddsATodoThatCanBeReadBack()
    {
        var service = CreateService();

        var created = service.Create(new CreateTodoRequest { Title = "Book flights" });

        var fetched = service.GetById(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Book flights", fetched!.Title);
        Assert.False(fetched.IsCompleted);
    }

    [Fact]
    public void GetById_ForUnknownId_ReturnsNull()
    {
        var service = CreateService();

        Assert.Null(service.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void Update_ChangesTitleAndCompletedState()
    {
        var service = CreateService();
        var created = service.Create(new CreateTodoRequest { Title = "Clean desk" });

        var updated = service.Update(
            created.Id,
            new UpdateTodoRequest { Title = "Clean the whole office", IsCompleted = true });

        Assert.NotNull(updated);
        Assert.Equal("Clean the whole office", updated!.Title);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public void Update_ForUnknownId_ReturnsNull()
    {
        var service = CreateService();

        var result = service.Update(
            Guid.NewGuid(),
            new UpdateTodoRequest { Title = "Does not matter" });

        Assert.Null(result);
    }

    [Fact]
    public void Delete_RemovesTheTodo()
    {
        var service = CreateService();
        var created = service.Create(new CreateTodoRequest { Title = "Temporary" });

        var deleted = service.Delete(created.Id);

        Assert.True(deleted);
        Assert.Null(service.GetById(created.Id));
    }

    [Fact]
    public void Delete_ForUnknownId_ReturnsFalse()
    {
        var service = CreateService();

        Assert.False(service.Delete(Guid.NewGuid()));
    }
}
