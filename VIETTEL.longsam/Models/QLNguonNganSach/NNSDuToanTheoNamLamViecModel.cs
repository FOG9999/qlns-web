using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class NNSDuToanTheoNamLamViecModel
    {
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public Decimal? iTongTien { get; set; }
        public string sMaNoiDungChi { get; set; }
        public string sTongTien
        {
            get
            {
                return this.iTongTien.HasValue ? this.iTongTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }

        //NinhNV start
        public Decimal? fTongTien { get; set; }
        public string sFTongTien
        {
            get
            {
                return this.fTongTien.HasValue ? this.fTongTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public Guid rootParent { get; set; }
        public Guid? iID_Parent { get; set; }
        public Guid? iID_NoiDungChi { get; set; }
        //NinhNV end
    }
}
