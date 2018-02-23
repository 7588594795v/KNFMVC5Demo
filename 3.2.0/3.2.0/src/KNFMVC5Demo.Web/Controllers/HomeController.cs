using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;

namespace KNFMVC5Demo.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : KNFMVC5DemoControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}