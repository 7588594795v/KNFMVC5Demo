using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using KNFMVC5Demo.Roles.Dto;

namespace KNFMVC5Demo.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
    }
}
