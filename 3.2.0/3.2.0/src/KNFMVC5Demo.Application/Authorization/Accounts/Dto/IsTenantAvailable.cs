using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;

namespace KNFMVC5Demo.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {
        [Required]
        [MaxLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}
