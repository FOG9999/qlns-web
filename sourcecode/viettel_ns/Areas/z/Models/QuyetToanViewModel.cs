using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Areas.QuyetToan.Models;

namespace VIETTEL.Areas.z.Models
{
    [Bind(Exclude = "FileList")]
    public class ChungTuViewModel_QuyetToan : ChungTuViewModel
    {
        public ChungTuViewModel_QuyetToan()
        {
            FileList = new SelectList(new string[] { });
        }

        [AutoMapper.IgnoreMap]
        public HttpPostedFileBase FileUpload { get; set; }

        public SelectList FileList { get; set; }

        public string File_List { get; set; }
        public string File_Data { get; set; }

    }
}