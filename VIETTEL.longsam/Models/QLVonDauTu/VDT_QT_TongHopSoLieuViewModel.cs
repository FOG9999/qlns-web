using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_QT_TongHopSoLieuViewModel : VDT_QT_TongHopSoLieu
    {
        public string TenDonViQuanLy { get; set; }
        public string TenNguonVon { get; set; }
    }
}
