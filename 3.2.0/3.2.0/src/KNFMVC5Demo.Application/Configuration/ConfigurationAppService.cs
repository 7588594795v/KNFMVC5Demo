using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using KNFMVC5Demo.Configuration.Dto;

namespace KNFMVC5Demo.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : KNFMVC5DemoAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
