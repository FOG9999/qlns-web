using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class NNSDuToanExportDataModel
    {
        public int STT { get; set; }
        public string sMaLoaiDuToan { get; set; }
        public string sTenLoaiDuToan { get; set; }
        public string sSoChungTu { get; set; }
        public string sSoQuyetDinh { get; set; }
        public string dNgayQuyetDinh { get; set; }
        public string sSoCongVan { get; set; }
        public string dNgayCongVan { get; set; }
        public string sNoiDung { get; set; }
        public decimal? sSoTien { get; set; }
    }
}
