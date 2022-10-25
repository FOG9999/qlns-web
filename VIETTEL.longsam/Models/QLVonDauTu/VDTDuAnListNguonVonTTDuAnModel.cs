using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnListNguonVonTTDuAnModel
    {
        public Guid? iID_DuAn_NguonVon { get; set; }
        public Guid iID_DuAnID { get; set; }

        public int iID_NguonVonID { get; set; }

        public string sTenNguonVon { get; set; }

        public double? fThanhTien { get; set; }
    }
    
        
}
