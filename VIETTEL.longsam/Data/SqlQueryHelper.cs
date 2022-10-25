using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIETTEL.Data
{


    public class SqlQuery
    {
        private string _sqlWhere;
        private string _sql;

        public SqlQuery()
        {

        }

        public SqlQuery(string sql)
        {
            _sql = sql;
        }

        #region private methods

        public string Where
        {
            get { return _sqlWhere; }
        }

        private bool isWhereNull
        {
            get { return string.IsNullOrWhiteSpace(_sqlWhere); }
        }

        #endregion 

        #region and

        public SqlQuery Ini(string sql)
        {
            _sqlWhere += sql;
            return this;
        }


        public SqlQuery And(string col, object value)
        {
            if (value == null)
                return this;

            //_sql += first ? $" {col}='{value}'" : $" AND {col}='{value}'";
            _sqlWhere += string.IsNullOrWhiteSpace(_sqlWhere) ? $" {col}='{value}'" : $" AND {col}='{value}'";

            return this;
        }

        public SqlQuery And(string sql, bool condition = true)
        {
            if (condition)
            {
                _sqlWhere += string.IsNullOrWhiteSpace(_sqlWhere) ? sql : $" AND {sql}";

                //_sqlWhere += sql;
            }
            return this;
        }

        public SqlQuery AndParam(string col)
        {
            _sqlWhere += $" AND {col}=@{col}";

            return this;
        }
        #endregion

        #region or


        public SqlQuery Or(string col, object value)
        {
            if (value == null)
                return this;


            _sqlWhere += string.IsNullOrWhiteSpace(_sqlWhere) ? $" {col}={value}" : $" OR {col}={value}";

            return this;
        }


        public SqlQuery OrParam(string col)
        {
            _sqlWhere += $" OR {col}=@{col}";

            return this;
        }

        public SqlQuery OrLike(string col, object value, bool exact = false)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return this;

            _sqlWhere += exact ? $" OR {col} = '{value}'" : $" OR {col} LIKE '%{value}%'";
            return this;
        }

        #endregion

        #region like

        public SqlQuery Like(string col, object value)
        {
            if (value == null)
                return this;

            _sqlWhere += string.IsNullOrWhiteSpace(_sqlWhere) ? $" {col} LIKE '%{value}%'" : $" AND {col} LIKE '%{value}%'";
            return this;
        }

        public SqlQuery LikeParam(string col, object value)
        {
            if (value == null)
                return this;


            _sqlWhere += isWhereNull ? $"{col} LIKE '%@{col}%'" : $" AND {col} LIKE '%@{col}%'";
            return this;
        }

        public SqlQuery AndLike(string col, string value, bool exact = false)
        {
            if (value == null) return this;

            if (exact)
            {
                And(col, value);
            }
            else
            {
                Like(col, value);
            }

            return this;
        }


        #endregion

        #region page

        public SqlQuery Page(string table, string where, string orderBy = "Id", int page = 1, int pageSize = 10)
        {
            var sqlFormat = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY {2}) AS RowNum, *
                                      FROM      {0}
                                      {1}
                                    ) AS result
                            WHERE   RowNum >= {3}
                                AND RowNum < {4}
                            ORDER BY RowNum";

            if (page < 1)
                page = 1;

            var pageFrom = (page - 1) * pageSize + 1;
            var pageTo = pageFrom + pageSize;

            var sql = string.Format(sqlFormat, table, where, orderBy, pageFrom, pageTo);
            _sql = sql;

            return this;
        }

        #endregion

        public string ToSql()
        {
            return _sql + (isWhereNull ? "" : $" WHERE {_sqlWhere}");
        }
    }
    public static class SqlQueryHelper
    {
    }
}
