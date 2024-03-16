using Common.Domain;

namespace Common.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; } = default!;
        public virtual ICollection<ToDo> ToDos { get; set; } = default!;
    }
}
