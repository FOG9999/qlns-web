using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNH
{
    public class DanhMucChuongTrinhViewModel
    {
    }

    public class DanhMucChuongTrinhFilter
    {
        public virtual string sTenNhiemVuChi { get; set; }
        public virtual Guid? iID_BQuanLyID { get; set; }
        public virtual Guid? iID_DonViID { get; set; }
    }
}
