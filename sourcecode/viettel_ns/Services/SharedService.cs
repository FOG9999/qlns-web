using DomainModel;
using FlexCel.Core;
using FlexCel.Render;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;
using Viettel.Data;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace Viettel.Services
{
    public interface ISharedService
    {
        #region Commons
        string LayNamLamViec(string MaND);
        DataRow LayPhongBan(string MaND);
        string LayNhomNguoiDung(string MaND);
        bool CheckLock(int Nam, string Id_PhanHe, string Table_Name = null, string Id_PhongBan = null, string Id_DonVi = null);
        void Language();
        #endregion

        #region Forms
        bool checkEditByPhongBan(string name, string value);
        #endregion

        #region Reports
        clsExcelResult ExportToExcel(ExcelFile exfile, string filename);
        ActionResult ViewPDF(ExcelFile exfile);
        DataTable SelectDistinct(string tableName, DataTable sourceTable, string columnsName, string columnsAdd, string tableGet = "", string columnsGet = "", string nam = "", string columns_Condition_Content = "", string columns_Condition_Content_Value = "", string sort_Condition = "");
        DataTable CountDistinctRow(DataTable sourceTable, string rangs);
        DataTable NullToZero(DataTable sourceTable, string[] colNames);
        #endregion
    }

    public class SharedService : ISharedService
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private static ISharedService _default;

        public static ISharedService Default
        {
            get { return _default ?? (_default = new SharedService()); }
        }
        #region Commons
        public string LayNamLamViec(string MaND)
        {
            DataTable dt = NguoiDungCauHinhModels.LayCauHinh(MaND);
            string iNamLamViec = DateTime.Now.Year.ToString();
            if (dt.Rows.Count > 0)
            {
                iNamLamViec = Convert.ToString(dt.Rows[0]["iNamLamViec"]);
            }
            return iNamLamViec;
        }
        public void Language()
        {
            string lang = "vi-VN";
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
        }
        public DataRow LayPhongBan(string MaND)
        {
            string SQL = "select sKyHieu, sTen + ' - ' + sMoTa as sTen from NS_PhongBan where iTrangThai = 1 and iID_MaPhongBan in (select iID_MaPhongBan from NS_NguoiDung_PhongBan where sMaNguoiDung = @sMaNguoiDung and iTrangThai = 1) order by sKyHieu";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sMaNguoiDung", MaND);

            var Content = Connection.GetDataTable(cmd);
            cmd.Dispose();

            if (Content.Rows.Count > 0)
            {
                return Content.Rows[0];
            } else
            {
                return Content.NewRow();
            }

        }
        public bool CheckLock(int Nam, string Id_PhanHe, string Table_Name = null, string Id_PhongBan = null, string Id_DonVi = null)
        {
            string SQL = "select * from DC_LockData where NamLamViec = @nam  and (@table is null or Name_Table = @table) and (@phongban is null or @phongban in (select * from f_split(Id_PhongBan))) and (@donvi is null or @donvi in (select * from f_split(Id_DonVi))) ";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@nam", Nam);
            cmd.Parameters.AddWithValue("@phanhe", Id_PhanHe);
            cmd.Parameters.AddWithValue("@table", Table_Name);
            cmd.Parameters.AddWithValue("@phongban", Id_PhongBan.ToParamString());
            cmd.Parameters.AddWithValue("@donvi", Id_DonVi.ToParamString());

            var Content = Connection.GetDataTable(cmd);
            cmd.Dispose();

            if (Content.Rows.Count > 0)
            {
                var result = !Convert.ToBoolean(Content.Rows[0]["Lock"]);
                return result;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Forms
        public bool checkEditByPhongBan(string name, string value)
        {
            var pb = LayPhongBan(name)["sKyHieu"].ToString();

            if (!string.IsNullOrEmpty(pb))
            {
                return pb == value || string.IsNullOrEmpty(LayNhomNguoiDung(name));
            } else
            {
                return false;
            }
        }
        public string LayNhomNguoiDung(string MaND)
        {
            string SQL = "SELECT iDoiTuongNguoiDung FROM QT_NguoiDung WHERE sID_MaNguoiDung = @sID_MaNguoiDung";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sID_MaNguoiDung", MaND);

            var Content = Connection.GetValueString(cmd, "");
            cmd.Dispose();

            return Content;
        }
        #endregion

        #region Report
        public clsExcelResult ExportToExcel(ExcelFile exfile, string filename)
        {
            clsExcelResult clsResult = new clsExcelResult();

            using (MemoryStream ms = new MemoryStream())
            {
                exfile.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = filename;
                clsResult.type = "xls";
                return clsResult;
            }
        }
        public ActionResult ViewPDF(ExcelFile exfile)
        {
            HamChung.Language();
            using (FlexCelPdfExport pdf = new FlexCelPdfExport())
            {
                pdf.Workbook = exfile;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.BeginExport(ms);
                    pdf.ExportAllVisibleSheets(false, "BaoCao");
                    pdf.EndExport();
                    ms.Position = 0;
                    var result = new FileContentResult(ms.ToArray(), contentType: "application/pdf");

                    return result;
                }

            }
        }
        /// <summary>
        /// SELECT DISTINCT Tu DATATABLE
        /// </summary>
        /// <param name="tableName">Tên bảng mới</param>
        /// <param name="sourceTable">Table bảng dữ liệu nguồn</param>
        /// <param name="columnsName">Tên cột cần distinct</param>
        /// <param name="columnsAdd">Danh sách cột của Table mới</param>
        /// <param name="tableGet">Tên bảng lấy content</param>
        /// <param name="columnGet">Tên cột lấy content của dòng</param>
        /// <param name="nam">Tên trường điều kiện năm làm việc</param>
        /// <param name="columns_Condition_Content">Tên trường điều kiện để lấy mô tả</param>
        /// <param name="columns_Condition_Content_Value">Tên trường để lấy giá trị khi lấy thông tin mô tả</param>
        /// <param name="sort_Condition">Tên trường điều kiện sắp xếp</param>
        /// <returns>DataTable</returns>
        public DataTable SelectDistinct(string tableName, DataTable sourceTable, string columnsName, string columnsAdd, string tableGet = "", string columnsGet = "", string nam = "", string columns_Condition_Content = "", string columns_Condition_Content_Value = "", string sort_Condition = "")
        {
            DataTable dt = new DataTable(tableName);
            string[] arrColumnsAdd = columnsAdd.Split(',');
            string[] arrColumnsName = columnsName.Split(',');
            for (int i = 0; i < arrColumnsAdd.Length; i++)
            {
                dt.Columns.Add(arrColumnsAdd[i], sourceTable.Columns[arrColumnsAdd[i]].DataType);
            }
            if (sourceTable.Rows.Count > 0)
            {
                object[] LastValue = new object[arrColumnsName.Length];
                for (int i = 0; i < LastValue.Length; i++)
                {
                    LastValue[i] = null;
                }

                foreach (DataRow dr in sourceTable.Select("", columnsName + " " + sort_Condition))
                {
                    bool ok = true;
                    for (int i = 0; i < arrColumnsName.Length; i++)
                    {
                        if (LastValue[i] != null && (ColumnEqual(LastValue[i], dr[arrColumnsName[i]])))
                        {
                            ok = false;
                        }
                        else
                        {
                            ok = true;
                            break;
                        }
                    }
                    for (int i = 0; i < arrColumnsName.Length; i++)
                    {
                        if (ok)
                        {
                            LastValue[i] = dr[arrColumnsName[i]];
                        }
                    }
                    if (ok)
                    {
                        DataRow R = dt.NewRow();
                        for (int j = 0; j < arrColumnsAdd.Length; j++)
                        {
                            R[arrColumnsAdd[j]] = dr[arrColumnsAdd[j]];
                        }
                        var columnGet = columnsGet.Split(',');
                        for (int i = 0; i < columnGet.Length; i++)
                        {
                            if ((string.IsNullOrEmpty(columns_Condition_Content) == false && Array.IndexOf(arrColumnsAdd, columnGet[i]) >= 0) || string.IsNullOrEmpty(columns_Condition_Content_Value) == false)
                                R[arrColumnsAdd[Array.IndexOf(arrColumnsAdd, columnGet[i])]] = GetContent(dr, tableGet, columnGet[i], nam, columns_Condition_Content, columns_Condition_Content_Value);
                        }                        
                        dt.Rows.Add(R);
                    }
                }
            }
            return dt;
        }
        public DataTable CountDistinctRow(DataTable sourceTable, string rangs)
        {
            string[] rang = rangs.Split(',');
            var dt = sourceTable.Copy();
            dt.Columns.Add(new DataColumn($"Count", typeof(string)));

            foreach (DataRow dtr in dt.Rows)
            {
                string wherecondition = "";
                for (int i = 0; i < rang.Length; i++)
                {
                    if (i == 0)
                    {
                        wherecondition += $"{rang[i]} = {dtr[rang[i]].ToString()}";
                    }
                    else
                    {
                        wherecondition += $" and {rang[i]} = {dtr[rang[i]].ToString()}";
                    }
                }
                var count = dt.Select(wherecondition).Length;
                dtr["Count"] = count;
            }
            return dt;
        }

        public DataTable NullToZero(DataTable sourceTable, string[] colNames)
        {
            var dt = sourceTable.Copy();
            foreach (DataRow dtr in dt.Rows)
            {
                for (int i = 0; i < colNames.Length; i++)
                {
                    if (dtr[colNames[i].ToString()] == null || String.IsNullOrEmpty(dtr[colNames[i].ToString()].ToString()))
                    {
                        dtr[colNames[i].ToString()] = 0;
                    }
                }
            }
            return dt;
        }
        #endregion

        #region private func
        /// <summary>
        /// Compares two values to see if they are equal. Also compares DBNULL.Value.
        /// Note: If your DataTable contains object fields, then you must extend this
        /// function to handle them in a meaningful way if you intend to group on them.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private bool ColumnEqual(object A, object B)
        {
            if ((A == DBNull.Value || A == null) && (B == DBNull.Value || B == null)) //  both are DBNull.Value
                return true;
            if ((A == DBNull.Value || A == null) || (B == DBNull.Value || B == null)) //  only one is DBNull.Value
                return false;
            return (A.Equals(B));  // value type standard comparison
        }
        private string GetContent(DataRow dr, string tableGet, string columnGet, string nam, string columns_Condition_Content, string column_Condition_Content_Value = "")
        {
            string Content = "";
            int Nam = Convert.ToInt32(LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name));
            
            string[] arrDK = columns_Condition_Content.Split(',');
            string DK = "";
            for (int i = 0; i < arrDK.Length; i++)
            {
                DK += arrDK[i] + "=@" + arrDK[i];
                if (i < arrDK.Length - 1)
                    DK += " AND ";
            }
            if (string.IsNullOrEmpty(DK) == false) DK = " WHERE " + DK;

            DK += " AND " + nam + " = @nam";

            string SQL = "SELECT " + columnGet + " FROM " + tableGet + DK;
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@nam", Nam);
            for (int i = 0; i < arrDK.Length; i++)
            {
                if (i < arrDK.Length - 1)
                    cmd.Parameters.AddWithValue("@" + arrDK[i], dr[arrDK[i]]);
                else
                    cmd.Parameters.AddWithValue("@" + arrDK[i], "");
            }
            if (string.IsNullOrEmpty(column_Condition_Content_Value) == false)
            {
                cmd.Parameters.RemoveAt(cmd.Parameters.IndexOf("@" + arrDK[0]));
                cmd.Parameters.AddWithValue("@" + arrDK[0], dr[column_Condition_Content_Value]);
            }
            Content = Connection.GetValueString(cmd, "");
            cmd.Dispose();  
                      
            return Content;
        }        
        #endregion
    }
}
