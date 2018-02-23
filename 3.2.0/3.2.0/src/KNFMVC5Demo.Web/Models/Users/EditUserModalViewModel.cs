using System.Collections.Generic;
using System.Linq;
using KNFMVC5Demo.Roles.Dto;
using KNFMVC5Demo.Users.Dto;

namespace KNFMVC5Demo.Web.Models.Users
{
    public class EditUserModalViewModel
    {
        public UserDto User { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }

        public bool UserIsInRole(RoleDto role)
        {
            return User.Roles != null && User.Roles.Any(r => r == role.DisplayName);
        }
    }
}