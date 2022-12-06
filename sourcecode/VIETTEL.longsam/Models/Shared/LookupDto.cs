using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.Shared
{
    public class LookupDto<TKey, TValue>
    {
        public virtual TKey Id { get; set; }
        public virtual TValue DisplayName { get; set; }
    }
}
