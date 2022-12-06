using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.Component
{
    public class AttachmentController : AppController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;

        // GET: Shared/Attachment
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //public JsonResult UploadFiles(Guid ID, int moduleType)
        public JsonResult UploadFiles()
        {
            HttpFileCollectionBase fileBaseCollection = Request.Files;
            List<Attachment> lstAttachments = new List<Attachment>();
            Guid ID = Guid.Empty;
            int moduleType = 0;
            if (Request.Params["ID"] != null)
                ID = Guid.Parse(Request.Params["ID"]);
            //ID = Request.Params["ID"];
            if (Request.Params["moduleType"] != null)
                moduleType = int.Parse(Request.Params["moduleType"]);

            for (int index = 0; index < fileBaseCollection.Count; index++)
            {
                Attachment attachment = new Attachment();
                HttpPostedFileBase file = fileBaseCollection[index];

                var fileName = Path.GetFileName(file.FileName);

                string filePath = DateTime.Now.ToString("yyyy-MM") + "\\VDT\\ChuTruongDauTu\\" + Guid.NewGuid() + fileName.Substring(fileName.LastIndexOf('.'));
                var sDirectoryPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), DateTime.Now.ToString("yyyy-MM"), "VDT\\ChuTruongDauTu");
                if (!Directory.Exists(sDirectoryPath))
                {
                    Directory.CreateDirectory(sDirectoryPath);
                }
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), filePath);

                attachment.CreatedDate = DateTime.Now;
                attachment.CreatedUser = Username;
                attachment.FileName = fileName;
                attachment.FilePath = filePath;
                attachment.ModuleType = moduleType;
                attachment.ObjectId = ID;
                attachment.dNgayCanCu = DateTime.Now;

                try
                {
                    file.SaveAs(path);
                    lstAttachments.Add(attachment);
                }
                catch (Exception ex)
                {
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return Json(new { status = false, data = ex });
                }
            }
            bool status = _iQLVonDauTuService.SaveListAttachment(lstAttachments);

            return Json(new { status = status });
        }

        [HttpPost]
        public JsonResult DieuChinhUploadFile(List<Guid> taiLieuIDs, Guid ObjectID)
        {
            List<Attachment> lstAttachments = new List<Attachment>();

            taiLieuIDs.ForEach(taiLieuID =>
            {
                Attachment srcAttachment = _iQLVonDauTuService.GetAttachment(taiLieuID);
                Attachment copyAttachment = new Attachment();


                var copyFileName = Path.GetFileName(srcAttachment.FileName);
                string copyFilePath = DateTime.Now.ToString("yyyy-MM") + "\\VDT\\ChuTruongDauTu\\" + Guid.NewGuid() + copyFileName.Substring(copyFileName.LastIndexOf('.'));
                var sDirectoryPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), DateTime.Now.ToString("yyyy-MM"), "VDT\\ChuTruongDauTu");
                if (!Directory.Exists(sDirectoryPath))
                {
                    Directory.CreateDirectory(sDirectoryPath);
                }
                var srcPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), srcAttachment.FilePath);
                var copyPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), copyFilePath);


                copyAttachment.CreatedDate = DateTime.Now;
                copyAttachment.CreatedUser = Username;
                copyAttachment.FileName = copyFileName;
                copyAttachment.FilePath = copyFilePath;
                copyAttachment.ModuleType = srcAttachment.ModuleType;
                copyAttachment.ObjectId = ObjectID;
                copyAttachment.dNgayCanCu = DateTime.Now;


                try
                {
                    System.IO.File.Copy(srcPath, copyPath);
                    lstAttachments.Add(copyAttachment);
                }
                catch (Exception ex)
                {
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    //return Json(new { status = false, ex = ex });
                }
            });
            bool status = _iQLVonDauTuService.SaveListAttachment(lstAttachments);
            return Json(new { status = status });
        }

        [HttpPost]
        public JsonResult GetTaiLieuDinhKemByID(Guid ID)
        {
            IEnumerable<Attachment> lstAttachment = _iQLVonDauTuService.GetAllAttachmentByID(ID);
            if (lstAttachment != null)
            {
                return Json(new { status = true, lstAttachment = lstAttachment }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteTaiLieuDinhKemByID(Guid AttachmentId)
        {
            Attachment attachment = _iQLVonDauTuService.GetAttachment(AttachmentId);

            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), attachment.FilePath);
            bool status = false;
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                status = _iQLVonDauTuService.DeleteAttachment(AttachmentId);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileInfo DownloadTaiLieuDinhKem(Guid AttachmentId)
        {
            Attachment attachment = _iQLVonDauTuService.GetAttachment(AttachmentId);
            var fileType = attachment.FileName.Substring(attachment.FileName.LastIndexOf('.'));

            if (attachment != null)
            {
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), attachment.FilePath);
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    Response.ContentType = "application/octet-stream";
                    Response.Clear();
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + attachment.FileName);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    //Response.ContentType = "application/octet-stream";
                    Response.ContentType = GetContentType(fileType);
                    Response.WriteFile(file.FullName);
                    Response.End();
                }
            }
            return null;
        }
        [HttpGet]
        public ActionResult XemTaiLieuDinhKem(Guid AttachmentId)
        {
            Attachment attachment = _iQLVonDauTuService.GetAttachment(AttachmentId);
            if (attachment != null)
            {
                var fileType = attachment.FileName.Substring(attachment.FileName.LastIndexOf('.'));
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), attachment.FilePath);
                return File(path, GetContentType(fileType));
            }
            return null;
        }

        public string GetContentType(string filetype)
        {
            filetype = filetype.ToUpper();
            var contentType = "";
            switch (filetype.ToUpper())
            {
                case ".PDF":
                    contentType = "application/pdf";
                    break;
                case ".XLSX":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".XLS":
                    contentType = "application/vnd.ms-excel";
                    break;
                case ".PNG":
                    contentType = "image/png";
                    break;
                case ".JPEG":
                    contentType = "image/jpeg";
                    break;
                case ".JPG":
                    contentType = "image/jpeg";
                    break;
                case ".DOC":
                    contentType = "application/msword";
                    break;
                case ".DOCX":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            return contentType;
        }
    }
}