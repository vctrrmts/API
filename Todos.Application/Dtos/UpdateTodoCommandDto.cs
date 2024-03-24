namespace Todos.Application.Dtos
{
    public class UpdateTodoCommandDto
    {
        public string Label { get; set; } = default!;
        public bool IsDone { get; set; }
    }
}
