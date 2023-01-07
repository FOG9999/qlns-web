using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class VdtDaTtHopDongImportModel
    {
        public bool bIsError { get; set; }
        public string IStt { get; set; }
        public string sMaDuAn { get; set; }
        public string sSoHopDong { get; set; }
        public string sTenHopDong { get; set; }
        public string sNgayHopDong { get; set; }
        public string sMaLoaiHopDong { get; set; }
        public string sMaNhaThau { get; set; }
        public string iThoiGianThucHien { get; set; }
        public string fTienHopDong { get; set; }
    }
}