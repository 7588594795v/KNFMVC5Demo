using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using KNFMVC5Demo.EntityFramework;

namespace KNFMVC5Demo
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(KNFMVC5DemoCoreModule))]
    public class KNFMVC5DemoDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<KNFMVC5DemoDbContext>());

            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
