using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLThongTinHopDong
{
    public class HopDongDetailModel : VDT_DA_TT_HopDong
    {
        public decimal fGiaTriHD { get; set; }
        public decimal fDaThanhToanTrongNam { get; set; }
        public decimal fDaTamUng { get; set; }
        public decimal fDaThuHoi { get; set; }
        public decimal fLuyKeThanhToanKLHT { get; set; }
        public string sTenGoiThau { get; set; }
        public string sTenNhaThau { get; set; }
        public string sNganHang { get; set; }
        public string sDonViThuHuong { get; set; }
        public string sSoTaiKhoanNhaThau { get; set; }
        public string fDuToanGoiThau { get; set; }
    }
}
