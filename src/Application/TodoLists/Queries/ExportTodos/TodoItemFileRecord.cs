using Application.Common.Mappings;
using Domain.Entities;

namespace Application.TodoLists.Queries.ExportTodos;
public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string Title { get; set; }

    public bool Done { get; set; }
}
