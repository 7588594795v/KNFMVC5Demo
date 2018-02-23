using Abp.Authorization;
using KNFMVC5Demo.Authorization.Roles;
using KNFMVC5Demo.Authorization.Users;

namespace KNFMVC5Demo.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
