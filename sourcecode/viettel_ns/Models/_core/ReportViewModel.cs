using System.Web.Mvc;

namespace VIETTEL.Models
{
    public class ReportViewModel
    {
        public string Id_PhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public SelectList PhongBanList { get; set; }



        public string TieuDe { get; set; }
    }
}
