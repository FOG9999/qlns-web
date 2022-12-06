using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKHVPhanBoVonViewModel : VDT_KHV_PhanBoVon
    {
        public string sNguonVon { get; set; }
        public string sLoaiNganSach { get; set; }
        public string sDonViQuanLy { get; set; }
        public decimal? fBoXung { get; set; }
        public int? iDieuChinh { get; set; }

    }
}
