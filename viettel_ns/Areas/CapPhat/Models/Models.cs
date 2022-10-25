using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.CapPhat.Models
{

    public class ChungTuChiTietViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public string Id_ChungTu { get; set; }
        public CP_CapPhat Entity { get; set; }
        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }



    }

    public class CapPhat_ChungTu_SheetTableFilterType
    {
        public const int All = 0;
        //public const int CoChiTieu = 1;
        //public const int KhongCoChiTieu = 2;
        public const int DaNhap = 1;
        //public const int CoQuyetToanKhacDeNghi = 4;

        private static Dictionary<int, string> _items;
        public static Dictionary<int, string> Items
        {
            get
            {
                return _items ?? (_items = new Dictionary<int, string>()
                {
                    { All, "Tất cả"},
                    { DaNhap, "Đã nhập số liệu"},
                });
            }
        }
    }

    //public class CapPhatSheetViewModel
    //{
    //    public CapPhatSheetViewModel()
    //    {
    //        FilterOptions = new Dictionary<string, string>();
    //    }

    //    public SheetViewModel Sheet { get; set; }

    //    public CP_CapPhat ChungTuViewModel { get; set; }

    //    public Guid Id { get; set; }

    //    public Dictionary<string, string> FilterOptions { get; set; }
    //}


}
