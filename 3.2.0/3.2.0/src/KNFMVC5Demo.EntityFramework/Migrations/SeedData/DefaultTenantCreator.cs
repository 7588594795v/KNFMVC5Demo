using System.Linq;
using KNFMVC5Demo.EntityFramework;
using KNFMVC5Demo.MultiTenancy;

namespace KNFMVC5Demo.Migrations.SeedData
{
    public class DefaultTenantCreator
    {
        private readonly KNFMVC5DemoDbContext _context;

        public DefaultTenantCreator(KNFMVC5DemoDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUserAndRoles();
        }

        private void CreateUserAndRoles()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                _context.Tenants.Add(new Tenant {TenancyName = Tenant.DefaultTenantName, Name = Tenant.DefaultTenantName});
                _context.SaveChanges();
            }
        }
    }
}
