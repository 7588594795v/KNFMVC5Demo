using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using KNFMVC5Demo.EntityFramework;

namespace KNFMVC5Demo.Migrator
{
    [DependsOn(typeof(KNFMVC5DemoDataModule))]
    public class KNFMVC5DemoMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<KNFMVC5DemoDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}