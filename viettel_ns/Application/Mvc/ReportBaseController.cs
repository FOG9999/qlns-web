using Dapper;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{
    public class ReportBaseController : AppController
    {
        public ReportBaseController()
        {
            string lang = "vi-VN";
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
        }

        //protected virtual JsonResult ToCheckboxList(ChecklistModel vm)
        //{
        //    var view = "~/Views/Shared/_CheckboxList.cshtml";
        //    var result = HamChung.RenderPartialViewToStringLoad(view, vm, this);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //protected virtual JsonResult ToCheckboxListJson(ChecklistModel vm)
        //{
        //    var view = "~/Views/Shared/_CheckboxList.cshtml";
        //    var result = HamChung.RenderPartialViewToStringLoad(view, vm, this);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}



        protected virtual JsonResult ds_ToIn(int count = 0, int colCount = 6, string id = "To")
        {
            var list = new List<SelectListItem>();

            if (count > 0)
            {
                var count_to = count <= colCount ? 1 : (count % colCount) == 0 ? (count / colCount) : (count / colCount) + 1;
                for (int i = 1; i <= count_to; i++)
                {
                    var to = i.ToString();
                    list.Add(new SelectListItem()
                    {
                        Value = to,
                        Text = "Tờ " + to,
                    });
                }
            }

            var vm = new ChecklistModel(id, new SelectList(list, "Value", "Text"));
            return ToCheckboxList(vm);
        }

        protected JsonResult Ds_ToIn(DataTable dt)
        {
            return ds_ToIn(dt.Rows.Count);
        }

        protected virtual string ViewFolder()
        {
            return null;
        }

        protected virtual string ViewFileName()
        {
            return $"{ViewFolder()}/{this.ControllerName()}.cshtml";
        }
    }


    [Authorize]
    public class FlexcelReportController : ReportBaseController
    {
        protected virtual ActionResult Print(ExcelFile xls, string ext = "pdf", string filename = null)
        {
            filename = filename ?? string.Format("{0}_{1}.{2}", this.ControllerName(), DateTime.Now.GetTimeStamp(), ext);
            return
                string.IsNullOrWhiteSpace(ext) || ext == "pdf" ?
                    xls.ToPdfContentResult(filename) :
                    xls.ToFileResult(filename);
        }

        //        protected virtual ActionResult PrintEmpty(string msg)
        //        {
        //            var filename = string.Format("{0}_{1}_empty.txt", this.ControllerName(), DateTime.Now.GetTimeStamp());
        //            var text = @"
        //Quản lý ngân sách

        //time:       {0}
        //user:       {1}
        //-----------------

        //{2}


        //";
        //            text = string.Format(text, DateTime.Now, Username, msg);
        //            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
        //            {

        //                ms.Position = 0;
        //                //return File(ms.ToArray(), "application/pdf");
        //                byte[] fileContents = ms.ToArray();

        //                var result = new FileContentResultWithContentDisposition(fileContents,
        //                    contentType: "application/pdf",
        //                    contentDisposition: new ContentDisposition()
        //                    {
        //                        Inline = true,
        //                        FileName = filename,
        //                    });

        //                return result;
        //            }
        //        }

        protected virtual ActionResult PrintEmpty(string msg)
        {
            var file = Server.MapPath(@"~/Report_ExcelFrom/_core/rptEmpty.xls");
            var xls = new XlsFile();
            xls.Open(file);

            var fr = new FlexCelReport();
            fr.UseCommonValue()
                .SetValue(new { ThongBao = msg })
                .Run(xls);
            return Print(xls);
        }

        #region Fill LNS

        protected virtual FlexCelReport FillDataTableLNS(FlexCelReport fr, DataTable data, int type, string iNamLamViec, string field = null)
        {

            var fields = "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM";
            List<string> lnsList = (fields).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("sMoTa"))
            {
                AddLnsMoTa(data, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", fields.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.Skip(type).ToList();
            for (int i = 0; i < 7 - type; i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : string.Format("{0},{1}", lns.Join(), field);
                var dt = dtSource.SelectDistinct(tableName, distinctField);//.AddLnsMoTa(iNamLamViec);
                AddLnsMoTa(dt, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);


                // nho relationshipo
                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            return fr;
        }

        protected virtual FlexCelReport FillDataTable(FlexCelReport fr, DataTable data, string fields = null, string iNamLamViec = null, string field = null)
        {
            iNamLamViec = iNamLamViec.IsEmpty() ? PhienLamViec.iNamLamViec : iNamLamViec;
            fields = fields.IsEmpty() ? "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM" : fields;


            //var fields = "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM";
            List<string> lnsList = (fields).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("sMoTa"))
            {
                AddLnsMoTa(data, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", fields.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count(); i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : string.Format("{0},{1}", lns.Join(), field);
                var dt = dtSource.SelectDistinct(tableName, distinctField);//.AddLnsMoTa(iNamLamViec);
                AddLnsMoTa(dt, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

                // nho relationshipo
                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            return fr;
        }

        protected virtual FlexCelReport FillDataTable_NG(FlexCelReport fr, DataTable data, string iNamLamViec, string field = null)
        {

            List<string> lnsList = ("sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM,sNG").Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("sMoTa"))
            {
                AddLnsMoTa(data, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", ("sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM,sNG").Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count; i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : string.Format("{0},{1}", lns.Join(), field);
                var dt = dtSource.SelectDistinct(tableName, distinctField);//.AddLnsMoTa(iNamLamViec);
                AddLnsMoTa(dt, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

                // nho relationshipo
                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            return fr;
        }
        protected virtual FlexCelReport FillDataTable_sTM(FlexCelReport fr, DataTable data, string iNamLamViec, string field = null)
        {

            List<string> lnsList = string.IsNullOrEmpty(field) ? ("sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM").Split(new char[] { ' ' }).ToList() : field.Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("sMoTa"))
            {
                AddLnsMoTa(data, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", string.IsNullOrEmpty(field) ? ("sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM").Split(' ').Join() : field.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count; i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : string.Format("{0},{1}", lns.Join(), field);
                var dt = dtSource.SelectDistinct(tableName, distinctField);//.AddLnsMoTa(iNamLamViec);
                AddLnsMoTa(dt, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

                // nho relationshipo
                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            return fr;
        }
        protected FlexCelReport FillDataTableLNS(FlexCelReport fr, DataTable data, FillLNSType type, string iNamLamViec, string field = null)
        {
            return FillDataTableLNS(fr, data, (int)type, iNamLamViec, field);
        }

        protected virtual FlexCelReport FillDataTableLNS(FlexCelReport fr, DataTable data, string typeLNS, string iNamLamViec, string field = null)
        {

            List<string> lnsList = (typeLNS).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);

            if (!data.GetColumnNames().Contains("sMoTa"))
            {
                AddLnsMoTa(data, iNamLamViec);
            }

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count; i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : string.Format("{0},{1}", lns.Join(), field);
                var dt = dtSource.SelectDistinct(tableName, distinctField);

                AddLnsMoTa(dt, iNamLamViec);

                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);
                
                lns.Remove(lns.Last());
                dtSource = dt;
            }

            return fr;
        }

        #endregion

        private DataTable AddLnsMoTa(DataTable dt, string iNamLamViec = null)
        {
            if (dt.GetColumnNames().Any(x => x == "sMoTa")) return dt;

            var count = dt.Columns.Count;
            dt.Columns.Add("sMoTa");

            var lns = "sLNS,sL,sK,sM,sTM,sTTM,sNG".Split(',').ToList();
            var columnNames = dt.GetColumnNames();

            dt.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var items = new List<string>();
                    lns.ForEach(l =>
                    {
                        if (columnNames.Contains(l))
                        {
                            //var c = r.Field<string>(l);
                            var value = r[l].ToString();
                            items.Add(value);
                        }
                    });


                    var key = items.Count == 0 ?
                    r.ItemArray[count - 1] :
                    items.Join("-");

                    if (string.IsNullOrWhiteSpace(iNamLamViec))
                        iNamLamViec = DateTime.Now.Year.ToString();

                    //r["sMoTa"] = getMLNS_MoTa(iNamLamViec, key.ToString());
                    r["sMoTa"] = getMLNS_MoTa2(iNamLamViec, key.ToString());

                    //r["sMoTa"] = getMLNS_MoTa_Cache(iNamLamViec, key.ToString());
                });


            return dt;
        }

        protected string GhiChu(DataTable dtGhiChu, string ghiChu, Func<DataRow, string> func)
        {
            var result = string.Empty;

            if (dtGhiChu != null && dtGhiChu.Rows.Count != 0)
            {
                var builder = new StringBuilder();

                result = dtGhiChu.AsEnumerable()
                    .Select(x => func(x))
                    .Join(Environment.NewLine);

            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                if (!string.IsNullOrWhiteSpace(ghiChu))
                {
                    result = result + Environment.NewLine + ghiChu;
                }
            }
            else
            {
                result = ghiChu;
            }

            return result;
        }

        #region MLNS mota

        //private string getMLNS_MoTa_Cache(string iNamLamViec, string xauNoiMa)
        //{
        //    var cacheKey = string.Format("MLNS_{0}", iNamLamViec);

        //    var mlns = CacheService.Default.CachePerRequest(cacheKey, () => getMLNS_List(iNamLamViec), CacheTimes.OneHour);
        //    //var entity = mlns.FirstOrDefault(x => x.sXauNoiMa == xauNoiMa ||
        //    //                (x.sL == "" && x.sLNS.Length == 7 && xauNoiMa.Length <= 7 && x.sLNS.Substring(0, xauNoiMa.Length) == xauNoiMa));


        //    var entity = mlns.FirstOrDefault(x => x.sXauNoiMa == xauNoiMa ||
        //                    (x.sL == "" && xauNoiMa.Length == 1 && x.sLNS.Substring(0, 1) == xauNoiMa));
        //    return entity == null ? "" : entity.sMoTa;

        //    //return "smota";
        //}

        protected string getMLNS_MoTa(string iNamLamViec, string lns)
        {
            var key = string.Format("lns_{0}_{1}", lns, iNamLamViec);

            if (_mlnsList.Count == 0)
            {
                _mlnsDate = DateTime.Now;
            }

            var delta = (DateTime.Now - _mlnsDate).Minutes;
            if (delta == 5)
            {
                _mlnsDate = DateTime.Now;
                _mlnsList.Clear();
            }

            if (_mlnsList.ContainsKey(key))
            {
                return _mlnsList[key];
            }

            var sql =
@"

        SELECT  TOP(1) sMoTa 
        FROM    NS_MucLucNganSach
        WHERE   iTrangThai =1
                AND iNamLamViec=dbo.f_ns_nammlns(@iNamLamViec) 
                AND (sXauNoiMa= @lns or (@lns='3' AND slNS='3010000' AND sL=''))
        ORDER BY sXauNoiMa

        ";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {

                var mota = conn.QueryFirstOrDefault<string>(
                    sql,
                    new
                    {
                        iNamLamViec,
                        lns
                    });

                _mlnsList.Add(key, mota);
                return mota;
            }

        }

        protected string getMLNS_MoTa2(string iNamLamViec, string lns)
        {
            var list = getMlns_full(iNamLamViec);
            var entity = list.FirstOrDefault(x => x.sXauNoiMa == lns);


            //return entity == null ? "" : entity.sMoTa;

            // test co nam 2017
            if (entity == null)
            {
                if (Convert.ToInt32(iNamLamViec) <= 2017)
                {
                    var row = getMLNS_2017().AsEnumerable().FirstOrDefault(x => x.Field<string>("XauNoiMa") == lns);
                    if (row == null) return string.Empty;
                    return row.Field<string>("MoTa");
                }
                return string.Empty;
            }
            else
            {
                return entity.sMoTa;
            }

        }

        private DataTable getMLNS_2017()
        {
            var cache_key = "mlns_2017";

            var dt = CacheService.Default.CachePerRequest(cache_key, () =>
            {
                var sql = @"

select lns
		,l = case l when '460' then '010' else l end
		,k = case k when '468' then '011' else k end
		,m,tm,ttm,ng,tng
        ,MoTa=ten
		,XauNoiMa=''
from Appsrv.[dmuc2009].[dbo].[MLNS]
order by lns,l,k,m,tm,ttm,ng,tng



";

                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    var t = conn.GetTable(sql);

                    t.AsEnumerable().ToList()
                    .ForEach(r =>
                    {
                        var list = new string[]
                        {
                            r.Field<string>("lns"),
                            r.Field<string>("l"),
                            r.Field<string>("k"),
                            r.Field<string>("m"),
                            r.Field<string>("tm"),
                            r.Field<string>("ttm"),
                            r.Field<string>("ng"),
                            r.Field<string>("tng"),
                        };

                        r["XauNoiMa"] = list.Where(x => x.IsNotEmpty()).Join("-");

                    });

                    return t;
                }
            });

            return dt;
        }

        private IEnumerable<NS_MucLucNganSach> getMlns_full(string iNamLamViec)
        {
            var list = NganSachService.Default.GetMLNS_All(iNamLamViec);
            return list;
        }




        protected string getMLNS_MoTa(string iNamLamViec, string lns, string sXauNoiMa)
        {
            var key = string.Format("lns_{0}_{1}", lns, iNamLamViec);

            if (_mlnsList.Count == 0)
            {
                _mlnsDate = DateTime.Now;
            }

            var delta = (DateTime.Now - _mlnsDate).Hours;
            if (delta == 1)
            {
                _mlnsDate = DateTime.Now;
                _mlnsList.Clear();
            }

            if (_mlnsList.ContainsKey(key))
            {
                return _mlnsList[key];
            }

            var sql =
@"

        SELECT  TOP(1) sMoTa 
        FROM    NS_MucLucNganSach
        WHERE   iTrangThai =1
                AND iNamLamViec=@iNamLamViec 
                AND (sXauNoiMa= @sXauNoiMa or (@lns='3' AND slNS='3010000' AND sL='') or (sLNS=@lns and LEN(sLNS)=7 and sL=''))
        ORDER BY sXauNoiMa

        ";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {

                var mota = conn.QueryFirstOrDefault<string>(
                    sql,
                    new
                    {
                        iNamLamViec,
                        lns,
                        sXauNoiMa
                    });

                _mlnsList.Add(key, mota);
                return mota;
            }

        }

        private IEnumerable<NS_MucLucNganSach> getMLNS_List(string iNamLamViec)
        {

            #region sql

            var sql = @"
SELECT * FROM F_MLNS(@iNamLamViec)
--ORDER BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG

";
            #endregion

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(
                    new CommandDefinition(
                         commandText: sql,
                         parameters: new
                         {
                             iNamLamViec,
                         },
                         commandType: CommandType.Text
                 ));

                return items.OrderBy(x => x.sXauNoiMa);
            }
        }

        private string getMLNS_MoTa_Cache(string iNamLamViec, string lns)
        {
            var key = string.Format("lns_{0}_{1}", lns, iNamLamViec);
            if (_mlnsList.ContainsKey(key))
            {
                return _mlnsList[key];
            }
            else
            {
                var mota = getMLNS_MoTa(iNamLamViec, lns);
                _mlnsList.Add(key, mota);

                return mota;
            }
        }

        private static Dictionary<string, string> _mlnsList = new Dictionary<string, string>();
        private static DateTime _mlnsDate;

        #endregion

        #region condtion query

        protected object ToParamPhongBan(string iID_MaPhongBan)
        {
            if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
            {
                return DBNull.Value;
            }
            else
            {
                if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "02" || iID_MaPhongBan == "11")
                {
                    return DBNull.Value;
                }
                else
                {
                    return iID_MaPhongBan;
                }
            }


        }
        #endregion


        public SelectList GetPhongBanList(INganSachService ngansachService, string id_phongban = null)
        {
            var isTLTHCuc = ngansachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHopCuc;
            var phongBanList = new Dictionary<string, string>();
            if (isTLTHCuc || string.IsNullOrWhiteSpace(GetPhongBanId(PhienLamViec.iID_MaPhongBan)))
            {
                phongBanList.Add("-1", "-- Tất cả các phòng ban --");

                ngansachService.GetPhongBanQuanLyNS(id_phongban).AsEnumerable().ToList()
                    .ForEach(r =>
                    {
                        phongBanList.Add(r.Field<string>("iID_MaPhongBan"), r.Field<string>("sMoTa"));
                    });
            }
            else
            {
                phongBanList.Add(PhienLamViec.iID_MaPhongBan, PhienLamViec.sTenPhongBanFull);
            }

            return phongBanList.ToSelectList();
        }

        public string GetPhongBanId(string iID_MaPhongBan)
        {
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "11" || iID_MaPhongBan == "02")
            {
                return string.Empty;
            }


            return iID_MaPhongBan;
        }

        protected string checkParam_Username(string id_phongban, INganSachService ngansachService = null)
        {
            var username = Username;

            // 
            if (string.IsNullOrWhiteSpace(GetPhongBanId(id_phongban)))
            {
                username = "";
            }
            else if (ngansachService != null)
            {
                var role = ngansachService.GetUserRoleType(Username);
                //if(role != UserRoleType.TroLyPhongBan || role !) 
            }

            return username;
        }

        #region xls (merge, check,...)


        #endregion
    }


    public enum FillLNSType
    {
        LNS1 = 0,
        LNS3 = 1,
        LNS5 = 2,
        LNS = 3
    };
}
