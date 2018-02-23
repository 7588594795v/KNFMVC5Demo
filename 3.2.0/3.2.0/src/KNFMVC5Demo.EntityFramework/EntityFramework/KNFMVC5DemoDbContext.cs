using System.Data.Common;
using Abp.Zero.EntityFramework;
using KNFMVC5Demo.Authorization.Roles;
using KNFMVC5Demo.Authorization.Users;
using KNFMVC5Demo.MultiTenancy;

namespace KNFMVC5Demo.EntityFramework
{
    public class KNFMVC5DemoDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public KNFMVC5DemoDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in KNFMVC5DemoDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of KNFMVC5DemoDbContext since ABP automatically handles it.
         */
        public KNFMVC5DemoDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public KNFMVC5DemoDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public KNFMVC5DemoDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }
    }
}
