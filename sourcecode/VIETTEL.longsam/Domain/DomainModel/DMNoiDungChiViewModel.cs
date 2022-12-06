using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Domain.DomainModel
{
    public class DMNoiDungChiViewModel : DM_NoiDungChi
    {
        public  string sMaNoiDungChiCha { get; set; }
        public string depth { get; set; }
        public string location { get; set; }

        public int fSoCon { get; set; }
        public bool bLaHangCha { get; set; }
        public Guid rootParent { get; set; }
        public double? SoTien { get; set; }
        public string sSoTien
        {
            get
            {
                return this.SoTien.HasValue ? this.SoTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "0";
            }
        }
    }
}
