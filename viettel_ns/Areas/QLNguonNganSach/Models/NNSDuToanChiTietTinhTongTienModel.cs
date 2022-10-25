using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NNSDuToanChiTietTinhTongTienModel
    {
        public Guid iID_DuToan { get; set; }
        public Guid iID_DuToanChiTiet { get; set; }
        public string sMaNoiDungChi { get; set; }
        public string sTenNoiDungChi { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public int iID_MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public decimal SoTienDonVi { get; set; }
        public decimal SoTienXauNoiMa { get; set; }
    }
}