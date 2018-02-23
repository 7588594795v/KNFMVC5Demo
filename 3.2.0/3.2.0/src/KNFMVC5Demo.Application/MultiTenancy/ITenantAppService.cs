using Abp.Application.Services;
using Abp.Application.Services.Dto;
using KNFMVC5Demo.MultiTenancy.Dto;

namespace KNFMVC5Demo.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
