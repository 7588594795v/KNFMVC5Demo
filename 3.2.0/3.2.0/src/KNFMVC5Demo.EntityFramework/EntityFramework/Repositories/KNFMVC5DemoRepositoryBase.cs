using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace KNFMVC5Demo.EntityFramework.Repositories
{
    public abstract class KNFMVC5DemoRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<KNFMVC5DemoDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected KNFMVC5DemoRepositoryBase(IDbContextProvider<KNFMVC5DemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class KNFMVC5DemoRepositoryBase<TEntity> : KNFMVC5DemoRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected KNFMVC5DemoRepositoryBase(IDbContextProvider<KNFMVC5DemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
