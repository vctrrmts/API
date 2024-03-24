namespace Common.Domain
{
    public class ApplicationUserRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public IEnumerable<ApplicationUserApplicationRole> Users { get; set; } = default!;
    }
}
