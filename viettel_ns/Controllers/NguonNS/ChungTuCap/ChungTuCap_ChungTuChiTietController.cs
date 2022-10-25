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
    public class ChungTuCap_ChungTuChiTietController : AppController
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;

        public string sViewPath = "~/Views/NguonNS/ChungTuCap/ChungTuChiTiet/";

        [Authorize]
        public ActionResult Index(string Id_CTCap)
        {
            ViewData["Id_CTCap"] = Id_CTCap;
            return View(sViewPath + "ChungTuChiTiet_Index.aspx");
        }
        [Authorize]
        public ActionResult ChungTuChiTiet_Frame()
        {
            string Id_CTCap = Request.Form["ChungTuCap_ChungTuChiTiet_Id_CTCap"];

            ViewData["Id_CTCap"] = Id_CTCap;
            return View(sViewPath + "ChungTuChiTiet_Index_DanhSach_Frame.aspx");
        }        
    }
}
