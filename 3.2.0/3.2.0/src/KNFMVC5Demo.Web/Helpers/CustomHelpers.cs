using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Dapper;

namespace KNFMVC5Demo.HelperObjects
{
    public static class CustomHelpers
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) action(item);
            return collection;
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> collection)
        {
            return new SelectList(collection, "Key", "Value");
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> collection, string selectedValue)
        {
            return new SelectList(collection, "Key", "Value", selectedValue);
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> collection,
                             string dataValueField, string dataTextField)
        {
            return new SelectList(collection, dataValueField, dataTextField);
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> collection,
                             string dataValueField, string dataTextField, string selectedValue)
        {
            return new SelectList(collection, dataValueField, dataTextField, selectedValue);
        }
        
        public static void FlashInfo(this Controller controller, string message)
        {
            controller.TempData["info"] = message;
        }
        public static void FlashWarning(this Controller controller, string message)
        {
            controller.TempData["warning"] = message;
        }
        public static void FlashError(this Controller controller, string message)
        {
            controller.TempData["error"] = message;
        }

        public static string Flash(this HtmlHelper helper)
        {

            var message = "";
            var className = "";
            if (helper.ViewContext.TempData["info"] != null)
            {
                message = helper.ViewContext.TempData["info"].ToString();
                className = "info";
            }
            else if (helper.ViewContext.TempData["warning"] != null)
            {
                message = helper.ViewContext.TempData["warning"].ToString();
                className = "warning";
            }
            else if (helper.ViewContext.TempData["error"] != null)
            {
                message = helper.ViewContext.TempData["error"].ToString();
                className = "error";
            }
            var sb = new StringBuilder();
            if (!String.IsNullOrEmpty(message))
            {
                sb.AppendLine("<script>");
                sb.AppendLine("$(document).ready(function() {");
                sb.AppendFormat("$('#flash').html('{0}');", message);
                sb.AppendFormat("$('#flash').toggleClass('{0}');", className);
                sb.AppendLine("$('#flash').slideDown('slow');");
                sb.AppendLine("$('#flash').click(function(){$('#flash').toggle('highlight')});");
                sb.AppendLine("});");
                sb.AppendLine("</script>");
            }
            return sb.ToString();
        }

        public static System.Web.Mvc.MvcHtmlString ToMvcHtmlString(this string value)
        {
            return System.Web.Mvc.MvcHtmlString.Create(value);
        }

        public static string ToString(this DateTime dateTime, string format, bool useExtendedSpecifiers)
        {
            return dateTime.ToString(format)
                .Replace("nn", dateTime.Day.ToOccurrenceSuffix().ToLower())
                .Replace("NN", dateTime.Day.ToOccurrenceSuffix().ToUpper());
        }

        public static string ToOccurrenceSuffix(this int integer)
        {
            switch (integer % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }
            switch (integer % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        /// <summary>
        /// This extension converts an enumerable set to a Dapper TVP
        /// </summary>
        /// <typeparam name="T">type of enumerbale</typeparam>
        /// <param name="enumerable">list of values</param>
        /// <param name="typeName">database type name</param>
        /// <param name="orderedColumnNames">if more than one column in a TVP, columns order must mtach order of columns in TVP</param>
        /// <returns>a custom query parameter</returns>
        public static SqlMapper.ICustomQueryParameter AsTableValuedParameter<T>(this IEnumerable<T> enumerable,
            string typeName, IEnumerable<string> orderedColumnNames = null)
        {
            var dataTable = new DataTable();
            if (typeof(T).IsValueType)
            {
                dataTable.Columns.Add("NONAME", typeof(T));
                foreach (T obj in enumerable)
                {
                    dataTable.Rows.Add(obj);
                }
            }
            else
            {
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo[] readableProperties = properties.Where(w => w.CanRead).ToArray();
                if (readableProperties.Length > 1 && orderedColumnNames == null)
                    throw new ArgumentException("Ordered list of column names must be provided when TVP contains more than one column");
                var columnNames = (orderedColumnNames ?? readableProperties.Select(s => s.Name)).ToArray();
                foreach (string name in columnNames)
                {
                    dataTable.Columns.Add(name, readableProperties.Single(s => s.Name.Equals(name)).PropertyType);
                }

                foreach (T obj in enumerable)
                {
                    dataTable.Rows.Add(
                        columnNames.Select(s => readableProperties.Single(s2 => s2.Name.Equals(s)).GetValue(obj))
                            .ToArray());
                }
            }
            return dataTable.AsTableValuedParameter(typeName);
        }

        public static bool IsImpersonating(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return false;
            }
            return principal.HasClaim("UserImpersonation", "true");
        }

        public static String GetOriginalUsername(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return String.Empty;
            }

            if (!principal.IsImpersonating())
            {
                return String.Empty;
            }

            var originalUsernameClaim = principal.Claims.SingleOrDefault(c => c.Type == "OriginalUsername");

            if (originalUsernameClaim == null)
            {
                return String.Empty;
            }

            return originalUsernameClaim.Value;
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


    }
}