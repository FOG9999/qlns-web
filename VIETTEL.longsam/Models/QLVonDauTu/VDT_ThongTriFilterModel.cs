using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_ThongTriFilterModel
    {
        public string iID_ThongTriID { get; set; }
        public Guid? iID_DonViQuanLy { get; set; }
        public string iID_NguonVon { get; set; }
        public string iID_LoaiCongTrinh { get; set; }
        public int? KieuThongTri { get; set; }
        public int? NamThucHien { get; set; }
        public string MaThongTri { get; set; }
        public DateTime? NgayTaoThongTri { get; set; }
        public string NguoiLapThongTri { get; set; }
        public string TruongPhongTaiChinh { get; set; }
        public string ThuTruongDonVi { get; set; }
    }
}
