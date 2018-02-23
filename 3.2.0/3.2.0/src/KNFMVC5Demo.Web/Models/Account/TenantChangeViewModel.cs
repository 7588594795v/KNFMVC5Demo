using Abp.AutoMapper;
using KNFMVC5Demo.Sessions.Dto;

namespace KNFMVC5Demo.Web.Models.Account
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}