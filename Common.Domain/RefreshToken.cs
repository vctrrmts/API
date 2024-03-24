namespace Common.Domain
{
    public class RefreshToken
    {
        public string Id { get; set; } = default!;
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = default!;
    }
}
