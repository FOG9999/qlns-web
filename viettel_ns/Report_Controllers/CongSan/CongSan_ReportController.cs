﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;

namespace VIETTEL.Report_Controllers.CongSan
{
    public class CongSan_ReportController : Controller
    {
        //
        // GET: /CongSan_Report/

        public string sViewPath = "~/Report_Views/CongSan/";
        public ActionResult Index()
        {
            return View(sViewPath + "Report_Index.aspx");
        }


    }
}
