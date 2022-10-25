using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using DomainModel;
using DomainModel.Controls;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;
using System.Collections;
using VIETTEL.Flexcel;
using VIETTEL.Services;
using VIETTEL.Data;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1020700_TongHopController : AppController
    {
        //
        // GET: /rptDuToan_2_B8_TongHop/
        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath1 = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_1.xls";
        private const String sFilePath2 = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_2.xls";
        private const String sFilePath1_KG = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_1_TrinhKy.xls";
        private const String sFilePath2_KG = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_2_TrinhKy.xls";
        private DataTable dtSoTo;
        private ReportDataModel _data;
        private Viettel.Domain.DomainModel.DC_NguoiDungCauHinh _config;
        private readonly INganSachService _nganSachService;

        public rptDuToan_1020700_TongHopController()
        {
            _nganSachService = NganSachService.Default;
        }



        public ActionResult Index(string iID_MaPhongBan)
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_2_B8_TongHop.aspx";
                ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        public ActionResult EditSubmit(string ParentID, string iID_MaPhongBan)
        {
            String MaTo = Request.Form["MaTo"];
            String Nganh = Request.Form[ParentID + "_Nganh"];
            //String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String sLNS = Request.Form[ParentID + "_sLNS"];
            ViewData["PageLoad"] = "1";
            ViewData["MaTo"] = MaTo;
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["sLNS"] = sLNS;
            ViewData["Nganh"] = Nganh;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_2_B8_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        public ExcelFile CreateReport(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            var file = "";
            if (iID_MaPhongBan == "0")
            {
                file = ToSo == "1" ? sFilePath1 : sFilePath2; 
            }
            else
            {
                file = ToSo == "1" ? sFilePath1_KG : sFilePath2;
            }

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            _config = _config??(_config = _nganSachService.GetCauHinh(Username));
            _data = get_dtDuToan_1020700(Username, Nganh,ToSo,sLNS,iID_MaPhongBan);
            DataTable data = _data.dtDuLieu;
            data.TableName = "ChiTiet";

            var fr = new FlexCelReport();
            fr.AddTable("ChiTiet", data);
            data.Dispose();

            int i = 1;
            foreach (object obj in _data.arrMoTa1)
            {
                fr.SetValue("MoTa1_" + i, obj);
                i++;
            }
            i = 1;
            foreach (object obj in _data.arrMoTa2)
            {
                fr.SetValue("MoTa2_" + i, obj);
                i++;
            }
            i = 1;
            foreach (object obj in _data.arrMoTa3)
            {
                fr.SetValue("MoTa3_" + i, obj);
                i++;
            }
            //var sTenDonVi = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh", "iID", Nganh, "sTenNganh")); ;
            //fr.SetValue("sTenDonVi", sTenDonVi);

            fr.SetValue("ToSo", ToSo);
            fr.SetValues(_data.Values);

            fr.UseCommandValue()
              .UseChuKy(Username, iID_MaPhongBan)
              .UseChuKyForController(this.ControllerName(), iID_MaPhongBan)
              .Run(xls);
            return xls;
        }

        private ReportDataModel get_dtDuToan_1020700(String MaND, String Nganh, String ToSo, String sLNS, String iID_MaPhongBan)
        {
            //var v = Request.GetQueryStringValue("v");

            //return v == 2 ?
            //    get_dtDuToan_1020700_v2(sLNS, iID_MaPhongBan, ToSo) :
            //    get_dtDuToan_1020700_v1(MaND, Nganh, ToSo, sLNS, iID_MaPhongBan);

            return get_dtDuToan_1020700_v2(sLNS, iID_MaPhongBan, ToSo);

        }

        private ReportDataModel get_dtDuToan_1020700_v2(string sLNS, string iID_MaPhongBan, string toSo)
        {
            var data = new ReportDataModel();

            #region sql

            var sql =
@"SELECT	CT.iID_MaDonVi,
		    CT.iID_MaDonVi+' - '+ NS_DonVi.sTen AS TenDonVi, 
            sMoTa,
			NG,
            rTuChi,
            rHienVat,
		    (rTuChi + rHienVat) as TongSo		
FROM (	SELECT	iID_MaDonVi,
                sMoTa,
			    sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
				SUM(rTuChi+rDuPhong)/{0} AS rTuChi,
				SUM(rHienVat/{0}) AS rHienVat 
		FROM	DT_ChungTuChiTiet_PhanCap 
		WHERE	
                -- dk LNS
                {1} 
                MaLoai<>'1'  AND 
				(iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2 AND iID_MaNguonNganSach=1) AND 
				iID_MaPhongBanDich=@iID_MaPhongBan AND
                iTrangThai=1
		GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaDonVi
		HAVING	SUM(rTuChi+rDuPhong)>0 OR 
				SUM(rHienVat)>0 
		UNION 
		SELECT	iID_MaDonVi,
                sMoTa,
			    sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
				SUM(rTuChi+rDuPhong)/{0} AS rTuChi,
				SUM(rHienVat/{0}) AS rHienVat 
		FROM	DT_ChungTuChiTiet 
		WHERE	
                 -- dk LNS
                {1} 
                iID_MaPhongBan=  @iID_MaPhongBan AND 
				(iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2 AND iID_MaNguonNganSach=1)   AND 
				iTrangThai=1 
		GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaDonVi
		HAVING	SUM(rTuChi+rDuPhong)>0 OR 
				SUM(rHienVat)>0) CT  
		INNER JOIN (SELECT iID_MaDonVi as MaDonVi, sTen 
					FROM  NS_DonVi 
					WHERE iTrangThai=1 AND 
						iNamLamViec_DonVi=@iNamLamViec) as NS_DonVi 
				ON NS_DonVi.MaDonVi=CT.iID_MaDonVi
ORDER BY iID_MaDonVi";

            //var dkLns = @"  sLNS LIKE '2%' AND  
            //                sLNS NOT IN ('2060101', '2060102') AND";

            var dkLns = @"sLNS IN ('1020700') AND";

            sql = string.Format(sql, 1000, dkLns);

            #endregion

            #region execute query

            var nam = Request.GetQueryStringValue("nam");
            if (nam == 0)
                nam = _config.iNamLamViec;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iNamLamViec = nam,
                         iID_MaPhongBan
                     },
                     commandType: CommandType.Text
                 );

                var dtDonvi = dt.SelectDistinct("dtDonvi", "iID_MaDonVi,TenDonVi");
                var dtNgAll = dt.SelectDistinct("dtNg", "NG, sMoTa");
                List<DataRow> dtNg;
                var themCot = 0;
                // tinh trang
                if (toSo == "1")
                {
                    dtNg = dtNgAll.AsEnumerable().Take(5).ToList();

                    // tinh cot tong
                    dtDonvi.Columns.Add("TongTuChi", typeof(decimal));
                    dtDonvi.AsEnumerable().ToList()
                        .ForEach(r =>
                        {
                            var sum = dt.AsEnumerable()
                                .Where(x => x["iID_MaDonVi"].ToString() == r["iID_MaDonVi"].ToString())
                                .Sum(x => x.Field<decimal>("rTuChi") + x.Field<decimal>("rHienVat"));

                            r["TongTuChi"] = sum;
                        });

                    themCot = 5 - dtNg.Count();
                }
                else
                {
                    var to = int.Parse(toSo);
                    dtNg = dtNgAll.AsEnumerable().Skip(to * 6 - 1).Take(6).ToList();
                    themCot = 6 - dtNg.Count();
                }


                // cac cot mota
                if (themCot > 0)
                {
                    for (int i = 0; i < themCot; i++)
                    {
                        dtNg.Add(dtNgAll.NewRow());
                    }
                }

                for (int i = 0; i < dtNg.Count(); i++)
                {
                    var item = dtNg.ElementAt(i);
                    var cotName = "Cot" + (i + 1);

                    decimal sumHienVat = 0;

                    // them cot
                    dtDonvi.Columns.Add(cotName, typeof(decimal));
                    dtDonvi.AsEnumerable()
                        .ToList()
                        .ForEach(r =>
                        {
                            //var sum = dt.AsEnumerable()
                            //  .Where(x => x["iID_MaDonVi"].ToString() == r["iID_MaDonVi"].ToString() && x["NG"].ToString() == item["NG"].ToString())
                            //  .Select(x => x.Field<decimal>("rTuChi") + x.Field<decimal>("rHienVat"))
                            //  .Sum();

                            var items = dt.AsEnumerable()
                             .Where(x => x["iID_MaDonVi"].ToString() == r["iID_MaDonVi"].ToString() &&
                             x["NG"].ToString() == item["NG"].ToString());

                            var tuChi = items.Select(x => x.Field<decimal>("rTuChi")).Sum();
                            var hienVat = items.Select(x => x.Field<decimal>("rHienVat")).Sum();

                            sumHienVat += hienVat;
                            r[cotName] = tuChi + hienVat;

                        });

                    data.arrMoTa1.Add(item["NG"]);
                    data.arrMoTa2.Add(item["sMoTa"]);

                    var mota3 = string.Empty;
                    if (!string.IsNullOrWhiteSpace(item["NG"].ToString()))
                    {
                        mota3 = sumHienVat > 0 ? "Bằng hiện vật" : "Bằng tiền";
                    }
                    data.arrMoTa3.Add(mota3);
                }

                // danh lai chi so cot
                var index = 1;
                dtDonvi.AsEnumerable().ToList()
                   .ForEach(r =>
                   {
                       r["iID_MaDonVi"] = index++;
                   });

                data.dtDuLieuAll = dt;
                data.dtDuLieu = dtDonvi;

                data.Values.Add("TieuDe1", string.Format("Dự toán chi ngân sách nhà nước giao năm {0}", nam).ToUpper());
                data.Values.Add("TieuDe2", "Phần ngân sách bảo hiểm y tế");
            }

            #endregion


            return data;
        }

        public JsonResult Ds_DonVi(String ParentID, String Nganh, String ToSo, String sLNS, String iID_MaPhongBan)
        {
            String MaND = User.Identity.Name;
            if(String.IsNullOrEmpty(ToSo))
                ToSo = "1";
            DataTable dt = DanhSachToIn(MaND, Nganh, "1", sLNS, iID_MaPhongBan);

            if (String.IsNullOrEmpty(ToSo))
            {
                ToSo = Guid.Empty.ToString();
            }
            String ViewNam = "~/Views/DungChung/DonVi/To_DanhSach.ascx";
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, ToSo, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }
        private DataTable DanhSachToIn(String MaND, String Nganh, String ToSo,String sLNS,String iID_MaPhongBan)
        {
            _data = get_dtDuToan_1020700(MaND,Nganh,ToSo,sLNS,iID_MaPhongBan);

            DataTable dtToIn = new DataTable();
            dtToIn.Columns.Add("MaTo", typeof(String));
            dtToIn.Columns.Add("TenTo", typeof(String));
            DataRow R = dtToIn.NewRow();
            dtToIn.Rows.Add(R);
            R[0] = "1";
            R[1] = "Tờ 1";
            if (_data.dtDuLieuAll != null)
            {
                int a = 2;
                for (int i = 0; i < _data.dtDuLieuAll.Columns.Count - 10; i = i + 6)
                {
                    DataRow R1 = dtToIn.NewRow();
                    dtToIn.Rows.Add(R1);
                    R1[0] = a;
                    R1[1] = "Tờ " + a;
                    a++;
                }
            }
            return dtToIn;
        }

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(Nganh, ToSo, sLNS, iID_MaPhongBan);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(Nganh, ToSo, sLNS, iID_MaPhongBan);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}
