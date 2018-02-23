using System.Threading.Tasks;
using Abp.Application.Services;
using KNFMVC5Demo.Sessions.Dto;

namespace KNFMVC5Demo.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
