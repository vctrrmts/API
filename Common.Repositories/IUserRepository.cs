using Common.Domain;

namespace Common.Repositories
{
    public interface IUserRepository
    {
        IReadOnlyCollection<User> GetList(int? offset, int limit);
        User? GetById(int id);
        User Post(User todo);
        User? Patch(int id, string name);
        bool Delete(User user);
    }
}
