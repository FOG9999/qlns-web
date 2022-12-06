using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.BaoHiemXaHoi
{
    public class ImportBHXHModel: BHXH_BenhNhanTempImport
    {

    }

    public class TTblImportLine
    {
        public int iLine { get; set; }
        public int iThang { get; set; }
    }
}
