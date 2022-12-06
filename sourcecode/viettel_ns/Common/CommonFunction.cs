using DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Common
{
    public static class CommonFunction
    {
        private static readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private static readonly INganSachNewService _nsService = NganSachNewService.Default;

        public static SelectList GetDataDropDownDonViQuanLy(string[] lstDonViExclude = null)
        {
            var lstData = DonViModels.Get_dtDonVi();
            List<SelectListItem> lstCbx = new List<SelectListItem>();
            foreach (DataRow dr in lstData.Rows)
            {
                if (lstDonViExclude != null && !lstDonViExclude.Contains(Convert.ToString(dr["iCapNS"]))) continue;
                lstCbx.Add(new SelectListItem()
                {
                    Text = Convert.ToString(dr["iID_MaDonVi"]) + " - " + Convert.ToString(dr["sTen"]),
                    Value = Convert.ToString(dr["iID_Ma"]) + "|" + Convert.ToString(dr["iID_MaDonVi"])
                });
            }   
            return lstCbx.ToSelectList();
        }

        public static SelectList GetDataDropDownNguonNganSach()
        {
            var lstData = DanhMucModels.NS_NguonNganSach();
            if (lstData == null || lstData.Rows.Count <= 0) return new SelectList(null);
            return lstData.ToSelectList("iID_MaNguonNganSach", "sTen");
        }

        public static SelectList GetDataDropDownMucLucNganSach(int iNamLamViec)
        {
            var lstData = _nsService.GetDataDropdownMucLucNganSachInVDT(iNamLamViec);
            List<SelectListItem> lstCbx = new List<SelectListItem>();
            foreach (var item in lstData)
            {
                lstCbx.Add(new SelectListItem()
                {
                    Text = item.sXauNoiMa,
                    Value = GetValueDropdownMLNS(item)
                });
            }
            return lstCbx.ToSelectList();
        }

        public static List<NS_DonVi> GetListDataDonViQuanLy(string[] lstDonViExclude = null)
        {
            List<NS_DonVi> results = new List<NS_DonVi>();
            var lstData = DonViModels.Get_dtDonVi();
            foreach (DataRow dr in lstData.Rows)
            {
                if (lstDonViExclude != null && !lstDonViExclude.Contains(Convert.ToString(dr["iCapNS"]))) continue;
                results.Add(new NS_DonVi()
                {
                    sTenLoaiDonVi = Convert.ToString(dr["sTen"]),
                    iID_MaDonVi = Convert.ToString(dr["iID_MaDonVi"]),
                    iID_Ma = Guid.Parse(Convert.ToString(dr["iID_Ma"])),
                    sMoTa = Convert.ToString(dr["iID_MaDonVi"]) + " - " + Convert.ToString(dr["sTen"]),
                });
            }
            return results;
        }

        #region Helper
        private static string GetValueDropdownMLNS(NS_MucLucNganSach item)
        {
            if (!string.IsNullOrEmpty(item.sNG))
                return string.Format("{0}|{1}", "Ng", item.iID_MaMucLucNganSach.ToString());
            if (!string.IsNullOrEmpty(item.sTTM))
                return string.Format("{0}|{1}", "Ttm", item.iID_MaMucLucNganSach.ToString());
            if (!string.IsNullOrEmpty(item.sTM))
                return string.Format("{0}|{1}", "Tm", item.iID_MaMucLucNganSach.ToString());
            if (!string.IsNullOrEmpty(item.sM))
                return string.Format("{0}|{1}", "M", item.iID_MaMucLucNganSach.ToString());
            return string.Empty;
        }
        #endregion

        public static string ConvertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }

        public static string ConvertLaMa(decimal num)
        {
            string strRet = string.Empty;
            decimal _Number = num;
            bool _Flag = true;
            string[] ArrLama = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            int[] ArrNumber = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            int i = 0;
            while (_Flag)
            {
                while (_Number >= ArrNumber[i])
                {
                    _Number -= ArrNumber[i];
                    strRet += ArrLama[i];
                    if (_Number < 1) _Flag = false;
                }
                i++;
            }
            return strRet;
        }

        public static string DinhDangSo(object So, int SoSauDauPhay, bool removeSign = false)
        {
            string result = "";
            bool flagSign = false;
            string text = Convert.ToString(So, new CultureInfo("en-US"));
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                double num = Convert.ToDouble(text);
                if (num == 0.0)
                {
                    return result;
                }
            }
            else if (string.IsNullOrEmpty(text))
            {
                return result;
            }

            if (So != null)
            {
                int num2 = text.IndexOf("-");
                if (num2 >= 0)
                {
                    flagSign = true;
                    text = text.Replace("-", "");
                }

                int num3 = text.IndexOf(".");
                int num4 = 0;
                string text2 = "";
                if (num3 >= 0)
                {
                    text2 = text.Substring(num3 + 1, text.Length - num3 - 1);
                    text = text.Substring(0, num3);
                }

                int length = text.Length;
                if (text.Length > 3)
                {
                    for (num2 = length; num2 > 1; num2--)
                    {
                        num4++;
                        if (num4 % 3 == 0)
                        {
                            text = ((!Globals.KieuSoVietNam) ? text.Insert(num2 - 1, ",") : text.Insert(num2 - 1, "."));
                        }
                    }
                }

                if (SoSauDauPhay >= 0)
                {
                    string text3 = "";
                    for (num3 = 0; num3 < SoSauDauPhay; num3++)
                    {
                        text3 = ((num3 >= text2.Length) ? (text3 + "0") : (text3 + text2[num3]));
                    }

                    text2 = text3;
                }

                if (text2 != "")
                {
                    text = ((!Globals.KieuSoVietNam) ? (text + "." + text2) : (text + "," + text2));
                }

                if (flagSign && !removeSign)
                {
                    text = "-" + text;
                }

                result = text;
            }

            return result;
        }

        public static string UCS2Convert(string sContent)
        {
            sContent = sContent.Trim().ToLower();
            var sUTF8Lower = "a|á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ|đ|e|é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ|i|í|ì|ỉ|ĩ|ị|o|ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ|u|ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự|y|ý|ỳ|ỷ|ỹ|ỵ";
            var sUCS2Lower = "a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|d|e|e|e|e|e|e|e|e|e|e|e|e|i|i|i|i|i|i|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|u|u|u|u|u|u|u|u|u|u|u|u|y|y|y|y|y|y";
            var aUTF8Lower = sUTF8Lower.Split(new[] { '|' });
            var aUCS2Lower = sUCS2Lower.Split(new[] { '|' });

            var l = aUTF8Lower.GetUpperBound(0);

            for (var i = 1; i < l; i++)
            {
                sContent = sContent.Replace(aUTF8Lower[i], aUCS2Lower[i]);
            }
            sContent = sContent.Replace(" ", "-");
            var list = new List<string> { "/", "?", "&", ":", "#", "*", "\"", "@", "%", "$", "!", "~" };
            sContent = list.Aggregate(sContent, (current, str) => current.Replace(str, "-"));

            const string filter = ".-_[]0123456789abcdefghijklmnopqrstuvwxyz";
            var s = "";
            l = sContent.Length;
            for (var i = 0; i < l; i++)
            {
                if (filter.IndexOf(sContent[i]) >= 0)
                {
                    s = s + sContent[i];
                }
            }

            return s;
        }

        public static double? TryParseDouble(string sGiaTri)
        {
            double fGiaTri;
            if (!double.TryParse(sGiaTri, NumberStyles.Any, CultureInfo.InvariantCulture, out fGiaTri))
            {
                return null;
            }
            else
            {
                return fGiaTri;
            }
        }

        public static int? TryParseInt(string sGiaTri)
        {
            int fGiaTri;
            if (!int.TryParse(sGiaTri, NumberStyles.Any, CultureInfo.InvariantCulture, out fGiaTri))
            {
                return null;
            }
            else
            {
                return fGiaTri;
            }
        }

        public static DateTime? TryParseDateTime(string sGiaTri)
        {
            DateTime dGiaTri;
            if (!DateTime.TryParse(sGiaTri, out dGiaTri))
            {
                return null;
            }
            else
            {
                return dGiaTri;
            }
        }

        public static bool CheckAlphanumeric(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]*$");
        }

        public static bool CheckPhoneOrFax(string value)
        {
            return Regex.IsMatch(value, @"^(([+]?([\s]?[0-9]+))|([(][+]?([\s]?[0-9]+)[)]))?([\s]?[(]?[0-9]+[)])?([-]?[\s]?[0-9])+$");
        }

        public static bool CheckEmail(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]+(([-._]{1}[a-zA-Z0-9]+)*)@(([a-zA-Z0-9\-]+\.)+[a-zA-Z0-9]{2,})$");
        }

        public static string NumberToText(double inputNumber, bool suffix = true)
        {
            inputNumber = Math.Round(inputNumber);
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString();
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }

            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "mốt " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Giảm " + result;
            if (string.IsNullOrEmpty(result))
            {
                result = "Không";
            }
            return FirstLetterToUpper(result + (suffix ? " " + "đồng" : " đô la Mỹ"));
        }

        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1) + ".";
            return str.ToUpper() + ".";
        }
    }
}