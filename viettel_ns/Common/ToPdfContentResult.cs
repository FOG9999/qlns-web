using FlexCel.Core;
using FlexCel.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VIETTEL.Mvc;

namespace VIETTEL.Common
{
    public static class ToPdfContentResultXls
    {
        public static FileContentResult ToPdfContentResult( this ExcelFile xls, string filename = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = string.Format("{0}.pdf", Guid.NewGuid());
            }


            using (var pdf = new FlexCelPdfExport(xls, true))
            {
                //pdf.Workbook = xls;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.BeginExport(ms);
                    pdf.ExportAllVisibleSheets(false, "BaoCao");
                    pdf.EndExport();

                    ms.Position = 0;
                    //return File(ms.ToArray(), "application/pdf");
                    byte[] fileContents = ms.ToArray();

                    var result = new FileContentResultWithContentDisposition(fileContents,
                        contentType: "application/pdf",
                        contentDisposition: new ContentDisposition()
                        {
                            Inline = true,
                            FileName = filename,
                        });

                    return result;
                }
            }
        }
    }
}
