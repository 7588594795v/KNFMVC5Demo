using System.Collections.Generic;
using KNFMVC5Demo.Roles.Dto;
using KNFMVC5Demo.Users.Dto;

namespace KNFMVC5Demo.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}