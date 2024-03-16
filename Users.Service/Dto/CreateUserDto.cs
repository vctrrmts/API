namespace Users.Service.Dto
{
    public class CreateUserDto
    {
        public string Login { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
