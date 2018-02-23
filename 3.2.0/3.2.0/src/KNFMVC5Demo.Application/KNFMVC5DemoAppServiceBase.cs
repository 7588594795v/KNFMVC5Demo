using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using KNFMVC5Demo.Authorization.Users;
using KNFMVC5Demo.MultiTenancy;
using KNFMVC5Demo.Users;
using Microsoft.AspNet.Identity;

namespace KNFMVC5Demo
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class KNFMVC5DemoAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected KNFMVC5DemoAppServiceBase()
        {
            LocalizationSourceName = KNFMVC5DemoConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}