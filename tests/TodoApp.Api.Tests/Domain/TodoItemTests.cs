using TodoApp.Api.Models;

namespace TodoApp.Api.Tests.Domain;

public class TodoItemTests
{
    [Fact]
    public void NewTodo_IsNotCompleted_AndHasAnId()
    {
        var todo = new TodoItem("Buy milk");

        Assert.NotEqual(Guid.Empty, todo.Id);
        Assert.False(todo.IsCompleted);
        Assert.Equal("Buy milk", todo.Title);
    }

    [Fact]
    public void Title_IsTrimmed()
    {
        var todo = new TodoItem("   Walk the dog   ");

        Assert.Equal("Walk the dog", todo.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreatingWithBlankTitle_Throws(string? title)
    {
        Assert.Throws<ArgumentException>(() => new TodoItem(title!));
    }

    [Fact]
    public void Update_ChangesTitleAndCompletedState()
    {
        var todo = new TodoItem("Draft email");

        todo.Update("Send email", isCompleted: true);

        Assert.Equal("Send email", todo.Title);
        Assert.True(todo.IsCompleted);
    }
}
