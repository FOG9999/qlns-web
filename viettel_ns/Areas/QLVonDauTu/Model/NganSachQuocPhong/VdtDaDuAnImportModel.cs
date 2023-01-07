using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class VdtDaDuAnImportModel
    {
        public string IStt { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }
        public string sTenHangMuc { get; set; }
        public string sMaLoaiCongTrinh { get; set; }
        public string iID_NguonVonID { get; set; }
        public string sDiaDiem { get; set; }
        public string sMucTieu { get; set; }
        public string fHanMucDauTu { get; set; }
        public bool bIsError { get; set; }
    }
}