using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class NNSNganSachTheoNamLamViec
    {
        public string sMaNoiDungChi { get; set; }
        public decimal? iTongTien { get; set; }
        public string sTongTien
        {
            get
            {
                return this.iTongTien.HasValue ? this.iTongTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }

        //NinhNV start
        public decimal? fTongTien { get; set; }
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
