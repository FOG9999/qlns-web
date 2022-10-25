using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class RptNNSDuToanChiTietModel
    {
        public string sMaKhoi { get; set; }
        public string sTenKhoi { get; set; }
        public string sMaNoiDungChi { get; set; }
        public string GhiChu { get; set; }
        public double? iTongTien { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string sNgayQuyetDinh
        { 
            get {
                return this.dNgayQuyetDinh.HasValue ? this.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
            } 
        }
        public string sSoQuyetDinh { get; set; }
        public string sTongTien
        {
            get
            {
                return this.iTongTien.HasValue ? this.iTongTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public int? iLineCount { get; set; }

        //NinhNV start
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        //NinhNV end

        public Guid iID_NhiemVu { get; set; }
        public string iID_MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public Guid? iID_NoiDungChi { get; set; }
        public Guid? iID_Parent { get; set; }
        public List<GiaTri> lstGiaTriPhanBo { get; set; }
        public List<GiaTri> lstGiaTriChoPheDuyet { get; set; }
    }

    public class PhongBanNhiemVu
    {
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public Guid iID_NhiemVu { get; set; }
        public double? iTongTien { get; set; }
        public List<GiaTri> lstGiaTriPhongBan { get; set; }
    }

    public class NhiemVu
    {
        public string sGhiChu { get; set; }
        public Guid iID_NhiemVu { get; set; }
        public double? iTongTien { get; set; }
        public List<GiaTri> lstGiaTriNhiemVu { get; set; }
    }

    public class PhongBan
    {
        public string sTenPhongBan { get; set; }
        public string sMaPhongBan { get; set; }
        public List<GiaTri> lstGiaTriPhongBan { get; set; }
        public double? iTongTien { get; set; }
    }
}
