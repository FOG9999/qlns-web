using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class GiaiNganThanhToanChiTietViewModel : VDT_TT_DeNghiThanhToan_ChiTiet
    {
        public string sNganh { get; set; }
        public string sTenDuAn { get; set; }
        public string sSoHopDong { get; set; }
        public string sTenNhaThau { get; set; }
        public decimal fDuToanGiaGoiThau { get; set; }
        public decimal fLuyKeThanhToanKLHT { get; set; }
        public decimal fChiTieuNganSachNam { get; set; }
        public decimal fSoThucThanhToanDotNay { get; set; }
        public string sTaiKhoanNganHang { get; set; }

    }
}
