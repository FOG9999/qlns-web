using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class NNSDotNhanExportDataModel
    {
        public int STT { get; set; }
        public string MaLoaiDuToan { get; set; }
        public string TenLoaiDuToan { get; set; }
        public string SoChungTu { get; set; }
        public string SoQuyetDinh { get; set; }
        public string NgayQuyetDinh { get; set; }
        public string NoiDung { get; set; }
        public decimal? SoTien { get; set; }
    }
}
