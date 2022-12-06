using FlexCel.Core;
using FlexCel.Render;
using System;
using System.IO;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Mvc;

namespace VIETTEL.Flexcel
{
    public class FileExts
    {
        public const string Xls = "xls";
        public const string Xlsx = "xlsx";
        public const string Pdf = "pdf";
    }

    public abstract class FlexcelFileResult : FileResult
    {
        private readonly string _filename;
        private readonly string _contentType;
        private readonly ExcelFile _xls;

        public FlexcelFileResult(ExcelFile xls, string contentType, string filename = null, string ext = null)
            : base(contentType)
        {
            _xls = xls;
            _filename = filename ?? string.Format("{0}.{1}", Guid.NewGuid(), ext);
        }


        protected virtual byte[] GetFileStream(ExcelFile xls)
        {
            using (var ms = new MemoryStream())
            {
                _xls.Save(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            response.Buffer = true;
            response.Clear();
            response.AddHeader("content-disposition", "attachment; filename=" + _filename);
            response.ContentType = _contentType;

            var fileContents = GetFileStream(_xls);
            response.BinaryWrite(fileContents);
            response.End();
        }
    }

    public class FlexExcelResult : FlexcelFileResult
    {
        public FlexExcelResult(ExcelFile xls, string filename = null) :
            base(xls, "application/excel", filename, FileExts.Xls)
        {

        }
    }

    public class FlexExcelXResult : FlexcelFileResult
    {
        public FlexExcelXResult(ExcelFile xls, string filename = null) :
            base(xls, "application/excel", filename, FileExts.Xlsx)
        {

        }

        protected override byte[] GetFileStream(ExcelFile xls)
        {
            using (var ms = new MemoryStream())
            {
                xls.Save(ms, TFileFormats.Xlsx);
                ms.Position = 0;
                return ms.ToArray();
            }
        }
    }
    public class FlexPdfResult : FlexcelFileResult
    {
        public FlexPdfResult(ExcelFile xls, string filename = null) :
            base(xls, "application/pdf", filename, FileExts.Pdf)
        {

        }

        protected override byte[] GetFileStream(ExcelFile xls)
        {
            using (var pdf = new FlexCelPdfExport())
            {
                pdf.Workbook = xls;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.BeginExport(ms);
                    pdf.ExportAllVisibleSheets(false, "BaoCao");
                    pdf.EndExport();

                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }
    }



    public class FlexFilePdfResult : ActionResult
    {
        private readonly string _filename;
        private readonly ExcelFile _xls;

        public FlexFilePdfResult(ExcelFile xls, string filename = null)
        {
            _xls = xls;
            _filename = filename ?? string.Format("{0}.pdf", Guid.NewGuid());
        }


        public override void ExecuteResult(ControllerContext context)
        {
            try
            {
                using (var pdf = new FlexCelPdfExport())
                {
                    pdf.Workbook = _xls;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pdf.BeginExport(ms);
                        pdf.ExportAllVisibleSheets(false, "BaoCao");
                        pdf.EndExport();

                        ms.Position = 0;

                        context.HttpContext.Response.Buffer = true;
                        context.HttpContext.Response.Clear();
                        //context.HttpContext.Response.AddHeader("content-disposition", "inline; filename=" + _filename);
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = _filename,

                            Inline = true,
                        };

                        //Response.AppendHeader("Content-Disposition", cd.ToString());
                        context.HttpContext.Response.AppendHeader("Content-Disposition", cd.ToString());
                        context.HttpContext.Response.ContentType = "application/pdf";

                        context.HttpContext.Response.BinaryWrite(ms.ToArray());
                        context.HttpContext.Response.End();
                    }

                }
            }
            catch
            {
            }
        }
    }

    public static class FlexcelResultExtension
    {
        public static FileContentResult ToFileContentResult(this ExcelFile xls, string contentType, ContentDisposition contentDisposition)
        {
            byte[] fileContents = null;
            using (var ms = new MemoryStream())
            {
                xls.Save(ms, TFileFormats.Pxl);
                ms.Position = 0;

                fileContents = ReadFully(ms);
            }
            var result = new FileContentResultWithContentDisposition(fileContents, contentType, contentDisposition);
            return result;
        }

        public static FileContentResult ToPdfContentResult(this ExcelFile xls, string filename = null)
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

        public static FileResult ToFileResult(this ExcelFile xls, string filename = null, string ext = FileExts.Xls)
        {
            if (ext == FileExts.Pdf)
            {
                return new FlexPdfResult(xls, filename);
            }
            else if (ext == FileExts.Xlsx)
            {
                return new FlexExcelXResult(xls, filename);
            }
            else
            {
                return new FlexExcelResult(xls, filename);
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static int TotalPageCount (this ExcelFile xls)
        {
            using (var image = new FlexCelImgExport(xls))
            {
                var info = image.GetFirstPageExportInfo();
                return info.TotalPages;
            }     
        }

        public static void ClearDiffFirstPage (this ExcelFile xls)
        {
            var header = xls.GetPageHeaderAndFooter();
            header.DiffFirstPage = false;            
            xls.SetPageHeaderAndFooter(header);
        }
        public static void AddPageFirstPage(this ExcelFile xls)
        {
            var header = xls.GetPageHeaderAndFooter();
            header.FirstHeader = "&C&\"Times New Roman\"&10&P";
            xls.SetPageHeaderAndFooter(header);
        }
    }
}
