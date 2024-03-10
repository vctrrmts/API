using Common.Domain;

namespace Common.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public virtual ICollection<ToDo> ToDo { get; set; }
    }
}
