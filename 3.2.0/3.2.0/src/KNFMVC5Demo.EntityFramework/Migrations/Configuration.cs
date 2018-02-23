using System.Data.Entity.Migrations;
using Abp.MultiTenancy;
using Abp.Zero.EntityFramework;
using KNFMVC5Demo.Migrations.SeedData;
using EntityFramework.DynamicFilters;

namespace KNFMVC5Demo.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<KNFMVC5Demo.EntityFramework.KNFMVC5DemoDbContext>, IMultiTenantSeed
    {
        public AbpTenantBase Tenant { get; set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "KNFMVC5Demo";
        }

        protected override void Seed(KNFMVC5Demo.EntityFramework.KNFMVC5DemoDbContext context)
        {
            context.DisableAllFilters();

            if (Tenant == null)
            {
                //Host seed
                new InitialHostDbBuilder(context).Create();

                //Default tenant seed (in host database).
                new DefaultTenantCreator(context).Create();
                new TenantRoleAndUserBuilder(context, 1).Create();
            }
            else
            {
                //You can add seed for tenant databases and use Tenant property...
            }

            context.SaveChanges();
        }
    }
}
