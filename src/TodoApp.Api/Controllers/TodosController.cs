using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Contracts;
using TodoApp.Api.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class TodosController : ControllerBase
{
    private readonly ITodoService _todos;

    public TodosController(ITodoService todos)
    {
        _todos = todos;
    }

    /// <summary>Returns the full todo list, oldest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TodoResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<TodoResponse>> GetAll() =>
        Ok(_todos.GetAll());

    /// <summary>Returns a single todo by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoResponse> GetById(Guid id)
    {
        var todo = _todos.GetById(id);
        return todo is null ? NotFound() : Ok(todo);
    }

    /// <summary>Adds a new todo to the list.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TodoResponse> Create(CreateTodoRequest request)
    {
        var created = _todos.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates a todo's title and/or completed state.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoResponse> Update(Guid id, UpdateTodoRequest request)
    {
        var updated = _todos.Update(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Deletes a todo from the list.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        _todos.Delete(id) ? NoContent() : NotFound();
}
