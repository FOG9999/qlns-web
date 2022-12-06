using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QuyetToan.Models
{
    public class QuyViewModel
    {
        public int MaQuy { get; set; }

        public string TenQuy { get; set; }

        public static IEnumerable<QuyViewModel> GetAll(bool all = false)
        {
            var list = new List<QuyViewModel>()
            {
                new QuyViewModel { MaQuy = 1, TenQuy = "Quý I" },
                new QuyViewModel { MaQuy = 2, TenQuy = "Quý II" },
                new QuyViewModel { MaQuy = 3, TenQuy = "Quý III" },
                new QuyViewModel { MaQuy = 4, TenQuy = "Quý IV" },
                new QuyViewModel { MaQuy = 5, TenQuy = "Bổ sung" },
            };

            if (all)
            {
                list.Insert(0, new QuyViewModel { MaQuy = 0, TenQuy = "-- QUÝ --" });
            }
            return list;
        }
    }
}