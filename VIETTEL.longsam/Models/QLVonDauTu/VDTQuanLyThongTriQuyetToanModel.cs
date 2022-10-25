using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
   public class VDTQuanLyThongTriQuyetToanModel
    {
    }
    public class VDTGetThongTriChiTietViewModel
    {
        public Guid IIdDuAnId { get; set; }
        public string STenDuAn { get; set; }
        public Guid IIdLoaiCongTrinhId { get; set; }
        public string STenLoaiCongTrinh { get; set; }
        public string LNS { get; set; }
        public string L { get; set; }
        public string K { get; set; }
        public string M { get; set; }
        public string TM { get; set; }
        public string TTM { get; set; }
        public string NG { get; set; }
        public Guid IIdMucLucNganSach { get; set; }
        public double? FSoTien { get; set; }
        public Guid? IIdMucId { get; set; }
        public Guid? IIdTieuMucId { get; set; }
        public Guid? IIdTietMucId { get; set; }
        public Guid? IIdNganhId { get; set; }
    }

}
