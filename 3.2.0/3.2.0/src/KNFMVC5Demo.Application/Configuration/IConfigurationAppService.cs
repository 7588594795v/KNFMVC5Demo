using System.Threading.Tasks;
using Abp.Application.Services;
using KNFMVC5Demo.Configuration.Dto;

namespace KNFMVC5Demo.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}