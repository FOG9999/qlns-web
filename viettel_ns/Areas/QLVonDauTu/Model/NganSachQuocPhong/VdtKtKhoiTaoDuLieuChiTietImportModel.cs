using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class VdtKtKhoiTaoDuLieuChiTietImportModel
    {
        public bool bIsError { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaLoaiCongTrinh { get; set; }
        public string fKHVN_VonBoTriHetNamTruoc { get; set; }
        public string fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc { get; set; }
        public string fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi { get; set; }
        public string fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc { get; set; }
        public string fKHVN_KeHoachVonKeoDaiSangNam { get; set; }
        public string fKHUT_VonBoTriHetNamTruoc { get; set; }
        public string fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc { get; set; }
        public string fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi { get; set; }
        public string fKHUT_KeHoachUngTruocKeoDaiSangNam { get; set; }
        public string fKHUT_KeHoachUngTruocChuaThuHoi { get; set; }
    }
}