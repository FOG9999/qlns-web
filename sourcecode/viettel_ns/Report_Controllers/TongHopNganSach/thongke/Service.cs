using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace QLTCController
{
    public class Service
    {
        #region public Methods
        /// <summary>
        /// Add STT to table
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="level"></param>
        /// <param name="rang"></param>
        /// <returns></returns>
        public static DataTable AddOrdinalsNum(DataTable dt, int level, string rang = null, string space = "")
        {
            DataTable vR = dt;
            vR.Columns.Add(new DataColumn("STT", typeof(string)));
            int start = 1;

            for (int i = 0; i < vR.Rows.Count; i++)
            {
                if (level == 1)
                {
                    vR.Rows[i]["STT"] = (char)(i + 65);
                }
                else
                {
                    if (rang != null)
                    {
                        var count = vR.AsEnumerable().ToList()
                            .Where(x => !string.IsNullOrEmpty(x.Field<string>(rang)) && x.Field<string>(rang) == vR.Rows[i][rang].ToString())
                            .Select(x => x.Field<string>(rang)).Count();
                        if (level == 2)
                        {
                            vR.Rows[i]["STT"] = space + NToR(start);
                        }
                        else if (level == 3)
                        {
                            vR.Rows[i]["STT"] = space + start;
                        }
                        if (start < count)
                        {
                            start++;
                        }
                        else
                        {
                            start = 1;
                        }
                    }
                    else if (level > 1 && level < 4)
                    {
                        if (level == 2)
                        {
                            vR.Rows[i]["STT"] = space + NToR(i + 1);
                        }
                        else if (level == 3)
                        {
                            vR.Rows[i]["STT"] = space + (i + 1);
                        }
                    }
                    else
                    {
                        if (level == 4)
                        {
                            vR.Rows[i]["STT"] = space + "-";
                        }
                        else
                        {
                            vR.Rows[i]["STT"] = space + "+";
                        }
                    }
                }
            }
            return vR;
        }

        /// <summary>
        /// Convert number int to Roman
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NToR(int number)
        {
            string roMan = "";
            if ((number < 0) || (number > 3999))
            {
                throw new ArgumentOutOfRangeException("Value must be between 1 and 3999");
            }
            else if (number < 1)
            {
                roMan = "";
            }
            else if (number >= 1000)
            {
                roMan = "M" + NToR(number - 1000);
            }
            else if (number >= 900)
            {
                roMan = "CM" + NToR(number - 900);
            }
            else if (number >= 500)
            {
                roMan = "D" + NToR(number - 500);
            }
            else if (number >= 400)
            {
                roMan = "CD" + NToR(number - 400);
            }
            else if (number >= 100)
            {
                roMan = "C" + NToR(number - 100);
            }
            else if (number >= 90)
            {
                roMan = "XC" + NToR(number - 90);
            }
            else if (number >= 50)
            {
                roMan = "L" + NToR(number - 50);
            }
            else if (number >= 40)
            {
                roMan = "XL" + NToR(number - 40);
            }
            else if (number >= 10)
            {
                roMan = "X" + NToR(number - 10);
            }
            else if (number >= 9)
            {
                roMan = "IX" + NToR(number - 9);
            }
            else if (number >= 5)
            {
                roMan = "V" + NToR(number - 5);
            }
            else if (number >= 4)
            {
                roMan = "IV" + NToR(number - 4);
            }
            else if (number >= 1)
            {
                roMan = "I" + NToR(number - 1);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Value must be between 1 and 3999.");
            }
            return roMan;
        }

        /// <summary>
        /// Convert null to zero of DataTable
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static void NullToZero(DataTable dt, string[] colNames)
        {
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
        }

        /// <summary>
        /// Get Table from accesfile with connectionString and scriptcommandtext.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetTable(string connectionString, string sql, double dvt = 1)
        {
            var connectionStringName = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            OleDbConnection connection = new OleDbConnection(connectionStringName);

            using (OleDbCommand scriptCommand = connection.CreateCommand())
            {
                connection.Open();
                scriptCommand.CommandType = CommandType.Text;
                scriptCommand.Parameters.Add("dvt", OleDbType.Double).Value = dvt;
                scriptCommand.CommandText = sql;

                OleDbDataReader reader = scriptCommand.ExecuteReader();
                var result = new DataTable();
                result.Load(reader);

                return result;
            }
        }

        /// <summary>
        /// Get Table from accesfile with connectionString and scriptcommandtext.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetTableStoreP(string connectionString, string sqlStorename, string Namlv = "",
            double dvt = 1)
        {
            var connectionStringName = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            OleDbConnection connection = new OleDbConnection(connectionStringName);

            using (OleDbCommand scriptCommand = connection.CreateCommand())
            {
                connection.Open();
                scriptCommand.CommandType = CommandType.StoredProcedure;
               
                scriptCommand.Parameters.Add("dvt", OleDbType.Double).Value = dvt;
                
                if (Namlv != "")
                {
                    scriptCommand.Parameters.Add("iNamlv", OleDbType.WChar, 4).Value = Namlv;
                }
                scriptCommand.CommandText = sqlStorename;

                OleDbDataReader reader = scriptCommand.ExecuteReader();
                var result = new DataTable();
                result.Load(reader);

                return result;
            }
        }

        public static string GetTenDonVi(string connectionString, int namLamViec, string id)
        {
            var connectionStringName = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;

            var sql = connectionString == "NSach11" 
                        ? @"
                            select  iID_MaDonVi, sTen = iID_MaDonVi + ' - ' + sTen
                            from    NS_DonVi
                            where   iTrangThai=1
                                    and iID_MaDonVi=@iID_MaDonVi
                                    and iNamLamViec_DonVi=@iNamLamViec
                            "
                        : @"
                            select  dvi, sTen = dvi + ' - ' + ten
                            from    [Dmuc2009].[dbo].[Donvi]
                            where   dvi=@iID_MaDonVi
                            ";

            using (var conn = new SqlConnection(connectionStringName))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {                    
                    cmd.Parameters.AddWithValue("@iID_MaDonVi",id);
                    cmd.Parameters.AddWithValue("@iNamLamViec",namLamViec);

                    return GetTable(cmd).AsEnumerable().Select(x=>x.Field<string>("sTen")).First();
                }
            }
        }

        public static DataTable LapNhuCau_GetTableStoreP(string connectionString, string sqlStorename, Dictionary<string,Param> lstpara )
        {
            var connectionStringName = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            OleDbConnection connection = new OleDbConnection(connectionStringName);

            using (OleDbCommand scriptCommand = connection.CreateCommand())
            {
                connection.Open();
                scriptCommand.CommandType = CommandType.StoredProcedure;
                foreach (string key in lstpara.Keys)
                {
                    scriptCommand.Parameters.Add(key, lstpara[key].type).Value = Convert.ChangeType(lstpara[key].value, key.GetType());

                }
                   // scriptCommand.Parameters.Add("dvt", OleDbType.Double).Value = dvt;
               
                scriptCommand.CommandText = sqlStorename;

                OleDbDataReader reader = scriptCommand.ExecuteReader();
                var result = new DataTable();
                result.Load(reader);

                return result;
            }
        }

        public static DataTable GetTableStorePWithDvi(string connectionString, string sqlStorename, string Namlv = "",double dvt = 0, string DviPhcap = "")
        {
            var connectionStringName = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            OleDbConnection connection = new OleDbConnection(connectionStringName);

            using (OleDbCommand scriptCommand = connection.CreateCommand())
            {
                connection.Open();
                scriptCommand.CommandType = CommandType.StoredProcedure;
                if (dvt != 0)
                {
                    scriptCommand.Parameters.Add("dvt", OleDbType.Double).Value = dvt;
                }
                if (Namlv != "")
                {
                    scriptCommand.Parameters.Add("iNamlv", OleDbType.WChar, 4).Value = Namlv;
                }
                if (DviPhcap != "")
                {
                    scriptCommand.Parameters.Add("iDviPhcap", OleDbType.WChar, 20).Value = DviPhcap;
                }
                scriptCommand.CommandText = sqlStorename;

                OleDbDataReader reader = scriptCommand.ExecuteReader();
                var result = new DataTable();
                result.Load(reader);

                return result;
            }
        }

        /// <summary>
        /// Get Distinct DataSet 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtName"></param>
        /// <param name="distincCol"></param>
        /// <param name="colCheck"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(DataTable dt, DataTable dtName, string distincCol, string colCheck, string colName)
        {
            DataSet results = new DataSet();

            DataView view = new DataView(dt);
            string[] colArr = distincCol.Split(',');
            for (int i = 0; i < colArr.Length; i++)
            {
                string[] distCol = colArr.Take(colArr.Length - i).ToArray();
                DataTable temp = view.ToTable(true, distCol);
                temp.TableName = "dt" + i;
                temp.Columns.Add(new DataColumn(colName, typeof(string)));
                if(dt.GetColumnNames().Contains("STT"))
                    temp.Columns.Add(new DataColumn("STT", typeof(string)));
                // ng ten noi dung ngan sach cho bang vua tao ra
                for (int j = 0; j < temp.Rows.Count; j++)
                {
                    foreach (DataRow dtr in dtName.Rows)
                    {
                        if (temp.Rows[j][distCol.Last().ToString()].ToString().Trim() == dtr[colCheck].ToString().Trim())
                        {
                            temp.Rows[j][colName] = dtr[colName];
                            if (temp.GetColumnNames().Contains("STT") && dt.GetColumnNames().Contains("STT"))
                            {
                                temp.Rows[j]["STT"] = dtr["STT"];
                            }
                            
                        }
                    }
                }
                results.Tables.Add(temp);
            }
            return results;
        }

        /// <summary>
        /// Get Distinct DataSet Mlns
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtName"></param>
        /// <param name="distincCol"></param>
        /// <param name="colCheck"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static DataSet GetDataSetMlns(DataTable dt, DataTable dtName, string distincCol, string colCheck, string colName)
        {
            DataSet results = new DataSet();

            DataView view = new DataView(dt);
            string[] colArr = distincCol.Split(',');
            for (int i = 0; i < colArr.Length; i++)
            {
                List<string> distCol = colArr.Take(colArr.Length - i).ToList();
                string tabName = "dt" + distCol.Last().Split(new char[] { ' ' })[0];
                if (distCol.Contains("L K"))
                {
                    distCol.Insert(distCol.IndexOf("L K"), "L");
                    distCol.Insert(distCol.IndexOf("L K"), "K");
                    distCol.RemoveAt(distCol.IndexOf("L K"));
                }

                if (distCol.Contains("TTm Ng"))
                {
                    distCol.Insert(distCol.IndexOf("TTm Ng"), "TTm");
                    distCol.Insert(distCol.IndexOf("TTm Ng"), "Ng");
                    distCol.RemoveAt(distCol.IndexOf("TTm Ng"));
                }

                DataTable temp = view.ToTable(true, distCol.ToArray());
                temp.TableName = tabName;
                temp.Columns.Add(new DataColumn(colName, typeof(string)));
               
                // Dien ten noi dung ngan sach cho bang vua tao ra
                for (int j = 0; j < temp.Rows.Count; j++)
                {
                    string sXaunoima = "";
                    if (temp.Columns.Count >= 2 && temp.Columns.Count <= 5)
                    {
                        sXaunoima = temp.Rows[j][temp.Columns.Count - 2].ToString().Trim();
                    }
                    else
                    {
                        for (int k = 3; k < temp.Columns.Count - 1; k++)
                        {
                            if (k == 3)
                            {
                                sXaunoima += temp.Rows[j][k].ToString().Trim();
                            }
                            else
                            {
                                sXaunoima += "-" + temp.Rows[j][k].ToString().Trim();
                            }

                        }
                    }
                    foreach (DataRow dtr in dtName.Rows)
                    {
                        if (sXaunoima == dtr[colCheck].ToString().Trim())
                        {
                            temp.Rows[j][colName] = dtr[colName];
                        }
                    }
                }
                results.Tables.Add(temp);
            }
            return results;
        }
        public static DataTable AddMoTa(DataTable dt, string tablename, DataTable dtName, string distincCol, string colName, string colCheck)
        {
            var vR = dt;
            vR.TableName = tablename;
            vR.Columns.Add(new DataColumn(colName, typeof(string)));

            var colarr = distincCol.Split(',');
                
            for (int j = 0; j < vR.Rows.Count; j++)
            {
                string sXaunoima = "";
                for (int i = 0; i < colarr.Length; i++)
                {
                    if (sXaunoima == "")
                    {
                        sXaunoima += vR.Rows[j][colarr[i]].ToString().Trim();
                    }
                    else
                    {
                        sXaunoima += "-"+vR.Rows[j][colarr[i]].ToString().Trim();
                    }
                }                
                
                foreach (DataRow dtr in dtName.Rows)
                {
                    if (sXaunoima == dtr[colCheck].ToString().Trim())
                    {
                        vR.Rows[j][colName] = dtr[colName];
                    }
                }
            }
            
            return vR;
        }

        public static DataSet GetDataSetMlnsBĐ(DataTable dt, DataTable dtName, string distincCol, string colCheck, string colName)
        {
            DataSet results = new DataSet();

            DataView view = new DataView(dt);
            string[] colArr = distincCol.Split(',');
            for (int i = 0; i < colArr.Length; i++)
            {
                List<string> distCol = colArr.Take(colArr.Length - i).ToList();
                string tabName = "dt" + distCol.Last().Split(new char[] { ' ' })[0];
                if (distCol.Contains("L K"))
                {
                    distCol.Insert(distCol.IndexOf("L K"), "L");
                    distCol.Insert(distCol.IndexOf("L K"), "K");
                    distCol.RemoveAt(distCol.IndexOf("L K"));
                }
                /*
                if (distCol.Contains("TTm Ng"))
                {
                    distCol.Insert(distCol.IndexOf("TTm Ng"), "TTm");
                    distCol.Insert(distCol.IndexOf("TTm Ng"), "Ng");
                    distCol.RemoveAt(distCol.IndexOf("TTm Ng"));
                }
                if (distCol.Contains("TNg TNg1"))
                {
                    distCol.Insert(distCol.IndexOf("TNg TNg1"), "TNg");
                    distCol.Insert(distCol.IndexOf("TNg TNg1"), "TNg1");
                    distCol.RemoveAt(distCol.IndexOf("TNg TNg1"));
                }
                 */
                if (distCol.Contains("TNg2 TNg3"))
                {
                    distCol.Insert(distCol.IndexOf("TNg2 TNg3"), "TNg2");
                    distCol.Insert(distCol.IndexOf("TNg2 TNg3"), "TNg3");
                    distCol.RemoveAt(distCol.IndexOf("TNg2 TNg3"));
                }

                DataTable temp = view.ToTable(true, distCol.ToArray());
                temp.TableName = tabName;
                temp.Columns.Add(new DataColumn(colName, typeof(string)));

                // Dien ten noi dung ngan sach cho bang vua tao ra
                for (int j = 0; j < temp.Rows.Count; j++)
                {
                    string sXaunoima = "";
                    if (temp.Columns.Count >= 2 && temp.Columns.Count <= 5)
                    {
                        sXaunoima = temp.Rows[j][temp.Columns.Count - 2].ToString().Trim();
                    }
                    else
                    {
                        for (int k = 3; k < temp.Columns.Count - 1; k++)
                        {
                            if (k == 3)
                            {
                                sXaunoima += temp.Rows[j][k].ToString().Trim();
                            }
                            else
                            {
                                
                                sXaunoima += "-" + temp.Rows[j][k].ToString().Trim();
                                /*
                                if (sXaunoima.Trim().Contains("1040100-010-011-6900-6905-30-65-01-01-01"))
                                    sXaunoima = "";*/
                            }

                        }
                    }
                  
                        
                    foreach (DataRow dtr in dtName.Rows)
                    {
                        if (sXaunoima == dtr[colCheck].ToString().Trim())
                        {
                            temp.Rows[j][colName] = dtr[colName];
                        }
                    }
                }
                results.Tables.Add(temp);
            }
            return results;
        }

        /// <summary>
        /// Calculate parent of DataTable.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static void SumTable(DataTable source, string[] columnNames)
        {
            foreach (DataColumn col in source.Columns)
            {
                col.ReadOnly = false;
            }

            var sour = source.Select("IsParent=true", "Ma ASC");

            foreach (string colname in columnNames)
            {
                for (int j = sour.Length - 1; j >= 0; j--)
                {
                    decimal sum = 0;
                    for (int i = source.Rows.Count - 1; i >= 0; i--)
                    {
                        if (sour[j]["Ma"].ToString().Trim() == source.Rows[i]["MaHangCha"].ToString().Trim())
                        {
                            if (source.Rows[i][colname] == DBNull.Value)
                            {
                                source.Rows[i][colname] = 0;
                            }
                            sum += Convert.ToDecimal(source.Rows[i][colname]);
                        }
                    }
                    source.Rows[source.Rows.IndexOf(sour[j])][colname] = sum;
                }
            }
        }

        /// <summary>
        /// Remove Zero value of DataTable.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static void RmzTable(DataTable source, string[] columnNames)
        {
            DataRow[] rmv;
            if (columnNames.Length == 1)
            {
                rmv = source.Select(columnNames[0] + " is null or " + columnNames[0] + " =0");
            }
            else
            {
                string selString = "";
                for (int i = 0; i < columnNames.Length; i++)
                {
                    if (i == 0)
                    {
                        selString = "(" + columnNames[0] + " is null or " + columnNames[0] + " =0)";
                    }
                    else
                    {
                        selString += "and (" + columnNames[i] + " is null or " + columnNames[i] + " =0)";
                    }
                }
                rmv = source.Select(selString);
            }
            foreach (DataRow dtr in rmv)
            {
                source.Rows.RemoveAt(source.Rows.IndexOf(dtr));
            }
        }

        /// <summary>
        /// Mark Page for all rows of DataTable
        /// </summary>
        /// <param name="dt"></param>
        public static void MarkPage(DataTable dt, int fpItems, int maxItems)
        {
            dt.Columns.Add(new DataColumn("Toso", typeof(int)));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Toso"] = ((i + fpItems) / maxItems) + 1;
            }
        }

        public static string GetSumAsString(DataTable data, string p)
        {
            Decimal money = Convert.ToDecimal(data.Compute("SUM(" + p + ")", ""));
            string rs = ToStringMoney(money);
            if (money < 0)
            {
                rs = "Giảm " + rs;
            }
           
            return rs;
        }
        /// <summary>
        /// Get Table from accesfile with connectionString and scriptcommandtext OleDbConnection.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataFromSql(string connectionString, string sql, Dictionary<string, string> param = null)
        {
            var connectionStringName = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;

            using (var conn = new SqlConnection(connectionStringName))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (param != null)
                    {
                        for (int i = 0; i < param.Count; i++)
                        {
                            cmd.Parameters.AddWithValue("@" + param.ElementAt(i).Key, param.ElementAt(i).Value.ToString() == "-1" ? (object)DBNull.Value : param.ElementAt(i).Value.ToString());
                        }
                    }

                    return GetTable(cmd);
                }
            }
        }

        public static DataTable GetTable(SqlCommand cmd)
        {
            using (var da = new SqlDataAdapter(cmd))
            {
                using (var ds = new DataSet())
                {
                    try
                    {
                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.Write(ex);
#endif
                        return null;
                    }
                }
            }
        }
        #endregion

        #region private Methods
        #region to text money

        private static string ToStringMoney(int money)
        {
            return ToStringMoney((long)money);
        }

        private static string ToStringMoney(double money)
        {
            return ToStringMoney((long)money);
        }

        private static string ToStringMoney(decimal money)
        {
            return ToStringMoney((long)money);
        }

        private static string ToStringMoney(long Tien)
        {
            string text = "";
            if (Tien < 0)
            {
                return text;
            }
            string text2 = "";
            long num = 0L;
            string text3 = "không,một,hai,ba,bốn,năm,sáu,bảy,tám,chín";
            string text4 = ",nghìn,triệu,tỷ, nghìn, triệu, tỷ";
            string[] array = text3.Split(',');
            string[] array2 = text4.Split(',');
            do
            {
                long num2 = Tien % 10;
                Tien = (Tien - num2) / 10;
                long num3 = Tien % 10;
                Tien = (Tien - num3) / 10;
                long num4 = Tien % 10;
                Tien = (Tien - num4) / 10;
                if (num2 != 0 || num3 != 0 || num4 != 0)
                {
                    text2 = "";
                    if (num4 != 0 || Tien != 0)
                    {
                        text2 = array[num4] + " trăm";
                    }
                    switch (num3)
                    {
                        case 0L:
                            if (text2 != "" && num2 != 0)
                            {
                                text2 += " linh";
                            }
                            break;
                        case 1L:
                            text2 += " mười";
                            break;
                        default:
                            text2 = text2 + " " + array[num3] + " mươi";
                            break;
                    }
                    int num5;
                    switch (num2)
                    {
                        case 1L:
                            num5 = ((num3 < 2) ? 1 : 0);
                            goto IL_0171;
                        default:
                            num5 = 1;
                            goto IL_0171;
                        case 0L:
                            break;
                            IL_0171:
                            text2 = ((num5 != 0) ? ((num2 != 5 || num3 < 1) ? (text2 + " " + array[num2]) : (text2 + " lăm")) : (text2 + " mốt"));
                            break;
                    }
                    text2 = text2.Trim();
                    if (text2 != "")
                    {
                        if ((array2[num] == "tỷ" || array2[num] == "triệu" || array2[num] == "nghìn") && !string.IsNullOrWhiteSpace(text))
                        {
                            text = text2 + " " + array2[num] + ", " + text.Trim();

                        }
                        else
                        {
                            text = text2 + " " + array2[num] + " " + text.Trim();
                        }

                    }
                }
                num++;
            }
            while (Tien != 0);
            text = text.Trim();
            if (text == "")
            {
                text = "không";
            }
            text = text.Substring(0, 1).ToUpper() + text.Substring(1);
            return text + " đồng";
        }

        private static string ToStringNumber(int number)
        {
            return number.ToString("###,##0");
        }
        #endregion
        #endregion       
        
    }
    public class Param {
        public Object value { get; set; }
        public OleDbType type { get; set; }
      
    }
}
