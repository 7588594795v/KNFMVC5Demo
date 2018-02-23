using System.Collections.Generic;
using KNFMVC5Demo.Roles.Dto;

namespace KNFMVC5Demo.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}