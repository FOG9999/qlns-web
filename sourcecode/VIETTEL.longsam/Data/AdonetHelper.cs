using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DapperExtensions;
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace Viettel.Data
{
    public static class AdonetHelper
    {
        #region query

        public static SqlCommand AddParams(this SqlCommand cmd, dynamic param)
        {
            var dic = new Dictionary<string, object>();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(param))
            {
                var value = prop.GetValue(param);
                var name = prop.Name;
                dic.Add(name, value);
            }

            var sqlParams = new List<SqlParameter>();
            dic.ToList().ForEach(x =>
            {
                sqlParams.Add(new SqlParameter(x.Key, x.Value));
            });
            cmd.Parameters.AddRange(sqlParams.ToArray());

            return cmd;
        }

        public static SqlCommand AddParams(this SqlCommand cmd, Dictionary<string, string> filters, string format = "%{0}%")
        {
            filters.ToList().ForEach(x =>
            {
                cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : string.Format(format, x.Value));
            });

            return cmd;
        }

        public static DataSet ExecuteSelect(string connectionString,
            string sql,
            object param = null,
            CommandType cmdType = CommandType.StoredProcedure)
        {
            var da = new SqlDataAdapter(sql, connectionString)
            {
                SelectCommand =
                {
                    CommandType = cmdType
                }
            };

            if (param != null)
                da.SelectCommand.AddParams(param);

            var ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public static DataSet ExecuteSelect(IDbConnection connection,
           string sql,
           object param = null,
           CommandType cmdType = CommandType.StoredProcedure)
        {
            var da = new SqlDataAdapter(sql, (SqlConnection)connection)
            {
                SelectCommand =
                {
                    CommandType = cmdType
                }
            };

            if (param != null)
                da.SelectCommand.AddParams(param);

            var ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public static DataTable ExecuteDataTable(string connectionString,
           string sql,
           object param = null,
           CommandType cmdType = CommandType.StoredProcedure)
        {
            var ds = ExecuteSelect(connectionString, sql, param, cmdType);
            return ds.Tables[0];
        }

        public static DataTable ExecuteDataTable(this IDbConnection connection,
         string commandText,
         object parameters = null,
         CommandType commandType = CommandType.Text)
        {
            var ds = ExecuteSelect(connection, commandText, parameters, commandType);
            return ds.Tables[0];
        }


        //private static DataSet execute(string connectionString,
        //   string sql,
        //   string sqlType = "select",
        //   object param = null,
        //   CommandType cmdType = CommandType.StoredProcedure)
        //{
        //    DataSet ds = new DataSet();
        //    var da = new SqlDataAdapter(sql, connectionString);
        //    switch (sqlType)
        //    {
        //        case "select":
        //            {
        //                da.SelectCommand.CommandType = cmdType;
        //                break;
        //            }
        //        case "insert":
        //            {
        //                da.InsertCommand.CommandType = cmdType;
        //                break;
        //            }
        //        case "update":
        //            {
        //                da.UpdateCommand.CommandType = cmdType;
        //                break;
        //            }

        //        default:
        //            break;
        //    }
        //    if (param != null)
        //        da.SelectCommand.AddParams(param);
        //    da.Fill(ds);
        //    return ds;
        //}



        #endregion

        #region dataset
        public static DataSet GetDataset(this SqlCommand cmd, object parameters = null)
        {
            using (var da = new SqlDataAdapter(cmd))
            {
                using (var ds = new DataSet())
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.AddParams(parameters);
                        da.Fill(ds);

                        return ds;
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
        #region datatable 

        public static DataTable GetTable(this SqlCommand cmd)
        {
            cmd.CommandTimeout = 600;

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

        public static DataTable GetTable(this SqlConnection connection,
         string commandText,
         object parameters = null,
         CommandType commandType = CommandType.Text)
        {
            using (var cmd = new SqlCommand(commandText, connection))
            {
                cmd.CommandType = commandType;
                cmd.CommandTimeout = 600;
                cmd.AddParams(parameters);
                return cmd.GetTable();
            }

        }

        public static string GetValue(this SqlCommand cmd)
        {
            var dt = cmd.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }

            return string.Empty;
        }

        //        public static int InsertIdentity<T>(this IDbConnection conn, T obj)
        //        {
        //            var result = 0;

        //            var sql = @"

        //DECLARE @id int;
        //INSERT INTO [@table] (@columns) values (@values)
        //SELECT @ID=SCOPE_IDENTITY()

        //";
        //            var columns = new List<string>();
        //            var values = new List<string>();

        //            typeof(T).GetProperties().ToList()
        //                .ForEach(p =>
        //                {
        //                    var isPK = p.GetCustomAttributes(true).Any(x => x is KeyAttribute);
        //                    if (!isPK)
        //                    {
        //                        columns.Add($"@{p.Name}");

        //                        var value = p.GetValue(obj);
        //                        if (value == null)
        //                            values.Add("NULL");
        //                        else if (value.GetType() == typeof(string))
        //                        {
        //                            values.Add($"N'{value}'");
        //                        }
        //                        else //if(value.GetType() == typeof(Guid))
        //                        {
        //                            values.Add($"'{value}'");
        //                        }
        //                    }
        //                });

        //            sql = sql
        //                .Replace("@table", typeof(T).Name)
        //                .Replace("@columns", columns.Join())
        //                .Replace("@values", values.Join());

        //            result = conn.QueryFirstOrDefault<int>(sql);


        //            return result;
        //        }

        public static int InsertIdentity<T>(this IDbConnection conn, T obj)
        {
            var result = 0;

            var sql = @"

DECLARE @id int;
INSERT INTO [@table] (@columns) values (@values)
SELECT @ID=SCOPE_IDENTITY()

";
            var columns = new List<string>();
            var values = new List<string>();

            typeof(T).GetProperties().ToList()
                .ForEach(p =>
                {
                    var isPK = p.GetCustomAttributes(true).Any(x => x is KeyAttribute);
                    if (!isPK)
                    {
                        columns.Add($"{p.Name}");

                        var value = p.GetValue(obj);

                        if (p.PropertyType == typeof(string))
                        {
                            values.Add(value == null ? "''" : $"N'{value}'");
                        }
                        else
                            values.Add($"@{p.Name}");
                    }
                });

            sql = sql
                .Replace("@table", typeof(T).Name)
                .Replace("@columns", columns.Join())
                .Replace("@values", values.Join());

            result = conn.QueryFirstOrDefault<int>(sql, obj);


            return result;
        }


        #endregion

        #region helpers

        public static object ParamString(string value, string defaultValue = "-1")
        {
            if (string.IsNullOrWhiteSpace(value))
                return DBNull.Value;

            var isDefault = defaultValue.Split(new char[] { ',' }).Contains(value);
            return string.IsNullOrWhiteSpace(value) || isDefault ? DBNull.Value : (object)value;
        }

        public static object ToParamString(this string value)
        {
            return ParamString(value);
        }

        public static string ToParamList(this string value)
        {
            if (value.IsEmpty()) return "";
            else
                return value.ToList().Select(x => $"'{x}'").Join();
        }

        public static object ToParamString(this string value, bool condition = false)
        {
            return string.IsNullOrWhiteSpace(value) || condition ? DBNull.Value : (object)value;
        }

        public static object ToParamString<T>(this T value, bool condition = false)
        {
            //return string.IsNullOrWhiteSpace(value.ToString()) || condition ? DBNull.Value : (object)value;
            return ParamString(value.ToString());
        }

        public static DateTime ParamDate(string date, DateTime? defaultValue = null)
        {
            var d = DateTime.ParseExact(date, "dd/MM/yyyy", new System.Globalization.CultureInfo("vi-VN").DateTimeFormat);
            return d;
        }

        public static DateTime ToParamDate(this string date, DateTime? defaultValue = null)
        {
            //var dt = ParamDate(date, defaultValue);
            //return ToParamDate(dt);
            return ParamDate(date, defaultValue);
        }

        public static object ToParamDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static object ToParamDateVN(this string date)
        {
            var dt = ParamDate(date);
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToParamLike(this string value, string col, string formatLike = "'%{0}%'")
        {
            return ToParamLike(value.Split(','), col, formatLike);
        }

        public static string ToParamLikeStartWith(this string value, string col)
        {
            return ToParamLike(value.Split(','), col, "'{0}%'");
        }

        public static string ToParamLikeEndWith(this string value, string col)
        {
            return ToParamLike(value.Split(','), col, "'%{0}'");
        }

        public static string ToParamLike(this IEnumerable<string> values, string col, string formatLike = "'%{0}%'")
        {
            var p = values
                         .Select(x => x.Contains('%') ? string.Format("'{0}'", x) : string.Format(formatLike, x))
                         .Join(string.Format(" or {0} like ", col));
            return p;
        }


        #endregion

        #region nice sql

        public static string ToSqlLower(this string sql)
        {
            var list = "SELECT,FROM,WHERE,AND,OR,IN".ToList();
            foreach (var item in list)
            {
                sql = sql.Replace(item, item.ToLower());
            }

            return sql;
        }

        #endregion

    }
}
