using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTPheDuyetVonUngNgoaiCTViewModel
    {
        public int iId { get; set; }
        public int? iParentId { get; set; }
        public Guid iID_DeNghiThanhToanID { get; set; }
        public Guid iID_DeNghiThanhToan_ChiTietID { get; set; }
        public Guid iID_DuAnID { get; set; }
        public Guid iID_CapPheDuyetID { get; set; }
        public string sSoDeNghi { get; set; }
        public string sNguoiLap { get; set; }
        public string sGhiChu { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaCapPheDuyet { get; set; }
        public DateTime? dNgayDeNghi { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public Guid iID_HopDongID { get; set; }
        public DateTime? dNgayHopDong { get; set; }
        public string sNganHang { get; set; }
        public double fTongGiaTriHD { get; set; }
        public Guid iID_NhaThauID { get; set; }
        public string sTenDonViQuanLy { get; set; }
        public string sTenNhomQuanLy { get; set; }
        public string sTenGoiThau { get; set; }
        public string sTenNhaThau { get; set; }
        public string sSoTaiKhoan { get; set; }
        public double fLKKHVUDuocDuyet { get; set; }
        public double fLKSoVonDaTamUng { get; set; }
        public double fLKThuHoiUng { get; set; }
        public double fGiaTriThanhToan { get; set; }
        public double fGiaTriThuHoiUngNgoaiChiTieu { get; set; }
        public double fTongTienTrungThau { get; set; }
    }
}
