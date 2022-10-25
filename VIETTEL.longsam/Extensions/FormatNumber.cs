using FlexCel.Report;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace VTS.QLNS.CTC.App.Service.UserFunction
{
    public class FormatNumber : TFlexCelUserFunction
    {
        private int _maxDecimalPlace;
        private string _defaultIfZero;
        private int donVi = 1;

        public enum ExportType
        {
            EXCEL = 0,
            PDF = 1,
            WORD = 2,
            BROWSER = 3,
            SIGNATURE = 4,
            PDFONEPAPER = 5,
        }
        private ExportType _exportType;

        /*public FormatNumber(int dvt)
        {
            _exportType = ExportType.PDF;
            _maxDecimalPlace = dvt == 1 ? 1 : dvt == 1000 ? 3 : 9;
            _defaultIfZero = string.Empty;
        }*/

        public FormatNumber(int dvt, ExportType exportType)
        {
            _maxDecimalPlace = dvt == 1 ? 1 : dvt == 1001 ? 3 : 9;
            _defaultIfZero = string.Empty;
            _exportType = exportType;
            donVi = dvt;
        }

        /*public FormatNumber(int dvt, string defaultIfZero)
        {
            _exportType = ExportType.PDF;
            _maxDecimalPlace = dvt == 1 ? 1 : dvt == 1000 ? 3 : 9;
            _defaultIfZero = defaultIfZero;
        }*/

        public FormatNumber(int dvt, string defaultIfZero, ExportType exportType)
        {
            _exportType = ExportType.PDF;
            _maxDecimalPlace = dvt == 1 ? 1 : dvt == 1001 ? 3 : 9;
            _defaultIfZero = defaultIfZero;
            donVi = dvt;
        }

        public override object Evaluate(object[] parameters)
        {
            if (parameters.Length == 0) return string.Empty;
            double val = 10;
            if (parameters[0] != null)
            {
                if (parameters[0] is decimal)
                {
                    val = decimal.ToDouble((decimal)parameters[0]);
                }
                else if (parameters[0] is System.DBNull)
                {
                    return string.Empty;
                }
                else
                {
                    parameters[0] = parameters[0].ToString().Replace(",",".");
                    val = double.Parse(parameters[0].ToString())/ donVi;
                }
            }

            if (val == 0) return _defaultIfZero;
            return formatNumber(val);
        }

        private object formatNumber(double number)
        {
            if (_exportType == ExportType.PDF)
            {
                CultureInfo vi = new CultureInfo("vi-VN", true);
                vi.NumberFormat.NumberDecimalSeparator = ",";
                vi.NumberFormat.NumberGroupSeparator = ".";
                return Regex.Replace(String.Format(vi, "{0:n" + _maxDecimalPlace + "}", number),
                                     @"[" + vi.NumberFormat.NumberDecimalSeparator + "]?0+$", "");
            }
            if (_exportType == ExportType.EXCEL)
            {
                CultureInfo vi = new CultureInfo("vi-VN", true);
                vi.NumberFormat.NumberDecimalSeparator = ",";
                vi.NumberFormat.NumberGroupSeparator = ".";
                return Regex.Replace(String.Format(vi, "{0:n" + _maxDecimalPlace + "}", number),
                                     @"[" + vi.NumberFormat.NumberDecimalSeparator + "]?0+$", "");
            }
            return number;
        }
    }
}
