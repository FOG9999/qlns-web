using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_ThongTriViewModel : VDT_ThongTri
    {
        public int STT { get; set; }
        public string TenDonVi { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public string TenNguonVon { get; set; }
        public Guid? iID_LoaiCongTrinh { get; set; }
        public int iKieuThongTri { get; set; }
    }
}
