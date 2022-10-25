using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Viettel.Domain.DomainModel;
using Viettel.Services;

namespace VIETTEL
{
    public static class BreadcrumbExtension
    {
        public static MvcHtmlString BuildBreadcrumbNavigation(this HtmlHelper helper)
        {
            string area = (helper.ViewContext.RouteData.DataTokens["area"] ?? "").ToString();
            string controller = helper.ViewContext.RouteData.Values["controller"].ToString();
            string action = helper.ViewContext.RouteData.Values["action"].ToString();

            // add link to homepage by default
            StringBuilder breadcrumb = new StringBuilder(@"
                <ol class='breadcrumb'>
                    <li>" + helper.ActionLink(L("Home"), "Index", "Home", new { Area = "" }, new { @class = "first" }) + @"</li>");

            // add link to area if existing
            if (area != "")
            {
                breadcrumb.Append("<li>");
                if (ControllerExistsInArea("Default", area)) // by convention, default Area controller should be named Default
                {
                    breadcrumb.Append(helper.ActionLink(L(area), "Index", "Default", new { Area = area }, new { @class = "" }));
                }
                else
                {
                    breadcrumb.Append(L(area));
                }
                breadcrumb.Append("</li>");
            }

            // add link to controller Index if different action
            if ((controller != "Home" && controller != "Default") && action != "Index")
            {
                if (ActionExistsInController("Index", controller, area))
                {
                    breadcrumb.Append("<li>");
                    breadcrumb.Append(helper.ActionLink(L(controller), "Index", controller, new { Area = area }, new { @class = "" }));
                    breadcrumb.Append("</li>");
                }
            }

            // add link to action
            if ((controller != "Home" && controller != "Default") || action != "Index")
            {
                breadcrumb.Append("<li>");
                //breadcrumb.Append(helper.ActionLink((action.ToLower() == "index") ? controller.AddSpaceOnCaseChange() : action.AddSpaceOnCaseChange(), action, controller, new { Area = area }, new { @class = "" }));
                var text = (action.ToLower() == "index") ? L(controller) : L(action);
                if (text.First() == '[')
                    text = helper.ViewBag.Title;
                breadcrumb.Append(text);

                breadcrumb.Append("</li>");
            }

            return MvcHtmlString.Create(breadcrumb.Append("</ol>").ToString());
        }


        public static MvcHtmlString BuildBreadcrumbButtons(this HtmlHelper helper, string className = "btn-default")
        {
            string area = (helper.ViewContext.RouteData.DataTokens["area"] ?? "").ToString();
            string controller = helper.ViewContext.RouteData.Values["controller"].ToString();
            string action = helper.ViewContext.RouteData.Values["action"].ToString();

            // add link to homepage by default
            //StringBuilder breadcrumb = new StringBuilder(@"
            //    <div class='btn-group btn-breadcrumb'>
            //        <a>" + helper.ActionLink(L("Homepage"), "Index", "Home", new { Area = "" }, new { @class = $"btn {className}" }) + @"</a>");

            StringBuilder breadcrumb = new StringBuilder(@"
                <div class='btn-group btn-breadcrumb'>" +
                    $"<a href='/Home' class= 'btn {className}'><i class='fa fa-home'></i></a>");


            // add link to area if existing
            if (area != "")
            {
                //breadcrumb.Append("<a>");
                if (ControllerExistsInArea("Default", area)) // by convention, default Area controller should be named Default
                {
                    breadcrumb.Append(helper.ActionLink(L(area), "Index", "Default", new { Area = area }, new { @class = $"btn {className}" }));
                }
                else
                {
                    breadcrumb.Append($"<a class='btn {className}'>{L(area)}</a>");
                }
                //breadcrumb.Append("</a>");
            }

            // add link to controller Index if different action
            if ((controller != "Home" && controller != "Default") && action != "Index")
            {
                if (ActionExistsInController("Index", controller, area))
                {
                    //breadcrumb.Append("<a>");
                    breadcrumb.Append(helper.ActionLink(L(controller), "Index", controller, new { Area = area }, new { @class = $"btn {className}" }));
                    //breadcrumb.Append("</a>");
                }
            }

            // add link to action
            if ((controller != "Home" && controller != "Default") || action != "Index")
            {
                breadcrumb.Append($"<a class='btn {className}'>");
                //breadcrumb.Append(helper.ActionLink((action.ToLower() == "index") ? controller.AddSpaceOnCaseChange() : action.AddSpaceOnCaseChange(), action, controller, new { Area = area }, new { @class = "" }));
                //breadcrumb.Append((action.ToLower() == "index") ? L(controller) : L(action));
                var text = (action.ToLower() == "index") ? L(controller) : L(action);
                if (text.First() == '[' && !string.IsNullOrWhiteSpace(helper.ViewBag.Title))
                    text = helper.ViewBag.Title;

                breadcrumb.Append(text);
                breadcrumb.Append("</a>");
            }

            return MvcHtmlString.Create(breadcrumb.Append("</div>").ToString());
        }

        public static MvcHtmlString BuildBreadcrumbWebNewButtons(this HtmlHelper helper, string className = "btn-default")
        {
            var url = helper.ViewContext.HttpContext.Request.Url.ToString();
            string area = (helper.ViewContext.RouteData.DataTokens["area"] ?? "").ToString();
            string controller = helper.ViewContext.RouteData.Values["controller"].ToString();
            string action = helper.ViewContext.RouteData.Values["action"].ToString();
            StringBuilder breadcrumb = new StringBuilder(@"
                <div class='btn-group btn-breadcrumb'>" +
                    $"<a href='/Home' class= 'btn {className} nav-link' data-id='0' ><i class='fa fa-home'></i>Về trang chủ</a>");

            ICommonService _common = CommonService.Default;
            int iIdMenu = 0;
            if(helper.ViewContext.HttpContext.Session["iIdMenu"] != null)
            {
                iIdMenu = (int)helper.ViewContext.HttpContext.Session["iIdMenu"];
            }
            if (!url.Contains("/Home"))
            {
                IEnumerable<MENU_MenuItem> lstMenu = _common.GetBreadcrumbStringByMenuId(iIdMenu);
                foreach (MENU_MenuItem item in lstMenu)
                {
                    breadcrumb.Append($"<a class='btn {className}'>{item.sTen}</a>");
                }
            }
            //else
            //{
            //    helper.ViewContext.HttpContext.Session["iIdMenu"] = null;
            //}
            //// add link to area if existing
            //if (area != "")
            //{
            //    //breadcrumb.Append("<a>");
            //    if (ControllerExistsInArea("Default", area)) // by convention, default Area controller should be named Default
            //    {
            //        breadcrumb.Append(helper.ActionLink(L(area), "Index", "Default", new { Area = area }, new { @class = $"btn {className}" }));
            //    }
            //    else
            //    {
            //        breadcrumb.Append($"<a class='btn {className}'>{L(area)}</a>");
            //    }
            //    //breadcrumb.Append("</a>");
            //}

            //// add link to controller Index if different action
            //if ((controller != "Home" && controller != "Default") && action != "Index")
            //{
            //    if (ActionExistsInController("Index", controller, area))
            //    {
            //        //breadcrumb.Append("<a>");
            //        breadcrumb.Append(helper.ActionLink(L(controller), "Index", controller, new { Area = area }, new { @class = $"btn {className}" }));
            //        //breadcrumb.Append("</a>");
            //    }
            //}

            //// add link to action
            //if ((controller != "Home" && controller != "Default") || action != "Index")
            //{
            //    breadcrumb.Append($"<a class='btn {className}'>");
            //    //breadcrumb.Append(helper.ActionLink((action.ToLower() == "index") ? controller.AddSpaceOnCaseChange() : action.AddSpaceOnCaseChange(), action, controller, new { Area = area }, new { @class = "" }));
            //    //breadcrumb.Append((action.ToLower() == "index") ? L(controller) : L(action));
            //    var text = (action.ToLower() == "index") ? L(controller) : L(action);
            //    if (text.First() == '[' && !string.IsNullOrWhiteSpace(helper.ViewBag.Title))
            //        text = helper.ViewBag.Title;

            //    breadcrumb.Append(text);
            //    breadcrumb.Append("</a>");
            //}

            return MvcHtmlString.Create(breadcrumb.Append("</div>").ToString());
        }

        public static MvcHtmlString BuildPagingButtons(this HtmlHelper helper, PagingInfo _paging)
        {

            if(_paging.TotalPages == 0) return MvcHtmlString.Create(string.Empty);
            StringBuilder sPagingHtml = new StringBuilder();
            sPagingHtml.Append("<nav class='paging justify-content-end'>");
            sPagingHtml.Append("<ul class='pagination'>");
            sPagingHtml.Append("<li class='page-item'>");
            sPagingHtml.AppendFormat("    <a class='page-link' href='#' onClick='ChangePage({0})' aria-label='Previous'>", 1);
            sPagingHtml.Append("        <span aria-hidden='true'>&laquo;</span>");
            sPagingHtml.Append("    </a>");
            sPagingHtml.Append("</li>");

            int iStartPage = _paging.CurrentPage - 2;
            int iEndPage = _paging.CurrentPage + 2;

            if(iStartPage <= 0)
            {
                iEndPage -= (iStartPage - 1);
                iStartPage = 1;
            }

            if (iEndPage > _paging.TotalPages)
                iEndPage = _paging.TotalPages;

            for(int i = iStartPage; i <= iEndPage; i++)
            {
                if(_paging.CurrentPage == i)
                    sPagingHtml.AppendFormat("<li class='page-item active'><a class='page-link' href='#'>{0}</a></li>", i);
                else
                    sPagingHtml.AppendFormat("<li class='page-item'><a class='page-link' onClick='ChangePage({0})' href='#'>{0}</a></li>", i);
            }

            sPagingHtml.Append("<li class='page-item'>");
            sPagingHtml.AppendFormat("    <a class='page-link' href='#' onClick='ChangePage({0})' aria-label='Next'>", _paging.TotalPages);
            sPagingHtml.Append("        <span aria-hidden='true'>&raquo;</span>");
            sPagingHtml.Append("    </a>");
            sPagingHtml.Append("</li>");
            sPagingHtml.Append("</ul>");
            sPagingHtml.AppendFormat("<div class='paging-content'>Hiển thị {0} - {1} trong : {2} bản ghi</div>", _paging.ItemForm, _paging.ItemTo, _paging.TotalItems);
            sPagingHtml.Append("</nav>");
            return MvcHtmlString.Create(sPagingHtml.ToString());
        }

        public static Type GetControllerType(string controller, string area)
        {
            string currentAssembly = Assembly.GetExecutingAssembly().GetName().Name;
            IEnumerable<Type> controllerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(o => typeof(IController).IsAssignableFrom(o));

            string typeFullName = String.Format("{0}.Controllers.{1}Controller", currentAssembly, controller);
            if (area != "")
            {
                typeFullName = String.Format("{0}.Areas.{1}.Controllers.{2}Controller", currentAssembly, area, controller);
            }

            return controllerTypes.Where(o => o.FullName == typeFullName).FirstOrDefault();
        }

        public static bool ActionExistsInController(string action, string controller, string area)
        {
            Type controllerType = GetControllerType(controller, area);
            return (controllerType != null && new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().Any(x => x.ActionName == action));
        }

        public static bool ControllerExistsInArea(string controller, string area)
        {
            Type controllerType = GetControllerType(controller, area);
            return (controllerType != null);
        }


        public static string AddSpaceOnCaseChange(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static string L(string key)
        {
            return Viettel.Services.LocalizationService.Default.Translate(key);
        }
    }
}
