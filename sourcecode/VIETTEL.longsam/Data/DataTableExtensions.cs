using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

public static class ExtensionMethods_DataTable
{
    #region Select Distinct
    /// <summary>
    /// "SELECT DISTINCT" over a DataTable
    /// </summary>
    /// <param name="SourceTable">Input DataTable</param>
    /// <param name="FieldNames">Fields to select (distinct)</param>
    /// <returns></returns>
    public static DataTable SelectDistinct(this DataTable SourceTable, string tableName, string FieldName)
    {
        return SelectDistinct(SourceTable, tableName, FieldName, string.Empty);
    }

    /// <summary>
    ///"SELECT DISTINCT" over a DataTable
    /// </summary>
    /// <param name="SourceTable">Input DataTable</param>
    /// <param name="FieldNames">Fields to select (distinct)</param>
    /// <param name="Filter">Optional filter to be applied to the selection</param>
    /// <returns></returns>
    public static DataTable SelectDistinct(this DataTable SourceTable, string tableName, string FieldNames, string Filter)
    {
        DataTable dt = new DataTable();
        dt.TableName = tableName;

        string[] arrFieldNames = FieldNames.Replace(" ", "").Split(',');
        foreach (string s in arrFieldNames)
        {
            if (SourceTable.Columns.Contains(s))
                dt.Columns.Add(s, SourceTable.Columns[s].DataType);
            else
                throw new Exception(string.Format("The column {0} does not exist.", s));
        }

        object[] LastValues = null;
        foreach (DataRow dr in SourceTable.Select(Filter, FieldNames))
        {
            object[] NewValues = GetRowFields(dr, arrFieldNames);
            if (LastValues == null || !(objectComparison(LastValues, NewValues)))
            {
                LastValues = NewValues;
                dt.Rows.Add(LastValues);
            }
        }

        return dt;
    }

    #endregion

    #region Select Distinct
    /// <summary>
    /// "SELECT DISTINCT" over a DataTable
    /// </summary>
    /// <param name="SourceTable">Input DataTable</param>
    /// <param name="FieldNames">Fields to select (distinct)</param>
    /// <returns></returns>
    //public static DataTable SelectDistinct(
    //    this DataTable SourceTable, 
    //    string tableName, 
    //    string FieldName, 
    //    string fieldAdd,
    //    string filter)
    //{
    //    return SelectDistinct(SourceTable, tableName, FieldName, fieldAdd, filter);
    //}

    /// <summary>
    ///"SELECT DISTINCT" over a DataTable
    /// </summary>
    /// <param name="SourceTable">Input DataTable</param>
    /// <param name="FieldNames">Fields to select (distinct)</param>
    /// <param name="Filter">Optional filter to be applied to the selection</param>
    /// <returns></returns>
    public static DataTable SelectDistinct(
        this DataTable SourceTable,
        string TableName,
        string FieldName,
        string FieldAdd,
        string Filter)
    {
        DataTable dt = new DataTable(TableName);
        string[] arrFieldAdd = FieldAdd.Split(',');
        string[] arrFieldName = FieldName.Split(',');
        for (int i = 0; i < arrFieldAdd.Length; i++)
        {
            dt.Columns.Add(arrFieldAdd[i], SourceTable.Columns[arrFieldAdd[i]].DataType);
        }
        if (SourceTable.Rows.Count > 0)
        {
            object[] LastValue = new object[arrFieldName.Length];
            for (int i = 0; i < LastValue.Length; i++)
            {
                LastValue[i] = null;
            }

            foreach (DataRow dr in SourceTable.Select("", FieldName + " " + Filter))
            {
                var ok = false;
                for (int i = 0; i < arrFieldName.Length; i++)
                {
                    ok = LastValue[i] == null || !objectComparison(LastValue[i], dr[arrFieldName[i]]);
                    if (ok)
                        break;
                }
                for (int i = 0; i < arrFieldName.Length; i++)
                {
                    if (ok)
                    {
                        LastValue[i] = dr[arrFieldName[i]];
                    }
                }
                if (ok)
                {
                    DataRow R = dt.NewRow();
                    for (int j = 0; j < arrFieldAdd.Length; j++)
                    {
                        R[arrFieldAdd[j]] = dr[arrFieldAdd[j]];
                    }
                    dt.Rows.Add(R);
                }
            }
        }
        return dt;
    }

