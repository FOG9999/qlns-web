using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;


namespace Viettel.Models.QLVonDauTu
{
    public class VDTQuanLyDuAnNguonVonModel
    {
        public Guid iIDDuAn_NguonVon { get; set; }
        public Guid iID_DuAn { get; set; }
        public int iID_NguonVonID { get; set; }
        public float fThanhTien { get; set; }

    }
}
