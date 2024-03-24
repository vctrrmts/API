using MediatR;
using Users.Application.Dtos;

namespace Users.Application.Query.GetList
{
    public class GetListQuery : BaseUsersFilter, IRequest<IReadOnlyCollection<GetUserDto>>
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
    }
}
