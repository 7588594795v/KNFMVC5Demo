using Abp.MultiTenancy;
using KNFMVC5Demo.Authorization.Users;

namespace KNFMVC5Demo.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}