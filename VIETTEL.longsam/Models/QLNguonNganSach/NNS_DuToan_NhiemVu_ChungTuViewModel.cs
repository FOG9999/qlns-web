using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class NNS_DuToan_NhiemVu_ChungTuViewModel
    {
        public Guid iID_MaChungTu { get; set; }
        public string iID_MaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public int iSoChungTu { get; set; }
        public DateTime dNgayChungTu { get; set; }
        public string sNgayChungTu
        {
            get
            {
                return dNgayChungTu.ToString("dd/MM/yyyy");
            }
        }
        public decimal SoTien { get; set; }
        public string sSoTien
        {
            get
            {
                return this.SoTien == 0 ? "0" : this.SoTien.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sNoiDung { get; set; }
    }
}
