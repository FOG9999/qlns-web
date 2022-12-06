using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Models;

namespace VIETTEL
{
    public static class MenuExtension
    {
        public static MvcHtmlString BuildMenuLeft(this HtmlHelper helper, IEnumerable<MenuLeftViewModel> lstMenus, string sParentName, List<int> lstMenuChoose)
        {
            if (lstMenus == null) return MvcHtmlString.Create(string.Empty);
            StringBuilder sMenu = new StringBuilder();
            Dictionary<int, List<MenuLeftViewModel>> dicMenu = lstMenus.GroupBy(n => n.iParentId).ToDictionary(n => n.Key, n => n.ToList());

            if (!string.IsNullOrEmpty(sParentName))
            {
                sMenu.AppendFormat(@"<div class='sidebar-header' id='sidebarCollapse'>
                                        <h7 style = 'font-weight: bold' class = 'thu_gon_menu' title = 'Thu gọn menu'> {0} <i class='fa fa-angle-double-left' style='font-size:18px; float: right'></i></h7>
                                        <i class='fa fa-angle-double-right fa-2x' aria-hidden='true' style='display:none'></i>
                                    </div> <hr class='color-background-red' style='margin-top: -3px' />", sParentName);
            }
            sMenu.Append(@"<div class='nav collapse navbar-collapse'>
                                <ul class='nav navbar-nav mr-auto mt-2 mt-lg-0'>");

            foreach (var itemMenu in lstMenus.Where(n => n.menu_level == 3).OrderBy(x => x.thu_tu))
            {
                sMenu.Append(MenuRecursion(itemMenu, dicMenu, lstMenuChoose));
            }

            sMenu.Append(@"     </ul>
                           </div>");
            return MvcHtmlString.Create(sMenu.ToString());
        }

        private static string MenuRecursion(MenuLeftViewModel itemMenu, Dictionary<int, List<MenuLeftViewModel>> dicMenu, List<int> lstMenuChoose)
        {
            StringBuilder str = new StringBuilder();
            bool isMenuChoose = false;
            if (lstMenuChoose.IndexOf(itemMenu.menu_id) != -1) isMenuChoose = true;

            if (dicMenu.ContainsKey(itemMenu.menu_id) && dicMenu[itemMenu.menu_id].Count != 0)
            {
                str.Append("<li class='nav-item parent-menu-left'>");
                str.Append(CreateItemMenu(itemMenu, true, lstMenuChoose, dicMenu[itemMenu.menu_id]));
                str.Append("</li>");

                str.AppendFormat("<li class='nav-item menu-child-left' id='lst-menu-{0}' style='{1}'>", itemMenu.menu_id, (isMenuChoose ? "" : "display:none"));
                str.Append("    <ul class='nav navbar-nav-child mr-auto mt-2 mt-lg-0'>");

                foreach (var child in dicMenu[itemMenu.menu_id])
                {
                    str.Append(CreateItemMenu(child, (dicMenu.ContainsKey(child.menu_id) && dicMenu[child.menu_id].Count != 0), lstMenuChoose));
                }
                str.Append("    </ul>");
                str.Append("</li>");
                return str.ToString();
            }
            else
            {
                return CreateItemMenu(itemMenu, false, lstMenuChoose);
            }
        }

        private static string CreateItemMenu(MenuLeftViewModel menuItem, bool bParentId, List<int> lstMenuChoose, List<MenuLeftViewModel> lstChildMenu = null)
        {
            string sClassParentId = string.Empty;
            string sIcon = string.Empty;
            string sCssMenuChoose = string.Empty;
            string iConDown = string.Empty;
            bool bMenuChoose = false;
            if (lstMenuChoose.IndexOf(menuItem.menu_id) != -1) bMenuChoose = true;
            if (bParentId)
                sClassParentId = "parent-menu-left";
                iConDown = "bg-down-1";
            if (bMenuChoose)
                sCssMenuChoose = "color-active-menu";
            if (!string.IsNullOrEmpty(menuItem.icon))
                sIcon = string.Format("<i class='{0} fa-1x' aria-hidden='true'></i>", menuItem.icon);

            //string sMenu = string.Format("<a class='nav-link {4}' {5} href='{2}' data-id='{0}' {6}>{3}<span>{1}</span></a>",
            //                        menuItem.menu_id, menuItem.menu_name, (menuItem.sMenuLeftUrl), sIcon, sClassParentId, sCssMenuChoose,(bParentId? "data-collapse='1'":""));

            string sMenu = $"<a class='nav-link {sClassParentId} {sCssMenuChoose}' href='/{menuItem.sMenuLeftUrl}' data-id='{menuItem.menu_id}' {(bParentId ? "data-collapse='1'" : "")}>{sIcon}<span>{menuItem.menu_name} <div class='{iConDown}'></div></span></a>";
            if (lstChildMenu != null)
            {
                StringBuilder str = new StringBuilder();
                str.Append(@"<div class='dropdown-menu-hide active'>
                                <ul class='nav navbar-nav'>");
                foreach (var child in lstChildMenu)
                {
                    //str.AppendFormat(@"<li class='nav-item' data-collapse='1'>
                    //                <a class='nav-link' href='{0}' data-id='{1}'>{2}</a>
                    //            </li>", child.sMenuLeftUrl, child.menu_id, child.menu_name);
                    str.Append($"<li class='nav-item' data-collapse='1'><a class='nav-link' href='/{child.sMenuLeftUrl}' data-id='{child.menu_id}'>{child.menu_name}</a></li>");
                }
                str.Append(@"   </ul>
                            </div>");
                sMenu += str.ToString();
            }
            if (!bParentId)
            {
                return string.Format("<li class='nav-item'>{0}</li>", sMenu);
            }
            return sMenu;
        }
    }
}