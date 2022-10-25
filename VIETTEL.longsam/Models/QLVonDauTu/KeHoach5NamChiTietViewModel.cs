using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoach5NamChiTietViewModel: VDT_KHV_KeHoach5Nam_ChiTiet
    {
        public int iId { get; set; }
        public int? iParentId { get; set; }
        public string sTenDuAn { get; set; }
        public string sThoiGianThucHien { get; set; }
        public double fGiaTriDieuChinh { get; set; }
        public double fGiaTriSauDieuChinh { get; set; }
    }
}
