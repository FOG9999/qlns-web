using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKeHoachVonUngDeXuatModel: VDT_KHV_KeHoachVonUng_DX
    {
        public string sTenDonVi { get; set; }
        public string sTenNguonVon { get; set; }
        public double? fTienKeHoachVUDX { get; set; }
    }
}
