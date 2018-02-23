using KNFMVC5Demo.EntityFramework;
using EntityFramework.DynamicFilters;

namespace KNFMVC5Demo.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly KNFMVC5DemoDbContext _context;

        public InitialHostDbBuilder(KNFMVC5DemoDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
