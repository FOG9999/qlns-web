using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoachVonNamChiTietViewModel : VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet
    {
        public Guid? IdParentNew { get; set; }
    }
}
