using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Api.Contracts;

namespace TodoApp.Api.Tests.Api;

/// <summary>
/// End-to-end tests that drive the real HTTP pipeline (routing, model
/// validation, status codes, JSON serialisation) via an in-memory test server.
/// A fresh factory per test gives each one a clean in-memory store.
/// </summary>
public class TodosApiTests
{
    private static WebApplicationFactory<Program> CreateApi() => new();

    [Fact]
    public async Task GetAll_OnAFreshServer_ReturnsEmptyList()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        var todos = await client.GetFromJsonAsync<List<TodoResponse>>("/api/todos");

        Assert.NotNull(todos);
        Assert.Empty(todos!);
    }

    [Fact]
    public async Task Post_CreatesTodo_AndReturns201WithLocation()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/todos",
            new CreateTodoRequest { Title = "Write README" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);
        Assert.Equal("Write README", created!.Title);
        Assert.NotEqual(Guid.Empty, created.Id);
    }

    [Fact]
    public async Task Post_WithBlankTitle_Returns400()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/todos",
            new CreateTodoRequest { Title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatedTodo_AppearsInTheList()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest { Title = "First" });
        await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest { Title = "Second" });

        var todos = await client.GetFromJsonAsync<List<TodoResponse>>("/api/todos");

        Assert.Equal(2, todos!.Count);
        Assert.Contains(todos, t => t.Title == "First");
        Assert.Contains(todos, t => t.Title == "Second");
    }

    [Fact]
    public async Task Put_UpdatesTitleAndCompletedState()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        var created = await (await client.PostAsJsonAsync(
            "/api/todos", new CreateTodoRequest { Title = "Draft" }))
            .Content.ReadFromJsonAsync<TodoResponse>();

        var response = await client.PutAsJsonAsync(
            $"/api/todos/{created!.Id}",
            new UpdateTodoRequest { Title = "Final", IsCompleted = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.Equal("Final", updated!.Title);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task Put_ForUnknownId_Returns404()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        var response = await client.PutAsJsonAsync(
            $"/api/todos/{Guid.NewGuid()}",
            new UpdateTodoRequest { Title = "Nope" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesTheTodo()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        var created = await (await client.PostAsJsonAsync(
            "/api/todos", new CreateTodoRequest { Title = "Temporary" }))
            .Content.ReadFromJsonAsync<TodoResponse>();

        var deleteResponse = await client.DeleteAsync($"/api/todos/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ForUnknownId_Returns404()
    {
        using var api = CreateApi();
        using var client = api.CreateClient();

        var response = await client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
