using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class VdtDaGoiThauImportModel
    {
        public bool bIsError { get; set; }
        public string IStt { get; set; }
        public string sMaDuAn { get; set; }
        public string sSoQuyetDinh { get; set; }
        public string sNgayQuyetDinh { get; set; }
        public string sTenGoiThau { get; set; }
        public string fTienTrungThau { get; set; }
        public string iThoiGianThucHien { get; set; }
        public string sMaNhaThau { get; set; }
    }
}