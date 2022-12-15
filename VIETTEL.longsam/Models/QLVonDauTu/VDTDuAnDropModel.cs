using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnDropViewModel : VDT_DA_DuAn
    {
        public string sTenDuAnShow { get 
            {
                return String.Concat(sMaDuAn, "-", sTenDuAn);
            } 
        }
    }
}
