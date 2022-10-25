using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_QT_TongHopSoLieuDetailViewModel
    {
        public VDT_QT_TongHopSoLieu Master { get; set; }
        public List<VDT_QT_TongHopSoLieu_ChiTiet> ListDetail { get; set; }
        public List<VDT_QT_XuLySoLieu> ListSoLieu { get; set; }
    }
}
