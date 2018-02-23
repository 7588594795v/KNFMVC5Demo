
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using KNFMVC5Demo.EntityFramework;
using Dapper;
using KNFMVC5Demo.HelperObjects;

namespace KNFMVC5Demo.Web.Helpers
{
    public static class HtmlHelpers
    {
        private class ScriptBlock : IDisposable
        {
            private const string ScriptsKey = "PartialViewScripts";
            public static List<string> PartialViewScripts
            {
                get
                {
                    if (HttpContext.Current.Items[ScriptsKey] == null)
                        HttpContext.Current.Items[ScriptsKey] = new List<string>();
                    return (List<string>)HttpContext.Current.Items[ScriptsKey];
                }
            }

            readonly WebViewPage _webPageBase;

            public ScriptBlock(WebViewPage webPageBase)
            {
                _webPageBase = webPageBase;
                _webPageBase.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                PartialViewScripts.Add(((StringWriter)this._webPageBase.OutputStack.Pop()).ToString());
            }
        }

        public static IDisposable BeginScripts(this HtmlHelper helper)
        {
            return new ScriptBlock((WebViewPage)helper.ViewDataContainer);
        }

        public static MvcHtmlString PartialViewScripts(this HtmlHelper helper)
        {
            return MvcHtmlString.Create(string.Join(Environment.NewLine, ScriptBlock.PartialViewScripts.Select(s => s.ToString())));
        }

        public static IEnumerable<T> ViewData<T>(this HtmlHelper helper, string name)
        {
            if (helper.ViewData[name] != null)
            {
                return (IEnumerable<T>)helper.ViewData[name];
            }
            return new List<T>();
        }

        public static T ViewDataSingle<T>(this HtmlHelper helper, string name)
        {
            if (helper.ViewData[name] != null)
            {
                return (T)helper.ViewData[name];
            }
            return default(T);
        }

        public static string IsActive(this HtmlHelper helper, string action = "", string controller = "", string cssClass = "", string subMenuCssClass = "")
        {
            string returncssclass = string.Empty;
            ViewContext viewContext = helper.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
                viewContext = helper.ViewContext.ParentActionViewContext;

            RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            if (string.IsNullOrEmpty(action))
                action = currentAction;

            if (string.IsNullOrEmpty(controller))
                controller = currentController;

            string[] acceptedActions = action.ToLower().Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = controller.ToLower().Trim().Split(',').Distinct().ToArray();

            if ((!string.IsNullOrEmpty(cssClass)) && (string.IsNullOrEmpty(subMenuCssClass)))
            {
                returncssclass = acceptedControllers.Contains(currentController.ToLower()) ? cssClass : String.Empty;
            }
            if ((!string.IsNullOrEmpty(subMenuCssClass)) && (string.IsNullOrEmpty(cssClass)))
            {
                string[] subMenuCss = subMenuCssClass.ToLower().Split(' ').Distinct().ToArray();
                returncssclass = acceptedControllers.Contains(currentController.ToLower()) ? subMenuCss[0].ToString() : subMenuCssClass;
            }

            return returncssclass;

            //return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ? cssClass : String.Empty;
        }

        public static string Amount(this HtmlHelper helper, double? amount)
        {

            string returnAmount = String.Empty;
            returnAmount = amount == null ? "N/A" : Convert.ToDouble(amount).ToString("0.00");
            return returnAmount;

        }

        public static string Name(this HtmlHelper helper, string name = "")
        {
            string returnName = String.Empty;
            returnName = string.IsNullOrEmpty(name) ? "N/A" : name.ToString();
            return returnName;
        }

        public static string Date(this HtmlHelper helper, DateTime? date)
        {
            string returnDate = String.Empty;
            returnDate = date == null ? "N/A" : Convert.ToDateTime(date).ToString("dd MMM yyyy");
            return returnDate;
        }

        public static string Status(this HtmlHelper helper, bool? trueFlag, bool? falseFlag, string status = "")
        {
            string returnStatus = String.Empty;

            if (trueFlag == null && falseFlag == null)
            {
                returnStatus = "N/A";
            }
            else
            {
                if (Convert.ToBoolean(trueFlag))
                {
                    returnStatus = status.ToString();
                }
                if (Convert.ToBoolean(falseFlag))
                {
                    returnStatus = "N/A";
                }
                if (Convert.ToBoolean(trueFlag) && Convert.ToBoolean(falseFlag))
                {
                    returnStatus = status.ToString();
                }
            }

            return returnStatus;
        }

        public static string IsSelected(this HtmlHelper html, string controllers = "", string actions = "", string cssClass = "")
        {
            ViewContext viewContext = html.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
                viewContext = html.ViewContext.ParentActionViewContext;

            RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            if (actions.IsNull())
                actions = currentAction;

            if (String.IsNullOrEmpty(controllers))
                controllers = currentController;

            string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }

        public static MvcHtmlString Avatar(this HtmlHelper helper, string userId, string cssClass = "", int w = 32, int h = 32)
        {
            string src, fileName;
            if (!string.IsNullOrEmpty(userId))
            {
                var p = new DynamicParameters();
                p.Add("@UserID", userId);
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings[knfConstant.DAPPER_CONNECTION_STRING].ConnectionString))
                {
                    fileName = con.ExecuteScalar<string>(sql: "[dbo].[KSP_GetAvatar]", param: p, commandType: CommandType.StoredProcedure);
                }
            }
            else
            {
                fileName = "Avatar.png";
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "Avatar.png";
            }
            src = string.Format("/ProfileImage/{0}?w={1}&h={2}", fileName, w, h);
            return new MvcHtmlString(String.Format("<img src=\"{0}\" class=\"{1}\" alt=\"Avatar-Image\"/>", src, cssClass));
        }

        public static string UserName(this HtmlHelper helper, string userId)
        {
            string name;
            if (!string.IsNullOrEmpty(userId))
            {
                var p = new DynamicParameters();
                p.Add("@UserID", userId);
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings[knfConstant.DAPPER_CONNECTION_STRING].ConnectionString))
                {
                    name = con.ExecuteScalar<string>(sql: "[dbo].[KSP_GetUserName]", param: p, commandType: CommandType.StoredProcedure);
                }
            }
            else
            {
                name = "Guest";
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "Guest";
            }
            return name;
        }

    }
}