using DomainModel;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Viettel.Services;
using VIETTEL.Application.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Flexcel
{
    public static class FlexcelExtension
    {
        public static FlexCelReport AddTable(this FlexCelReport fr, DataTable dt)
        {
            fr.AddTable(dt.TableName, dt);
            return fr;
        }

        public static FlexCelReport AddTableEmpty(this FlexCelReport fr, int count, int limit = 10)
        {
            var dt = new DataTable("empty");
            dt.Columns.Add("id");
            dt.Columns.Add("text");

            if (count < limit)
            {
                var delta = limit - count;
                for (int i = 0; i < delta; i++)
                {
                    var row = dt.NewRow();
                    row["id"] = i + 1;
                    dt.Rows.Add(row);
                }
            }

            fr.AddTable(dt);
            return fr;
        }

        public static FlexCelReport SetValues(this FlexCelReport fr, Dictionary<string, object> values)
        {
            values.ToList()
                .ForEach(x =>
                {
                    fr.SetValue(x.Key, x.Value);
                });

            return fr;
        }

        public static FlexCelReport SetValue(this FlexCelReport fr, dynamic param)
        {
            var dic = new Dictionary<string, object>();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(param))
            {
                var value = prop.GetValue(param);
                var name = prop.Name;
                dic.Add(name, value);
            }
            dic.ToList()
                .ForEach(x =>
                {
                    fr.SetValue(x.Key, x.Value);
                });

            return fr;
        }

        public static FlexCelReport UseCommonValue(this FlexCelReport fr,
            FlexcelModel model = null, int dvt = 1000)
        {
            if (model == null)
            {
                model = new FlexcelModel();
            }

            model.ToFlexcel(fr);
            return fr;
        }

        public static FlexCelReport UseChuKy(this FlexCelReport fr,
          string username = null,
          string iID_MaPhongBan = null,
          INganSachService ngansachService = null,
          string userDefault = "trolyphongban")
        {

            if (ngansachService == null)
                ngansachService = NganSachService.Default;
            username = username ?? HttpContext.Current.User.Identity.Name;

            var sTenPhongBan = "";

            //if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
            //{
            //    iID_MaPhongBan = NguoiDung_PhongBanModels.getMoTaPhongBan_NguoiDung(username);
            //}

            var phongban = string.IsNullOrWhiteSpace(iID_MaPhongBan) || iID_MaPhongBan == "-1" ?
                ngansachService.GetPhongBan(username) :
                ngansachService.GetPhongBanById(iID_MaPhongBan);

            if (!string.IsNullOrWhiteSpace(phongban.sMoTa))
                sTenPhongBan = phongban.sMoTa.ToUpper();

            var cauhinh = ngansachService.GetCauHinh(username);
            fr.SetValue("Nam", cauhinh.iNamLamViec);
            fr.SetValue("NamSau", cauhinh.iNamLamViec + 1);
            fr.SetValue("NamTruoc", cauhinh.iNamLamViec - 1);
            fr.SetValue("Cap1", LocalizationService.Default.Translate("Donvi_Cap1").ToUpper());
            fr.SetValue("Cap2", LocalizationService.Default.Translate("Donvi_Cap2").ToUpper());

            fr.SetValue("ChuKy_Bql_Ma", phongban.sKyHieu);
            fr.SetValue("ChuKy_CucTruong", LocalizationService.Default.Translate("ChuKy_CucTruong"));
            fr.SetValue("ChuKy_B2_Ten", ngansachService.GetPhongBanById("02").sMoTa.ToUpper());
            fr.SetValue("ChuKy_B2_TruongPhong", LocalizationService.Default.Translate("ChuKy_B2_TruongPhong"));

            fr.SetValue("ChuKy_B3_Ten", ngansachService.GetPhongBanById("03").sMoTa.ToUpper());
            fr.SetValue("ChuKy_B3_TruongPhong", LocalizationService.Default.Translate("ChuKy_B3_TruongPhong"));

            fr.SetValue("ChuKy_Bql_Ten", sTenPhongBan);
            fr.SetValue("ChuKy_Bql_Ma", "B " + phongban.sKyHieu);
            fr.SetValue("ChuKy_Bql_TruongPhong", ngansachService.GetTruongPhongBql(iID_MaPhongBan));

            return fr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="controller"></param>
        /// <param name="iID_MaPhongBan"></param>
        /// <param name="username"></param>
        /// <param name="userDefault"></param>
        /// <param name="hienThi"></param>
        /// <param name="chuky">Hiển thị mặc địch các chữ ký, ví dụ: 1 -> ko hiển thị chữ ký gồm: tên 1, chức danh 1, thừa lệnh 1</param>
        /// <returns></returns>
        public static FlexCelReport UseChuKyForController(this FlexCelReport fr,
            string controller,
            string iID_MaPhongBan = null,
            string username = null,
            string userDefault = "trolyphongban",
            bool hienThi = true,
            string chuky = null)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                username = System.Web.HttpContext.Current.User.Identity.Name;
            }

            var dt = Get_dtLayThongTinChuKy(controller, username);
            var dt2 = Get_dtLayThongTinChuKy(controller, userDefault);

            var fields = new List<string>()
            {
                "ThuaLenh1",
                "ThuaLenh2",
                "ThuaLenh3",
                "ThuaLenh4",
                "ThuaLenh5",

                "ChucDanh1",
                "ChucDanh2",
                "ChucDanh3",
                "ChucDanh4",
                "ChucDanh5",

                "Ten1",
                "Ten2",
                "Ten3",
                "Ten4",
                "Ten5",
            };


            fields.ForEach(x =>
            {
                string value = null;
                if (hienThi)
                {
                    if (dt.Rows.Count > 0)
                    {
                        value = dt.Rows[0][x].ToString();
                        //if (string.IsNullOrWhiteSpace(value) && dt2.Rows.Count > 0)
                        //{
                        //    var v = dt2.Rows[0][x].ToString();
                        //    value = x.Contains("Ten") ? v : v.ToUpper();
                        //}
                    }
                }

                /*
                    QUY UOC 
                    1. Trợ lý phòng ban
                    2. Chỉ huy phòng (trưởng phó phòng)
                    3. B3 
                    4. B2
                    5. Thủ trưởng Cục
                 */

                chuky = HttpContext.Current.Request.GetQueryString("chuky", chuky);
                var chuKys = string.IsNullOrWhiteSpace(chuky) ? new List<string>() : chuky.ToList();
                if (value == null)
                {
                    if (chuKys.Count > 0 && chuKys.Contains(x.LastOrDefault().ToString()))
                    {
                        //    #region fill default value

                        //    // tro ly
                        //    if (x == "Ten1")
                        //    {
                        //        value = NganSachService.Default.User_FullName(username);
                        //    }
                        //    else if (x == "ChucDanh1")
                        //    {
                        //        value = "TRỢ LÝ";
                        //    }
                        //    // CHI HUY PHONG
                        //    else if (x == "Ten2")
                        //    {
                        //        if (!string.IsNullOrWhiteSpace(iID_MaPhongBan))
                        //        {
                        //            var phongban = NganSachService.Default.GetPhongBan(HttpContext.Current.User.Identity.Name);
                        //            iID_MaPhongBan = phongban.sKyHieu;
                        //        }

                        //        value = NganSachService.Default.GetTruongPhongBql(iID_MaPhongBan);
                        //    }
                        //    else if (x == "ChucDanh2")
                        //    {
                        //        value = "TRƯỞNG PHÒNG";

                        //    }
                        //    else if (x == "Ten3")
                        //    {
                        //        value = LocalizationService.Default.Translate("ChuKy_B3_TruongPhong");
                        //    }
                        //    else if (x == "ChucDanh3")
                        //    {
                        //        value = "TRƯỞNG PHÒNG KTNH";
                        //    }
                        //    else if (x == "ChucDanh4")
                        //    {
                        //        value = "TRƯỞNG PHÒNG KHNS";

                        //    }
                        //    else if (x == "Ten4")
                        //    {
                        //        value = LocalizationService.Default.Translate("ChuKy_B2_TruongPhong");
                        //    }
                        //    // bql
                        //    else if (x == "ChucDanh5")
                        //    {
                        //        value = LocalizationService.Default.Translate("ChuKy_CucTruong_ChucVu").ToUpper();
                        //    }
                        //    else if (x == "Ten5")
                        //    {
                        //        value = LocalizationService.Default.Translate("ChuKy_CucTruong");
                        //    }
                        //    else
                        //    {
                        //        var debug = HttpContext.Current.Request.GetQueryStringValue("debug");
                        //        if (debug == 1)
                        //            value = string.Format("[{0}]", x);
                        //    }


                        //    #endregion
                    }
                }
                else if (value == "(tôi)" || value == "@")
                {
                    value = NganSachService.Default.User_FullName(username);
                }

                fr.SetValue(x, value);
            });
            return fr;
        }


        private static DataTable Get_dtLayThongTinChuKy(String ControllerName, String MaND)
        {
            String SQL =
                @"
SELECT
        ThuaLenh1=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaThuaLenh1)
        ,ThuaLenh2=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaThuaLenh2)
        ,ThuaLenh3=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaThuaLenh3)
        ,ThuaLenh4=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaThuaLenh4)
        ,ThuaLenh5=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaThuaLenh5)
        ,ChucDanh1=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaChucDanh1)
        ,ChucDanh2=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaChucDanh2)
        ,ChucDanh3=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaChucDanh3)
        ,ChucDanh4=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaChucDanh4)
        ,ChucDanh5=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaChucDanh5)
        ,Ten1=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaTen1)
        ,Ten2=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaTen2)
        ,Ten3=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaTen3)
        ,Ten4=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaTen4)
        ,Ten5=(SELECT sTen FROM NS_DanhMucChuKy WHERE iID_MaChuKy=NS_DanhMuc_BaoCao_ChuKy.iID_MaTen5)
