using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NhiemVuChungTuViewSearchModel
    {
        public string IdChungTu { get; set; }
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public int SoChungTu { get; set; }
        public string NgayChungTu { get; set; }
        public string SoTien { get; set; }
        public string NoiDung { get; set; }
    }
}