using TodoApp.Api.Contracts;
using TodoApp.Api.Models;
using TodoApp.Api.Repositories;

namespace TodoApp.Api.Services;

public sealed class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyCollection<TodoResponse> GetAll() =>
        _repository.GetAll()
            .Select(TodoResponse.FromDomain)
            .ToArray();

    public TodoResponse? GetById(Guid id)
    {
        var item = _repository.GetById(id);
        return item is null ? null : TodoResponse.FromDomain(item);
    }

    public TodoResponse Create(CreateTodoRequest request)
    {
        var item = new TodoItem(request.Title);
        _repository.Add(item);
        return TodoResponse.FromDomain(item);
    }

    public TodoResponse? Update(Guid id, UpdateTodoRequest request)
    {
        var item = _repository.GetById(id);
        if (item is null)
        {
            return null;
        }

        item.Update(request.Title, request.IsCompleted);
        _repository.Update(item);
        return TodoResponse.FromDomain(item);
    }

    public bool Delete(Guid id) =>
        _repository.Remove(id);
}
