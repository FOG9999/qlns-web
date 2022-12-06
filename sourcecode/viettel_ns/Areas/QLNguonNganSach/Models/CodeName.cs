using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class CodeName
    {
        public Guid Code { get; set; }
        public string Name { get; set; }

        public CodeName(Guid aCode,string aName)
        {
            Code = aCode;
            Name = aName;
        }
    }
}