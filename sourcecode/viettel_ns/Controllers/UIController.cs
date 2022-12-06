using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;

namespace VIETTEL.Controllers
{

    public class MenuItemViewModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }
        public int OrderIndex { get; set; }

        public int Depth { get; set; }

        public bool IsLastMenu { get; set; }

        public IEnumerable<MenuItemViewModel> Menus { get; set; }
    }

    public class UIController : AppController
    {
        #region menu


        //        public JsonResult JMenu()
        //        {
        //            var sql = @"

        //SELECT  iID_MaLuat
        //FROM    QT_NguoiDung
        //INNER JOIN PQ_NhomNguoiDung_Luat 
        //        ON QT_NguoiDung.iID_MaNhomNguoiDung = PQ_NhomNguoiDung_Luat.iID_MaNhomNguoiDung

        //";
        //            using (var conn = ConnectionFactory.Default.GetConnection())
        //            {
        //                var iID_MaLuat = conn.QueryFirstOrDefault<Guid>(
        //                    new CommandDefinition(sql,
        //                        new
        //                        {
        //                            sID_MaNguoiDung = Username
        //                        }));

        //                sql = string.Format("SELECT * FROM f_menu('{0}')", iID_MaLuat);

        //                var list = conn.Query<dynamic>(sql)
        //                    .Select(x => new MenuItemViewModel()
        //                    {
        //                        Id = x.iID_MaMenuItem.ToString(),
        //                        ParentId = x.iID_MaMenuItemCha.ToString(),
        //                        Title = x.sTen,
        //                        Url = x.sURL,
        //                        OrderIndex = x.tThuTu,
        //                        Depth = x.Depth,
        //                    })
        //                    .ToList();
        //                var menus = buildMenu(list, "0");
        //                return Json(menus, JsonRequestBehavior.AllowGet);
        //            }

        //        }

        [HttpGet]
        public PartialViewResult Menu()
        {
#if DEBUG
            var menus = CacheService.Default.CachePerRequest("ns_menu", () => getMenus(), CacheTimes.OneMinute);
#else
            var menus = getMenus();
#endif
            return PartialView("_Menu", menus);
        }

        public IEnumerable<MenuItemViewModel> getMenus(string username = null)
        {
            if (username == null)
                username = Username;

            var sql = @"

SELECT  iID_MaLuat
FROM    QT_NguoiDung
INNER JOIN PQ_NhomNguoiDung_Luat 
        ON QT_NguoiDung.iID_MaNhomNguoiDung = PQ_NhomNguoiDung_Luat.iID_MaNhomNguoiDung
WHERE   sID_MaNguoiDung=@sID_MaNguoiDung

";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var iID_MaLuat = conn.QueryFirstOrDefault<Guid>(
                    new CommandDefinition(sql,
                        new
                        {
                            sID_MaNguoiDung = username
                        }));

                sql = string.Format("SELECT * FROM f_menu('{0}')", iID_MaLuat);

                var list = conn.Query<dynamic>(sql)
                    .Select(x => new MenuItemViewModel()
                    {
                        Id = x.iID_MaMenuItem.ToString(),
                        ParentId = x.iID_MaMenuItemCha.ToString(),
                        Title = x.sTen,
                        Url = "/" + x.sURL,
                        OrderIndex = x.tThuTu == null ? 0 : x.tThuTu,
                        Depth = x.depth,
                    })
                    .ToList();
                var menus = buildMenu(list, "0");
                return menus;
            }
        }

        private IEnumerable<MenuItemViewModel> buildMenu(IList<MenuItemViewModel> source, string rootId = null)
        {
            var menus = new List<MenuItemViewModel>();

            // level 1
            var slideItems = source.Where(x => x.ParentId == rootId).OrderBy(x => x.OrderIndex).ToList();
            if (slideItems.Count() > 0)
            {
                slideItems.ForEach(x => source.Remove(x));

                // add childs
                var index = 1;
                slideItems.ForEach(x =>
                {
                    if (index < slideItems.Count)
                    {
                        index++;
                    }
                    else
                    {
                        x.IsLastMenu = true;
                    }
                    menus.Add(x);



                    if (source.Count > 0)
                        x.Menus = buildMenu(source, x.Id);
                    else
                        x.Menus = new List<MenuItemViewModel>();
                });
            }
            else
            {

                //menus.AddRange(source);
            }
            return menus;

        }
    }

    #endregion
}
