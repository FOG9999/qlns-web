using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class NNSDuToanChiTietGetSoTienByDonViModel
    {
        public string sMaNoiDungChi { get; set; }
        public string sTenNoiDungChi { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public string iID_MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public decimal SoTienXauNoiMa { get; set; }
    }
}
