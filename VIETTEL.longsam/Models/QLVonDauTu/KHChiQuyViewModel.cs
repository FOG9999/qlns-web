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
    public class NCNhuCauChi : VDT_NC_NhuCauChi
    {
        public string sDonViQuanLy { get; set; }
        public string sNguonVon { get; set; }
        public string sNgayDeNghi
        {
            get
            {
                return dNgayDeNghi.HasValue ? dNgayDeNghi.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
    }
    public class NcNhuCauChi_ChiTiet : VDT_NC_NhuCauChi_ChiTiet
    {
        public string sTenDuAn { get; set; }
        public double fKeHoachVonNam { get; set; }
        public string sKeHoachVonNam
        {
            get
            {
                return fKeHoachVonNam.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fThanhToanKLHTQuyTruoc { get; set; }
        public string sThanhToanKLHTQuyTruoc
        {
            get
            {
                return fThanhToanKLHTQuyTruoc.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fThanhToanTamUngQuyTruoc { get; set; }
        public string sThanhToanTamUngQuyTruoc
        {
            get
            {
                return fThanhToanTamUngQuyTruoc.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fTongQuyTruoc
        {
            get
            {
                return fThanhToanKLHTQuyTruoc + fThanhToanTamUngQuyTruoc;
            }
        }
        public string sTongQuyTruoc
        {
            get
            {
                return fTongQuyTruoc.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fThanhToanKLHTQuyNay { get; set; }
        public string sThanhToanKLHTQuyNay
        {
            get
            {
                return fThanhToanKLHTQuyNay.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fThanhToanTamUngQuyNay { get; set; }
        public string sThanhToanTamUngQuyNay
        {
            get
            {
                return fThanhToanTamUngQuyNay.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fThuHoiUng { get; set; }
        public string sThuHoiUng
        {
            get
            {
                return fThuHoiUng.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fTongQuyNay
        {
            get
            {
                return fThanhToanKLHTQuyNay + fThanhToanTamUngQuyNay - fThuHoiUng;
            }
        }
        public string sTongQuyNay
        {
            get
            {
                return fTongQuyNay.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public double fSoConGiaiNganNam
        {
            get
            {
                return fKeHoachVonNam - fTongQuyTruoc - fTongQuyNay;
            }
        }
        public string sSoConGiaiNganNam
        {
            get
            {
                return fSoConGiaiNganNam.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public string sGiaTriDeNghi
        {
            get
            {
                return (fGiaTriDeNghi ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }
    public class KinhPhiCucTaiChinhCap
    {
        public double fQuyTruocChuaGiaiNgan { get; set; }
        public double fQuyNayDuocCap { get; set; }
        public double fGiaiNganQuyNay { get; set; }
        public double fChuaGiaiNganChuyenQuySau
        {
            get
            {
                return fQuyTruocChuaGiaiNgan + fQuyNayDuocCap - fGiaiNganQuyNay;
            }
        }

        public string sQuyTruocChuaGiaiNgan
        {
            get
            {
                return fQuyTruocChuaGiaiNgan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sQuyNayDuocCap
        {
            get
            {
                return fQuyNayDuocCap.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaiNganQuyNay
        {
            get
            {
                return fGiaiNganQuyNay.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sChuaGiaiNganChuyenQuySau
        {
            get
            {
                return fChuaGiaiNganChuyenQuySau.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public class KHChiQuyPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NCNhuCauChi> lstData { get; set; }
        public string sDVTinh { get; set; }
        public long iDVTinh { get; set; }
    }
}
