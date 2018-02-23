using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using KNFMVC5Demo.MultiTenancy;

namespace KNFMVC5Demo.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}