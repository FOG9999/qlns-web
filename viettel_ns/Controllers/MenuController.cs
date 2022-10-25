using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{
    public class MenuController : AppController
    {
        private readonly ICommonService _common = CommonService.Default;

        public ActionResult MenuLeft(string id)
        {
            int iScreenId = 0;
            if (Session["iIdMenu"] != null)
            {
                iScreenId = (int)Session["iIdMenu"];
            }
            var lstMenu = _common.GetCurrentMenu(iScreenId, Username);
            string sParentName = string.Empty;
            if (lstMenu.Any()) lstMenu = lstMenu.Skip(1);
            lstMenu = MenuModels.GetMenuLeftByListMenu(iScreenId, lstMenu, ref sParentName);
            var listAllMenu = _common.GetAllMenu();
            if ((lstMenu != null && lstMenu.Any()) && (listAllMenu != null && listAllMenu.Any()))
            {
                foreach (var item in lstMenu)
                {
                    var model = listAllMenu.FirstOrDefault(x => x.iID_MaMenuItem == item.menu_id);
                    if (model != null)
                    {
                        item.thu_tu = model.tThuTu;
                    }
                }
            }
            lstMenu = lstMenu.OrderBy(x => x.thu_tu);
            ViewBag.sParentName = sParentName;
            IEnumerable<MENU_MenuItem> lst_parentMenu = _common.GetBreadcrumbStringByMenuId(iScreenId);
            if (lst_parentMenu != null)
            {
                ViewBag.MenuParent = lst_parentMenu.Select(n => n.iID_MaMenuItem).ToList();
            }
            else
            {
                ViewBag.MenuParent = new List<int>();
            }

            return PartialView("~/Views/Shared/Menu/_menuLeft.cshtml", lstMenu);
        }

        public ActionResult Header()
        {
            int iScreenId = 0;
            int iMenuColpanIndex = 0;
            int iMenuHeadFocus = 0;
            if (Session["iIdMenu"] != null)
            {
                iScreenId = (int)Session["iIdMenu"];
            }
            var lstMenu = _common.GetCurrentMenu(iScreenId, Username);
            if (lstMenu.Any()) lstMenu = lstMenu.Where(n => new List<int> { 1, 2 }.Contains(n.menu_level));
            var listAllMenu = _common.GetAllMenu();
            if ((lstMenu != null && lstMenu.Any()) && (listAllMenu != null && listAllMenu.Any()))
            {
                foreach (var item in lstMenu)
                {
                    var model = listAllMenu.FirstOrDefault(x => x.iID_MaMenuItem == item.menu_id);
                    if (model != null)
                    {
                        item.thu_tu = model.tThuTu;
                    }
                }
            }
            IEnumerable<MENU_MenuItem> lst_parentMenu = _common.GetBreadcrumbStringByMenuId(iScreenId);
            if (lst_parentMenu.Count() >= 1)
            {
                iMenuColpanIndex = lst_parentMenu.FirstOrDefault().iID_MaMenuItem;
                if (lst_parentMenu.Count() >= 2)
                    iMenuHeadFocus = lst_parentMenu.Skip(1).FirstOrDefault().iID_MaMenuItem;
            }
            ViewBag.iMenuColpanIndex = iMenuColpanIndex;
            ViewBag.iMenuHeadFocus = iMenuHeadFocus;

            return PartialView("~/Views/Shared/Menu/_headerLayout.cshtml", lstMenu);
        }

        [HttpPost]
        public void SetIdSessionMenuLink(int iScreenId)
        {
            Session["iIdMenu"] = iScreenId;
        }

        [HttpPost]
        public JsonResult GetMenuLink(int iScreenId)
        {
            var result = new MenuLeftTreeViewModel();
            Session["iIdMenu"] = iScreenId;
            var listAllMenu = _common.GetAllMenu();
            if (listAllMenu != null && listAllMenu.Any())
            {
                listAllMenu = listAllMenu.Where(x => x.bHoatDong == true).ToList();
                if(listAllMenu != null && listAllMenu.Any())
                {
                    var listResult = new List<MenuLeftTreeViewModel>();
                    var listMenuParent = new List<MENU_MenuItem>();
                    var menuItem = listAllMenu.Where(x => x.iID_MaMenuItemCha == iScreenId && x.bHoatDong == true).OrderBy(x => x.tThuTu).FirstOrDefault();
                    if (menuItem != null)
                    {
                        listMenuParent.Add(menuItem);
                    }
                    if (listMenuParent != null && listMenuParent.Any())
                    {
                        foreach (var item in listMenuParent)
                        {
                            var model = new MenuLeftTreeViewModel();
                            model.menu_id = item.iID_MaMenuItem;
                            model.menu_level = 1;
                            model.menu_name = item.sTen;
                            model.url = item.sURL;
                            model.thu_tu = item.tThuTu;
                            var listChild = GetTreeMenu(item.iID_MaMenuItem, listAllMenu.ToList(), model.menu_level + 1);
                            if (listChild != null && listChild.Any())
                            {
                                model.ListChild = new List<MenuLeftTreeViewModel>();
                                model.ListChild.AddRange(listChild);
                            }
                            listResult.Add(model);

                        }
                    }

                    var listFlatResult = new List<MenuLeftTreeViewModel>();
                    if (listResult != null && listResult.Any())
                    {
                        foreach (var item in listResult)
                        {
                            listFlatResult.Add(item);
                            if (item.ListChild != null && item.ListChild.Any())
                            {
                                var listFlat = GetFlatMenu(item.ListChild);
                                if (listFlat != null && listFlat.Any())
                                {
                                    listFlatResult.AddRange(listFlat);
                                }
                            }
                        }
                    }

                    if (listFlatResult != null && listFlatResult.Any())
                    {
                        var modelMaxLevel = listFlatResult.OrderByDescending(x => x.menu_level).FirstOrDefault();
                        result = listFlatResult.Where(x => x.menu_level == modelMaxLevel.menu_level).OrderBy(x => x.thu_tu).FirstOrDefault();
                    }
                    else
                    {
                        var modelItem = listAllMenu.FirstOrDefault(x => x.iID_MaMenuItem == iScreenId);
                        if (modelItem != null)
                        {
                            result.icon = modelItem.sIcon;
                            result.menu_id = modelItem.iID_MaMenuItem;
                            result.url = modelItem.sURL;
                        }
                    }
                }
            }

            result.url = $"/{result.url}";

            return Json(result);
        }

        public List<MenuLeftTreeViewModel> GetFlatMenu(List<MenuLeftTreeViewModel> listMenu)
        {
            var result = new List<MenuLeftTreeViewModel>();
            if(listMenu != null && listMenu.Any())
            {
                foreach (var item in listMenu)
                {
                    result.Add(item);
                    if(item.ListChild != null && item.ListChild.Any())
                    {
                        var listChild = GetFlatMenu(item.ListChild);
                        if(listChild != null && listChild.Any())
                        {
                            result.AddRange(listChild);
                        }
                    }
                }
            }
            return result;
        }

        public List<MenuLeftTreeViewModel> GetTreeMenu(int idMenu, List<MENU_MenuItem> listAllMenu, int level)
        {
            var result = new List<MenuLeftTreeViewModel>();
            if (listAllMenu != null && listAllMenu.Any())
            {
                var listResult = new List<MENU_MenuItem>();
                var menuItem = listAllMenu.Where(x => x.iID_MaMenuItemCha == idMenu && x.bHoatDong == true).OrderBy(x => x.tThuTu).FirstOrDefault();
                if(menuItem != null)
                {
                    listResult.Add(menuItem);
                }
                if (listResult != null && listResult.Any())
                {
                    foreach (var item in listResult)
                    {
                        var model = new MenuLeftTreeViewModel();
                        model.menu_id = item.iID_MaMenuItem;
                        model.menu_level = level;
                        model.menu_name = item.sTen;
                        model.url = item.sURL;
                        model.thu_tu = item.tThuTu;
                        var listChild = GetTreeMenu(item.iID_MaMenuItem, listAllMenu, model.menu_level + 1);
                        if(listChild != null && listChild.Any())
                        {
                            model.ListChild = new List<MenuLeftTreeViewModel>();
                            model.ListChild.AddRange(listChild);
                        }
                        result.Add(model);
                    }
                }
            }

            return result;
        }
    }
}