using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Domain.DomainModel
{
    public class DanhMucNguonViewModel : DM_Nguon
    {
        public  string sMaNguonCha { get; set; }
        public  decimal? SoTien { get; set; }
        public  string GhiChu { get; set; }
        public string depth { get; set; }
        public string location { get; set; }
    }
}
