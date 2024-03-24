using MediatR;
using Users.Application.Dtos;

namespace Users.Application.Query.GetCount
{
    public class GetCountQuery : BaseUsersFilter, IRequest<int>
    {
    }
}
