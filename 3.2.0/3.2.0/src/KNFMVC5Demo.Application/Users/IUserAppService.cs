using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using KNFMVC5Demo.Roles.Dto;
using KNFMVC5Demo.Users.Dto;

namespace KNFMVC5Demo.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
    }
}