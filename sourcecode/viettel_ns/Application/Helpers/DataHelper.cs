using System;
using System.Collections.Generic;
using System.Web.Mvc;
using VIETTEL.Models;

namespace VIETTEL.Helpers
{
    public static class DataHelper
    {
        public static SelectList ToSelectList(this IEnumerable<KeyViewModel> source)
        {
            return new SelectList(source, "Key", "Text");
        }

        public static IEnumerable<KeyViewModel> GetQuys()
        {
            return new List<KeyViewModel>()
            {
                new KeyViewModel("1", "Qúy I"),
                new KeyViewModel("2", "Qúy II"),
                new KeyViewModel("3", "Qúy III"),
                new KeyViewModel("4", "Qúy IV"),
                new KeyViewModel("5", "Bổ sung"),
            };
        }

        public static IEnumerable<KeyViewModel> GetNamNganSachList()
        {
            return new List<KeyViewModel>()
            {
                new KeyViewModel("1,2,4,5", "Tổng hợp"),
                new KeyViewModel("2,4", "Năm nay"),
                new KeyViewModel("1,5", "Năm trước"),
            };
        }

        public static Dictionary<string, string> GetNguonNganSachList()
        {
            return new Dictionary<string, string>()
            {
                {"-1", "Tổng hợp" },
                {"1", "1 - Ngân sách quốc phòng" },
                {"2,3", "2 - Ngân sách nhà nước" },
                {"4", "4 - Ngân sách khác" },
            };
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
                        result = "một " + result;
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
            return FirstLetterToUpper(result + (suffix ? " " + "đồng" : ""));
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
