using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Models
{
    public class ModalModels
    {
        public string Title { get; set; }
        public string FunctionName { get; set; }
        public List<string> Messages { get; set; }
        /// <summary>
        /// 0: Modal loại Confirm
        /// 1: Modal loại Thông báo
        /// </summary>
        public byte Category { get; set; }
    }
}