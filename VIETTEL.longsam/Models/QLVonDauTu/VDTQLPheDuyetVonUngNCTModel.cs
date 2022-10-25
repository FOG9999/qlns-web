using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQLPheDuyetVonUngNCTModel
    {
        public VDT_TT_DeNghiThanhToanUng dataDNTTU { get; set; }
        public IEnumerable<VDTPheDuyetVonUngNgoaiCTViewModel> listDNTTUChiTiet { get; set; }

    }
}
