using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoachVonNamViewModel
    {
        public Guid iId { get; set; }
        public int? iParentId { get; set; }
        public Guid iID_DuAnID { get; set; }
        public Guid iID_LoaiCongTrinhID { get; set; }
        public int iID_NguonVonID { get; set; }
        public Guid? iID_LoaiNguonVonID { get; set; }
        public Guid? iID_NhomQuanLyID { get; set; }
        public Guid? iID_NganhID { get; set; }
        public string sTenDuAn { get; set; }
        public string sXauNoiMa { get; set; }
        public string sMaKetNoi { get; set; }
        public decimal? fGiaTriDauTu { get; set; }
        public decimal? fVonDaBoTri { get; set; }
        public decimal? fChiTieuNamTruocChuaCap { get; set; }
        public decimal? fChiTieuNganSachDuocDuyet { get; set; }
        public decimal? fChiTieuDauNam { get; set; }
        public decimal? fThuHoiUngXDCBKhac { get; set; }
        public decimal? ChiTieuNganSachNam { get; set; }
        public string sGhiChu { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }
        public Guid? iID_CapPheDuyetID { get; set; }
    }
}
