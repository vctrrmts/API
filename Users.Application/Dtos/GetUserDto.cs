namespace Users.Application.Dtos
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = default!;
    }
}