    #endregion

    public static IEnumerable<string> GetColumnNames(this DataTable dt)
    {
        var columnNames = new List<string>();
        foreach (DataColumn c in dt.Columns)
        {
            columnNames.Add(c.ColumnName);
        }
        return columnNames;
    }

    #region Private Methods
    private static object[] GetRowFields(DataRow dr, string[] arrFieldNames)
    {
        if (arrFieldNames.Length == 1)
            return new object[] { dr[arrFieldNames[0]] };
        else
        {
            ArrayList itemArray = new ArrayList();
            foreach (string field in arrFieldNames)
                itemArray.Add(dr[field]);

            return itemArray.ToArray();
        }
    }

    /// <summary>
    /// Compares two values to see if they are equal. Also compares DBNULL.Value.
    /// </summary>
    /// <param name="A">object A</param>
    /// <param name="B">object B</param>
    /// <returns></returns>
    private static bool objectComparison(object a, object b)
    {
        if (a == DBNull.Value && b == DBNull.Value) //  both are DBNull.Value
            return true;
        if (a == DBNull.Value || b == DBNull.Value) //  only one is DBNull.Value
            return false;
        return (a.Equals(b));  // value type standard comparison
    }

    /// <summary>
    /// Compares two value arrays to see if they are equal. Also compares DBNULL.Value.
    /// </summary>
    /// <param name="A">object Array A</param>
    /// <param name="B">object Array B</param>
    /// <returns></returns>
    private static bool objectComparison(object[] a, object[] b)
    {
        bool retValue = true;
        bool singleCheck = false;

        if (a.Length == b.Length)
            for (var i = 0; i < a.Length; i++)
            {
                if (!(singleCheck = objectComparison(a[i], b[i])))
                {
                    retValue = false;
                    break;
                }
                retValue = retValue && singleCheck;
            }

        return retValue;
    }
    #endregion


    public static double Sum(this DataTable dt, string col)
    {
        var sum = 0d;

        sum = dt.AsEnumerable()
            .Select(x => Convert.ToDouble(x[col].ToString()))
            .ToList()
            .Sum();


        return sum;
    }

    public static void FilterRow(this DataTable dt, Func<DataRow, bool> func)
    {
        var items = new List<DataRow>();

        dt.AsEnumerable()
            .ToList()
            .ForEach(r =>
            {
                if (!func(r))
                {
                    items.Add(r);
                }
            });

        items.ForEach(dt.Rows.Remove);
    }

    public static T Field<T>(this DataRow row, string columnName, T defaultValue)
    {
        T result = defaultValue;

        try
        {
            var value = row.Field<T>(columnName);
            result = value;
        }
        catch
        {
            try
            {
                var value = row[columnName].ToString().ToValue<T>();
                return value;
            }
            catch
            {

            }
        }

        return result;
    }

    #region hiep
    public static void CountDistinctRow(this DataTable dt, string rangs)
    {
        string[] rang = rangs.Split(',');
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
    }

    public static void NullToZero(this DataTable dt, string[] colNames)
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
    #endregion

    #region fluent datatable

    public static DataTable SetTableName(this DataTable dt, string tableName)
    {
        dt.TableName = tableName;
        return dt;
    }

    public static DataTable AddOrdinalsNum(this DataTable dt, int level, string rang = null)
    {
        DataTable vR = dt;
        vR.Columns.Add(new DataColumn($"STT", typeof(string)));
        int start = 1;
        var dtRang = new DataTable();
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
                        vR.Rows[i]["STT"] = NumberExtension.NToR(start);
                    }
                    else if (level == 3)
                    {
                        vR.Rows[i]["STT"] = start;
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
                        vR.Rows[i]["STT"] = NumberExtension.NToR(i + 1);
                    }
                    else if (level == 3)
                    {
                        vR.Rows[i]["STT"] = i + 1;
                    }
                }
                else
                {
                    vR.Rows[i]["STT"] = "-";
                }
            }
        }
        return vR;
    }
    #endregion
}
