namespace Todos.Application.Dtos
{
    public class BaseTodosFilter
    {
        public string? LabelFreeText { get; set; }
        public int? OwnerId { get; set; }
    }
}
