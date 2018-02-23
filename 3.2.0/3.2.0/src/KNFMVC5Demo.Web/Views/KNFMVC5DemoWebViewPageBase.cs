using Abp.Web.Mvc.Views;

namespace KNFMVC5Demo.Web.Views
{
    public abstract class KNFMVC5DemoWebViewPageBase : KNFMVC5DemoWebViewPageBase<dynamic>
    {

    }

    public abstract class KNFMVC5DemoWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected KNFMVC5DemoWebViewPageBase()
        {
            LocalizationSourceName = KNFMVC5DemoConsts.LocalizationSourceName;
        }
    }
}