using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{ 
    public class ThongTriCapPhatModel
    {
        public int STT { get; set; }
        public Guid? ID { get; set; }
        public string sSoThongTri { get; set; }
        public DateTime? dNgayLap { get; set; }
        public DateTime? dNgayTao { get; set; }
        public Guid? iID_DonViID { get; set; }
        public int iNamThongTri { get; set; }
        public int iLoaiThongTri { get; set; }
        public int? iLoaiNoiDungChi { get; set; }
        public double? fThongTri_VND { get; set; }
        public string sThongTri_VND { get; set; }
        public string _sThongTri_VND {
            get { return fThongTri_VND != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(fThongTri_VND.Value, 0).ToString(CultureInfo.InvariantCulture), 0) : String.Empty; }
        }
        public double? fThongTri_USD { get; set; }
        public string sThongTri_USD { get; set; }
        public string _sThongTri_USD { 
                get { return fThongTri_USD != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(fThongTri_USD.Value, 2).ToString(CultureInfo.InvariantCulture), 2) : String.Empty;    }
            }
        public string sIdThanhToans { get; set; }
        public string sTenDonvi { get; set; }
        public string sLoaiThongTri { get; set; }
        public string sLoaiNoiDung { get; set; }

        public  string sNgayLap
        {
            get { return dNgayLap.HasValue ? dNgayLap.Value.ToString("dd/MM/yyyy"): String.Empty; }
        }
    }
    public class ThongTriCapPhatModelSearch
    {
        public PagingInfo _paging { get; set; }
        public Guid? iThongTri { get; set; }
        public Guid? iDonVi { get; set; }
        public int? iNam { get; set; }
        public int? iLoaiThongTri { get; set; }
        public int? iLoaiNoiDung { get; set; }
        public string sSongThongTri { get; set; }
        public DateTime? dNgayLap { get; set; }
        public int? iTypeAction { get; set; }
    }

    public class ThanhToan_ThongTriModel
    {
        public int STT { get; set; }
        public Guid? ID { get; set; }
        public Guid? iDonVi { get; set; }
        public string sTenDonVi { get; set; }
        public string sSoDeNghi { get; set; }
        public DateTime? dNgayDeNghi { get; set; }
        public int iLoaiNoiDungChi { get; set; }
        public int iLoaiDeNghi { get; set; }
        public int iTrangThai { get; set; }
        public decimal? fTongPheDuyetUSD { get; set; }
        public decimal? fTongPheDuyetVND { get; set; }
        public Guid? iDeNghiThanhToan { get; set; }
        public int iCheck { get; set; }

        public int isEnable { get; set; }

        public string sLoaiNoiDungChi { get; set; }
        public string sLoaiDeNghi { get; set; }
        public string sTrangThai { get; set; }
        public string sTongPheDuyetUSD
        {
            get { return fTongPheDuyetUSD != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(fTongPheDuyetUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2) : String.Empty; }
        }
        public string sTongPheDuyetVND {
            get { return fTongPheDuyetVND != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(fTongPheDuyetVND.Value, 0).ToString(CultureInfo.InvariantCulture), 0) : String.Empty; }
        }

    }
    public class ThanhToan_ThongTriModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<ThanhToan_ThongTriModel> thanhtoan_thongtri { get; set; }

    }

    public class ThongTriCapPhatModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<ThongTriCapPhatModel> thongtri{ get; set; }

    }

    public class ThongTriCapPhatCreateViewModel
    {
        public ThanhToan_ThongTriModelPaging thanhtoan_thongtri { get; set; }
        public ThongTriCapPhatModel thongtri_capphat { get; set; }
    }

    public class ThongTriBaoCaoModel
    {
        public int STT { get; set; }
        public string sM { get; set; }
        public string sTM { get; set; }
        public string sTTM { get; set; }
        public string sNG { get; set; }
        public string sTenNoiDungChi { get; set; }

        public decimal? fPheDuyetCapKyNay_USD { get; set; }
        public decimal? fPheDuyetCapKyNay_VND { get; set; }
    }
        
   
}
