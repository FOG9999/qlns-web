using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class DotNhanBoSungTrongNamViewModel
    {
        public  Guid iID_Nguon { get; set; }
        public  DateTime dNgayQuyetDinh { get; set; }
        public string sSoQuyetDinh { get; set; }
        public  decimal SoTien { get; set; }
        public  decimal TongSoTien { get; set; }
        public int SoCot { get; set; }

    }

    public class HeaderInfo
    {
        public string sTen { get; set; }
        public int iSoCon { get; set; }
        public int iCha { get; set; }
        public string sMergeRange { get; set; }
        public Guid? iID_Header { get; set; }
    }

    public class Header
    {
        public List<HeaderInfo> lstHeaderLv1 { get; set; }
        public List<HeaderInfo> lstHeaderLv2 { get; set; }
        public List<HeaderInfo> lstHeaderLv3 { get; set; }
        //public string MergeRangeLv1 { get; set; }
        //public string MergeRangeLv2 { get; set; }
        //public string MergeRangeLv3 { get; set; }
    }
}
