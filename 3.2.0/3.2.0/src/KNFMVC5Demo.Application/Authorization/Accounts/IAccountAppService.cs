using System.Threading.Tasks;
using Abp.Application.Services;
using KNFMVC5Demo.Authorization.Accounts.Dto;

namespace KNFMVC5Demo.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
