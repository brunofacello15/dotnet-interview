using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models;

public class TodoList
{
    public long Id { get; set; }
    public string? Name { get; set; }

    [ForeignKey("TodoListId")]
    public List<TodoItem> Items { get; set; }

}
