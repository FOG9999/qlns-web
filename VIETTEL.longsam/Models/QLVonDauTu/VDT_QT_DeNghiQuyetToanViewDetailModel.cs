using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_QT_DeNghiQuyetToanViewDetailModel : VDT_QT_DeNghiQuyetToan
    {
        public string TenDonViQuanLy { get; set; }
        public string TenDuAn { get; set; }
        public string TenChuDauTu { get; set; }
    }
}
