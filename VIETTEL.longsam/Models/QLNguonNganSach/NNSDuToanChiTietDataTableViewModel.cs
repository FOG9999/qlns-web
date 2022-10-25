using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class NNSDuToanChiTietDataTableViewModel
    {
        public string iID_DuToanChiTiet { get; set; }
        public string sMaNoiDungChi { get; set; }
        public string sTenNoiDungChi { get; set; }
        public string iID_MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public decimal SoTien { get; set; }
        public string GhiChu { get; set; }
        public bool bLaHangCha { get; set; }
        public bool bConLai { get; set; }
        public bool iThayDoi { get; set; }
        public string iID_NhiemVu { get; set; }
        public string iID_MaChungTu { get; set; }

        public string depth { get; set; }
        public string location { get; set; }

        public Guid rootParent { get; set; }
        public Guid? iID_NoiDungChi { get; set; }
        public Guid? iID_NoiDungChiCha { get; set; }
    }
}
