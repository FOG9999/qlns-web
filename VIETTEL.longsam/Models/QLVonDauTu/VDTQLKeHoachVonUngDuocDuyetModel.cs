using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQLKeHoachVonUngDuocDuyetModel
    {
        public VDT_KHV_KeHoachVonUng dataKHVU { get; set; }
        public IEnumerable<VdtKhvKeHoachVonUngChiTietModel> listKHVUChiTiet { get; set; }

    }
}