FROM    NS_DanhMuc_BaoCao_ChuKy 
WHERE   iTrangThai=1
        AND sController=@sController 
        AND sID_MaNguoiDungTao=@sID_MaNguoiDungTao";

            var cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sController", ControllerName);
            cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", MaND);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static FlexCelReport FillDataTableLNS4(this FlexCelReport fr, DataTable dt)
        {
            dt.TableName = "ChiTiet";
            DataTable dtsTM = SelectDistinct(dt, "dtsTM", "sLNS,sL,sK,sM,sTM");
            DataTable dtsM = SelectDistinct(dtsTM, "dtsM", "sLNS,sL,sK,sM");
            DataTable dtsL = SelectDistinct(dtsM, "dtsL", "sLNS,sL,sK");
            DataTable dtsLNS = SelectDistinct(dtsL, "dtsLNS", "sLNS");

            fr.AddTable("ChiTiet", dt);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);

            dt.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();

            return fr;
        }

        public static FlexCelReport FillDataTable_4(this FlexCelReport fr, DataTable data)
        {
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK", "sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "sLNS,sMoTa", "sLNS,sL");

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();

            return fr;
        }

        public static FlexCelReport FillDataTableLNS6(this FlexCelReport fr, DataTable dt)
        {
            dt.TableName = "ChiTiet";
            DataTable dtsTM = SelectDistinct(dt, "dtsTM", "sLNS3,sLNS5,sLNS,sL,sK,sM,sTM");
            DataTable dtsM = SelectDistinct(dtsTM, "dtsM", "sLNS3,sLNS5,sLNS,sL,sK,sM");
            DataTable dtsL = SelectDistinct(dtsM, "dtsL", "sLNS3,sLNS5,sLNS,sL,sK");
            DataTable dtsLNS = SelectDistinct(dtsL, "dtsLNS", "sLNS3,sLNS5,sLNS");
            DataTable dtLNS5 = SelectDistinct(dtsLNS, "dtLNS5", "sLNS3,sLNS5");
            DataTable dtLNS3 = SelectDistinct(dtLNS5, "dtLNS3", "sLNS3");

            fr.AddTable("ChiTiet", dt);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS3", dtLNS3);
            fr.AddTable("dtsLNS5", dtLNS5);

            dt.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtLNS3.Dispose();
            dtLNS5.Dispose();

            return fr;
        }

        public static FlexCelReport FillDataTableLNS7(this FlexCelReport fr, DataTable dt)
        {
            dt.TableName = "ChiTiet";
            DataTable dtsTM = SelectDistinct(dt, "dtsTM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM");
            DataTable dtsM = SelectDistinct(dtsTM, "dtsM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM");
            DataTable dtsL = SelectDistinct(dtsM, "dtsL", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK");
            DataTable dtsLNS = SelectDistinct(dtsL, "dtsLNS", "sLNS1,sLNS3,sLNS5,sLNS");
            DataTable dtLNS5 = SelectDistinct(dtsLNS, "dtLNS5", "sLNS1,sLNS3,sLNS5");
            DataTable dtLNS3 = SelectDistinct(dtLNS5, "dtLNS3", "sLNS1,sLNS3");
            DataTable dtLNS1 = SelectDistinct(dtLNS3, "dtLNS1", "sLNS1");

            fr.AddTable("ChiTiet", dt);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS5", dtLNS5);
            fr.AddTable("dtsLNS3", dtLNS3);
            fr.AddTable("dtsLNS1", dtLNS1);

            dt.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtLNS5.Dispose();
            dtLNS3.Dispose();
            dtLNS1.Dispose();

            return fr;
        }

        private static DataTable SelectDistinct(DataTable dtSource, string tableName, string fieldName)
        {
            var dt = dtSource.SelectDistinct(tableName, fieldName)
                .AddLnsMoTa();
            return dt;
        }

        public static DataTable AddLnsMoTa(this DataTable dt, string iNamLamViec = null)
        {
            var count = dt.Columns.Count;
            dt.Columns.Add("sMoTa");
            var lns = "sLNS,sL,sK,sM,sTM,sTTM".Split(',').ToList();

            var columnNames = dt.GetColumnNames();

            dt.AsEnumerable()
                .ToList()
                .ForEach(x =>
                {
                    var items = new List<string>();
                    lns.ForEach(l =>
                    {
                        if (columnNames.Contains(l))
                        {
                            var c = x.Field<string>(l);
                            items.Add(c);
                        }
                    });


                    var key = items.Count == 0 ?
                    x.ItemArray[count - 1] :
                    items.Join("-");

                    if (string.IsNullOrWhiteSpace(iNamLamViec))
                        iNamLamViec = DateTime.Now.Year.ToString();

                    x["sMoTa"] = getLnsMoTa(key.ToString(), iNamLamViec);
                });
            return dt;
        }

        private static string getLnsMoTa(string key, string iNamLamViec)
        {
            var cacheKey = string.Format("{0}_{1}", key, iNamLamViec);
            if (LnsCache.ContainsKey(cacheKey))
            {
                var item = LnsCache.FirstOrDefault(x => x.Key == cacheKey);
                return item.Value;
            }

            //            var sql =
            //@"

            //SELECT  TOP(1) sMoTa 
            //FROM    NS_MucLucNganSach
            //WHERE   iTrangThai =1
            //        AND iNamLamViec=@iNamLamViec 
            //        AND sXauNoiMa LIKE (@sXauNoiMa +'%')
            //ORDER BY sXauNoiMa

            //";

            var sql =
@"

SELECT  TOP(1) sMoTa 
FROM    NS_MucLucNganSach
WHERE   iTrangThai =1
        AND iNamLamViec=@iNamLamViec 
        AND (sXauNoiMa= @sXauNoiMa 
            or (sXauNoiMa like @sXauNoiMa + '-' + sTTM +'-%')) --lay tieu muc
ORDER BY sXauNoiMa

";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@sXauNoiMa", key);
                var mota = Connection.GetValueString(cmd, "");

                if (!LnsCache.ContainsKey(cacheKey))
                    LnsCache.Add(cacheKey, mota);
                return mota;
            }

        }

        #region cache lns

        private static Dictionary<string, string> _lnsCache;

        private static Dictionary<string, string> LnsCache
        {
            get
            {
                return _lnsCache ?? (_lnsCache = getLnsCache());
            }
        }

        private static Dictionary<string, string> getLnsCache()
        {
            var cacheKey = "LnsCache";
            var lnsCache = System.Web.HttpContext.Current.Cache.Get(cacheKey);
            if (lnsCache == null)
            {
                lnsCache = new Dictionary<string, string>();
                HttpContext.Current.Cache.Add(cacheKey,
                    lnsCache,
                    null,
                    DateTime.Now.AddMinutes(30),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Normal, onRemoveCallback);

            }
            return (Dictionary<string, string>)lnsCache;
        }

        private static void onRemoveCallback(string key, object value, CacheItemRemovedReason reason)
        {
            _lnsCache = null;
        }

        #endregion

        #region flexexcel form

        public static FlexCelReport UseForm(this FlexCelReport fr, Controller controller)
        {
            var Request = controller.Request;

            var r = Request.GetQueryStringValue("r", 140);
            r = r == 0 ? 140 : r;
            fr.SetExpression("fRow", "<#Row height(Autofit;" + r + ")>");

            fr.SetExpression("fRow0", "<#Row height(0)>");
            fr.SetExpression("fRowFit", "<#Row height(Autofit)>");
            fr.SetExpression("fRowF", "<#Row height(Autofit)>");
            fr.SetExpression("autoP", "<#auto page breaks>");

            return fr;
        }

        public static void MergeH(this XlsFile xls, int row, int col, int length)
        {
            var x = col;
            var y = row;

            var x_to = x;
            var x_from = x;

            var dic = new Dictionary<int, int>();

            var x_to_temp = x_from;

            for (int i = 0; i < length; i++)
            {
                var cell1 = xls.GetCellValue(y, x + i);
                var cell2 = xls.GetCellValue(y, x + i + 1);

                if (cell1 == cell2 && cell2 != null && cell1 != null)
                {
                    x_to = x_to + 1;
                    //x_to_temp += 1;
                }
                else
                {
                    if (x_to > x_from)
                    {
                        // merge cells
                        //xls.MergeCells(y, x_from, y, x_to);

                        dic.Add(x_from, x_to);
                    }

                    x_from = x + i + 1;
                    x_to = x_from;
                }

            }


            dic.ToList()
                .ForEach(p =>
                {
                    xls.MergeCells(y, p.Key, y, p.Value);

                    var value = xls.GetCellValue(y, p.Key);
                    //if (value != null && value.ToString() == "(+)")
                    //{
                    //    TFlxApplyFormat ApplyFormat = new TFlxApplyFormat();
                    //    ApplyFormat.SetAllMembers(true);
                    //    //ApplyFormat.Borders.SetAllMembers(true);  //We will only apply the borders to the existing cell formats
                    //    TFlxFormat fmt = xls.GetDefaultFormat;
                    //    //fmt.Font.Style = TFlxFontStyles.Bold;
                    //    //fmt.Font.Size20 = 172;

                    //    fmt.HAlignment = THFlxAlignment.center;
                    //    fmt.VAlignment = TVFlxAlignment.center;

                    //    fmt.Borders.SetAllBorders(TFlxBorderStyle.Thin, TExcelColor.Automatic);
                    //    fmt.Borders.Diagonal.Color = TExcelColor.FromArgb(100);

                    //    xls.SetCellFormat(y, p.Key, y, p.Value, fmt, ApplyFormat, true);
                    //}
                });

            //if (true)
            //    //{
            //    //    xls.SetCellValue(y + 1, x + i, $"Cộng");
            //    //    xls.MergeCells(y + 1, x + i, y + 2, x + i);


            //    //    //Add a rectangle around the cells
            //    //    TFlxApplyFormat ApplyFormat = new TFlxApplyFormat();
            //    //    ApplyFormat.SetAllMembers(true);
            //    //    //ApplyFormat.Borders.SetAllMembers(true);  //We will only apply the borders to the existing cell formats
            //    //    TFlxFormat fmt = xls.GetDefaultFormat;
            //    //    fmt.Font.Style = TFlxFontStyles.Bold;
            //    //    fmt.Font.Size20 = 172;

            //    //    fmt.HAlignment = THFlxAlignment.center;
            //    //    fmt.VAlignment = TVFlxAlignment.center;

            //    //    fmt.Borders.SetAllBorders(TFlxBorderStyle.Thin, TExcelColor.Automatic);
            //    //    fmt.Borders.Diagonal.Color = TExcelColor.FromArgb(100);

            //    //    xls.SetCellFormat(y + 1, x + i, y + 1, x + i, fmt, ApplyFormat, true);  //Set last parameter to true so it draws a box.


            //    //    //// merge cells
            //    //    //xls.SetCellValue(y, x + sumFrom, _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, c.sLNS));
            //    //    //xls.MergeCells(y, x + sumFrom, y, x + i);
            //}
        }

        public static void MergeC(this XlsFile xls, int rowStart, int rowEnd, int colStart, int length)
        {
            List<MergeInfo> listMerge = new List<MergeInfo>();
            var x = colStart;
            var y = rowStart;

            var y_to = rowStart;
            var y_from = rowStart;


            var x_to_temp = y_from;

            for (int i = 0; i < length; i++)
            {
                for (int j = rowStart; j <= rowEnd; j++)
                {
                    if(j + 1 <= rowEnd) {
                        var cell1 = xls.GetCellValue(j, x + i);
                        var cell2 = xls.GetCellValue(j + 1, x + i);

                        if (cell1 == cell2 && cell2 != null && cell1 != null)
                        {
                            y_to = y_to + 1;
                        }
                        else
                        {
                            if (y_to > y_from)
                            {
                                listMerge.Add(new MergeInfo
                                {
                                    xFrom = x + i,
                                    yFrom = y_from,
                                    xTo = x + i,
                                    yTo = y_to
                                });
                                //dic.Add(y_from, y_to);
                            }

                            y_from = j + 1;
                            y_to = y_from;
                        }
                    } else
                    {
                        if (y_to > y_from)
                        {
                            listMerge.Add(new MergeInfo
                            {
                                xFrom = x + i,
                                yFrom = y_from,
                                xTo = x + i,
                                yTo = y_to
                            });
                            //dic.Add(y_from, y_to);
                        }

                        y_from = rowStart;
                        y_to = y_from;
                    }
                    
                }
            }

            listMerge
                 .ForEach(p =>
                {
                    xls.MergeCells(p.yFrom, p.xFrom, p.yTo, p.xTo);
                    //var value = xls.GetCellValue(y, p.Key);
                });
        }

        public static void Run(this FlexCelReport fr, XlsFile xls, int to)
        {
            if (to != 1)
            {
                var header = xls.GetPageHeaderAndFooter();
                header.DiffFirstPage = false;
                xls.SetPageHeaderAndFooter(header);
            }
            fr.SetValue("To", to);
            fr.Run(xls);
        }
        #endregion

        public class MergeInfo
        {
            public int xFrom { get; set; }
            public int yFrom { get; set; }
            public int xTo { get; set; }
            public int yTo { get; set; }
        }
    }
}
