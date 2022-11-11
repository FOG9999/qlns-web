using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKHLuaChonNhaThauPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTKHLuaChonNhaThauViewModel> lstData { get; set; }
    }

    public class VDTKHLuaChonNhaThauViewModel : VDT_QDDT_KHLCNhaThau
    {
        public int iSoLanDieuChinh { get; set; }
        public string sTenDuAn { get; set; }
        public string sTenDonViQuanLy { get; set; }
        public string sTenChuDauTu { get; set; }
        public double? fTongMucDauTu { get; set; }
        public string sTongMucDauTu
        {
            get
            {
                return this.fTongMucDauTu.HasValue ? this.fTongMucDauTu.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public string sNgayQuyetDinh
        {
            get
            {
                return (dNgayQuyetDinh == null ? "" : dNgayQuyetDinh.Value.ToString("dd/MM/yyyy"));
            }
        }
    }

    public class VDTKHLCNTDetailViewModel
    {
        public Guid? iID_GoiThauID { get; set; }
        public Guid iID_ChungTu { get; set; }
        public int iID_NguonVonID { get; set; }
        public Guid? iID_ChiPhiID { get; set; }
        public Guid? iID_HangMucID { get; set; }
        public Guid? iID_ParentId { get; set; }
        public string sNoiDung { get; set; }
        public int? iThuTu { get; set; }
        public string sMaOrder { get; set; }
        public double? fGiaTriPheDuyet { get; set; }
        public double? fGiaTriGoiThau { get; set; }
        public int iProcessStatus { get; set; }
    }

    public class VDTKHLCNhaThauChungTuViewModel
    {
        public Guid id { get; set; }
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string sNgayQuyetDinh
        {
            get
            {
                return dNgayQuyetDinh.HasValue ? dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string sTenDonVi { get; set; }
        public double fGiaTriPheDuyet { get; set; }
        public string sGiaTriPheDuyet
        {
            get
            {
                return fGiaTriPheDuyet.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public class VDTDADuToanModel : VDT_DA_DuToan
    {
        public string sTenDonVi { get; set; } 
        public string sNgayQuyetDinh
        {
            get
            {
                return (dNgayQuyetDinh == null ? "" : dNgayQuyetDinh.Value.ToString("dd/MM/yyyy"));
            }
        }
    }

    public class VDTKHLuaChonNhaThauModel
    {
        public VDT_QDDT_KHLCNhaThau objKHLuaChonNhaThau { get; set; }
        public IEnumerable<VDT_DA_GoiThau> lstGoiThau { get; set; }
        public IEnumerable<VDTKHLCNTDetailViewModel> lstDetail { get; set; }
        public bool isDieuChinh { get; set; }
    }

    public class VDTDAGoiThauModel : VDT_DA_GoiThau
    {
        public string sHinhThucChonNhaThauValue { get; set; }
        public string sPhuongThucDauThauValue { get; set; }
        public string sHinhThucHopDongValue { get; set; }

    }
}
