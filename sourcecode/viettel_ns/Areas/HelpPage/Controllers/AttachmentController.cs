using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.HelpPage.Controllers
{
    public class AttachmentController : AppController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;

        // GET: HelpPage/Attachment
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadFiles(Guid ID, int moduleType)
        {
            HttpFileCollectionBase fileBaseCollection = Request.Files;
            List<Attachment> lstAttachments = new List<Attachment>();

            for (int index = 0; index < fileBaseCollection.Count; index++)
            {
                Attachment attachment = new Attachment();
                HttpPostedFileBase file = fileBaseCollection[index];

                var fileName = Path.GetFileName(file.FileName);

                string filePath = DateTime.Now.ToString("yyyy-mm") + "\\VDT\\ChuTruongDauTu\\" + Guid.NewGuid() + fileName.Substring(fileName.LastIndexOf('.'));
                var sDirectoryPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), DateTime.Now.ToString("yyyy-mm"), "VDT\\ChuTruongDauTu");
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
            bool status = _iQLVonDauTuService.DeleteAttachment(AttachmentId);
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}