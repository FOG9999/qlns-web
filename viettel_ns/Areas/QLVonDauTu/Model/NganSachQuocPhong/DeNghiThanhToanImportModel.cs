using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class DeNghiThanhToanImportModel
    {
        public string STT { get; set; }
        public string sSoDeNghi { get; set; }
        public string sNgayDeNghi { get; set; }
        public string sMaLoaiDeNghi { get; set; }
        public string sLoaiDeNghi => sMaLoaiDeNghi == "1" ? "Thanh toán" : "Tạm ứng";
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sSoHopDong { get; set; }
        public string sMaNguonVon { get; set; }
        public string sNguonVon { get; set; }
        public string sMaLoaiKeHoachVon { get; set; }
        public string sKeHoachVon { get; set; }
        public string sNamKeHoach { get; set; }
        public string sNoiDung { get; set; }
        public string sSoDeNghiThanhToanTn { get; set; }
        public string sSoDeNghiThanhToanNn { get; set; }
        public string sSoThuHoiTamUng_CheDoTn { get; set; }
        public string sSoThuHoiTamUng_CheDoNn { get; set; }
        public string sSoThuHoiTamUng_UngTruocTn { get; set; }
        public string sSoThuHoiTamUng_UngTruocNn { get; set; }
        public string BThanhToanTheoHD { get; set; }

    }
}