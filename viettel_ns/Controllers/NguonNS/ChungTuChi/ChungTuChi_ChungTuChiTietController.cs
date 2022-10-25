using DomainModel;
using DomainModel.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Controllers.NguonNS
{
    public class ChungTuChi_ChungTuChiTietController : AppController
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;

        public string sViewPath = "~/Views/NguonNS/ChungTuChi/ChungTuChiTiet/";

        [Authorize]
        public ActionResult Index(string Id_CTChi)
        {
            ViewData["Id_CTChi"] = Id_CTChi;
            return View(sViewPath + "ChungTuChiTiet_Index.aspx");
        }
        [Authorize]
        public ActionResult ChungTuChiTiet_Frame()
        {
            string Id_CTChi = Request.Form["ChungTuChi_ChungTuChiTiet_Id_CTChi"];

            ViewData["Id_CTChi"] = Id_CTChi;
            return View(sViewPath + "ChungTuChiTiet_Index_DanhSach_Frame.aspx");
        }        
    }
}
