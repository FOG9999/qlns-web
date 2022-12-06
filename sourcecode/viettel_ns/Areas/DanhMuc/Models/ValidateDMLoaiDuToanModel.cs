using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class ValidateDMLoaiDuToanModel
    {
        public string iID_LoaiDuToan { get; set; }
        public string sMaLoaiDuToan { get; set; }
        public string STT { get; set; }
        public bool bDelete { get; set; }
    }
}