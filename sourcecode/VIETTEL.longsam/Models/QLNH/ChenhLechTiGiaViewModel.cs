using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class ChenhLechTiGiaViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<ChenhLechTiGiaModel> Items { get; set; }
        public List<NS_DonVi> DonViList { get; set; }
        public IEnumerable<SelectListItem> ChuongTrinhList { get; set; }
        public List<NH_DA_HopDong> HopDongList { get; set; }
    }

    public class ChenhLechTiGiaModel
    {
        public string position { get; set; }
        public string sTen { get; set; }
        public string sTenDisplay { get => (IsBold == 1 ? string.Concat(position, ". ", sTen) : sTen); }
        public double? fTienKHTTBQPCapUSD { get; set; }
        public double? fTienKHTTBQPCapVND { get; set; }
        public double? fTienTheoHopDongUSD { get; set; }
        public double? fTienTheoHopDongVND { get; set; }
        public double? fKinhPhiDuocCapChoCDTUSD { get; set; }
        public double? fKinhPhiDuocCapChoCDTVND { get; set; }
        public double? fKinhPhiDaThanhToanUSD { get; set; }
        public double? fKinhPhiDaThanhToanVND { get; set; }
        public double? fTiGiaCLHopDongVsCDTUSD { get; set; }
        public double? fTiGiaCLHopDongVsCDTVND { get; set; }
        public double? fTiGiaCLKinhPhiDuocCapVsGiaiNganUSD { get; set; }
        public double? fTiGiaCLKinhPhiDuocCapVsGiaiNganVND { get; set; }
        public int IsBold { get; set; }
    }

    public class ChenhLechTiGiaExportModel
    {
        public string sTen { get; set; }
        public string sTienKHTTBQPCapUSD { get; set; }
        public string sTienKHTTBQPCapVND { get; set; }
        public string sTienTheoHopDongUSD { get; set; }
        public string sTienTheoHopDongVND { get; set; }
        public string sKinhPhiDuocCapChoCDTUSD { get; set; }
        public string sKinhPhiDuocCapChoCDTVND { get; set; }
        public string sKinhPhiDaThanhToanUSD { get; set; }
        public string sKinhPhiDaThanhToanVND { get; set; }
        public string sTiGiaCLHopDongVsCDTUSD { get; set; }
        public string sTiGiaCLHopDongVsCDTVND { get; set; }
        public string sTiGiaCLKinhPhiDuocCapVsGiaiNganUSD { get; set; }
        public string sTiGiaCLKinhPhiDuocCapVsGiaiNganVND { get; set; }
        public int IsBold { get; set; }
    }
}
