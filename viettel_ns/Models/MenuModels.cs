using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Models;

namespace VIETTEL.Models
{
    public class MenuModels
    {
        public static IEnumerable<MenuLeftViewModel> GetMenuLeftByListMenu(int iCurrentId, IEnumerable<MenuLeftViewModel> lstMenu, ref string sParentName)
        {
            var item = new MenuLeftViewModel();
            var dictionMenu = lstMenu.ToDictionary(x => x.parent_id);
            foreach (var m in dictionMenu)
            {
                if (string.IsNullOrEmpty(m.Key))
                {
                    continue;
                }
                var listKey = m.Key.Split('-');
                if(listKey != null && listKey.Any())
                {
                    if (!listKey.Contains(iCurrentId.ToString()))
                    {
                        continue;
                    }
                    item = m.Value;
                }
            }
            //MenuLeftViewModel item = lstMenu.FirstOrDefault(n => n.parent_id.Contains(iCurrentId.ToString()));
            //MenuLeftViewModel item = lstMenu.FirstOrDefault(n => n.parent_id.EndsWith($"_{iCurrentId.ToString()}"));
            if (item == null || item.menu_id == 0 || item.menu_level < 2) return new List<MenuLeftViewModel>();
            List<string> lst_Parent = item.parent_id.Split('-').ToList();

            string sParentId = lst_Parent[1];
            sParentName = lstMenu.FirstOrDefault(n => n.menu_id == int.Parse(sParentId)).menu_name;
            return lstMenu.Where(n => n.parent_id.Contains("-" + sParentId + "-"));
        }
    }
}