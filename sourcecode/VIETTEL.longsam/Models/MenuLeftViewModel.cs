using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models
{
    public class MenuLeftViewModel
    {
        public int menu_id { get; set; }
        public string menu_name { get; set; }
        public int menu_level { get; set; }
        public string parent_id { get; set; }
        public int? thu_tu { get; set; }
        public int iParentId { 
            get {
                if (string.IsNullOrEmpty(parent_id)) return -1;
                var items = parent_id.Split('-');
                if (items.Length <= 1) return 0;
                return int.Parse(items[items.Length - 2]);
            } 
        }
        public string url { get; set; }
        public string sMenuLeftUrl
        {
            get
            {
                if (url.Contains("#")) return url;
                return url.Replace("~/","");
            }
        }
        public string icon { get; set; }
    }

    public class MenuLeftTreeViewModel : MenuLeftViewModel
    {
        public List<MenuLeftTreeViewModel> ListChild { get; set; }
    }
}
