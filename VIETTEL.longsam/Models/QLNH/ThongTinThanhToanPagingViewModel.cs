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
    public class ThongTinThanhToanPagingViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<ThongTinThanhToanModel> Items { get; set; }
    }
    public class ThongTinThanhToanModel : NH_TT_ThanhToan
    {
        public string sTen { get; set; }
        public string sTenCDT { get; set; }
        public string sTenNhiemVuChi { get; set; }
        public string sTenNhaThau { get; set; }
        public double? fTongCTDeNghiCapKyNay_USD { get; set; }
        public double? fTongCTDeNghiCapKyNay_VND { get; set; }
        public double? fTongCtPheDuyetCapKyNay_USD { get; set; }
        public double? fTongCTPheDuyetCapKyNay_VND { get; set; }
        public string sTenDuAn { get; set; }
        public string sTenHopDong { get; set; }
        public string sTiGiaDeNghi { get; set; }
        public string sTiGiaPheDuyet { get; set; }
        public string sTongCTDeNghiCapKyNay_USD {
            get {
                  return chuyendoidulieu(fTongCTDeNghiCapKyNay_USD, 2);
                }
        }
        public string sTongCTDeNghiCapKyNay_VND {
            get { 
              
                return chuyendoidulieu(fTongCTDeNghiCapKyNay_VND, 0);
            }
        }
        public string sTongCtPheDuyetCapKyNay_USD {
            get { 
               
                return chuyendoidulieu(fTongCtPheDuyetCapKyNay_USD, 2);
            }
        }
        public string sTongCTPheDuyetCapKyNay_VND {
            get { 
               
                return chuyendoidulieu(fTongCTPheDuyetCapKyNay_VND, 0);
            }
        }
        public double? fDuToanPheDuyet_USD { get; set; }
        public double? fDuToanPheDuyet_VND { get; set; }
        public double? fHopDongPheDuyet_USD { get; set; }
        public double? fHopDongPheDuyet_VND { get; set; }

        public string sHopDongPheDuyet_USD {
            get { 
                return chuyendoidulieu(fHopDongPheDuyet_USD, 2);
            }
        }
        public string sHopDongPheDuyet_VND {
            get { 
                return chuyendoidulieu(fHopDongPheDuyet_VND, 0);
            }
        }
        public string sDuToanPheDuyet_USD {
            get { 
                return chuyendoidulieu(fDuToanPheDuyet_USD, 2);
            }
        }
        public string sDuToanPheDuyet_VND {
            get { 

                return chuyendoidulieu(fDuToanPheDuyet_VND, 0);
            }
        }
        public string sLuyKe_USD
        {
            get { 
                return chuyendoidulieu(fLuyKeUSD, 2);
            }
        }
        public string sLuyKe_VND
        {
            get { return chuyendoidulieu(fLuyKeVND, 0); }
             
        }

        public string _sChuyenKhoan_BangSo
        {
            get {
                    return chuyendoidulieu(fChuyenKhoan_BangSo, 0); 
               }
        }
        
        public string _sChuyenKhoan_BangSo_USD
        {
            get
            {
                return chuyendoidulieu(fChuyenKhoan_BangSo_USD, 2);
            }
        }

        public string _sChuyenKhoan_BangSo_VND
        {
            get
            {
                return chuyendoidulieu(fChuyenKhoan_BangSo_VND, 0);
            }
        }

        public string chuyendoidulieu(double? giatri, int sosaudauphay)
        {
            return giatri != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(giatri.Value, sosaudauphay).ToString(CultureInfo.InvariantCulture), sosaudauphay) : String.Empty;
        }
    }

    public class ThongTinThanhToanSearchModel
    {
        public PagingInfo _paging { get; set; }
        public Guid? iID_DonVi { get; set; }
        public string sSoDeNghi { get; set; }
        public DateTime? dNgayDeNghi { get; set; }
        public int? iLoaiNoiDungChi { get; set; }
        public int? iLoaiDeNghi { get; set; }
        public Guid? iID_ChuDauTuID { get; set; }
        public Guid? iID_KHCTBQP_NhiemVuChiID { get; set; }
        public int? iQuyKeHoach { get; set; }
        public int? iNamKeHoach { get; set; }
        public int? iNamNganSach { get; set; }
        public int? iCoQuanThanhToan { get; set; }
        public Guid? iID_NhaThauID { get; set; }
        public int? iTrangThai { get; set; }

    }

    public class ThongTinThanhToanDetaiModel
    {
        public ThongTinThanhToanModel thongtinthanhtoan { get; set; }
        public IEnumerable<ThanhToanChiTietViewModel> thanhtoan_chitiet { get; set; }
    }

    public class ThanhToanChiTietViewModel : NH_TT_ThanhToan_ChiTiet
    {
        public string sMucLucNganSach { get; set; }
        public int STT { get; set; }
        public string sDeNghiCapKyNay_USD { get; set; }
        public string sDeNghiCapKyNay_VND { get; set; }
        public string sPheDuyetCapKyNay_USD { get; set; }
        public string sPheDuyetCapKyNay_VND { get; set; }

        public string _sDeNghiCapKyNay_USD
        {
            get { 
                    return chuyendoidulieu(fDeNghiCapKyNay_USD, 2);
            }
        }
        public string _sDeNghiCapKyNay_VND
        {
            get { 
                return chuyendoidulieu(fDeNghiCapKyNay_VND, 0);
            }
        }
        public string _sPheDuyetCapKyNay_USD
        {
            get { 
                return chuyendoidulieu(fPheDuyetCapKyNay_USD, 2);
            }
        }
        public string _sPheDuyetCapKyNay_VND
        {
            get { 
                return chuyendoidulieu(fPheDuyetCapKyNay_VND, 0);
            }
        }
        public string chuyendoidulieu(double? giatri, int sosaudauphay)
        {
            return giatri != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(giatri.Value, sosaudauphay).ToString(CultureInfo.InvariantCulture), sosaudauphay) : String.Empty;
        }

    }

    public class ThanhToanBaoCaoModel
    {
        public int STT { get; set; }
        public decimal? TongSo_VND { get; set; }
        public decimal? ChiNgoaiTen_USD { get; set; }
        public decimal? ChiNgoaiTe_VND { get; set; }
        public decimal? ChiTrongNuocVND { get; set; }
        public int IsBold { get; set; }
        public int depth { get; set; }
        public string Muc { get; set; }
        public Guid? IDParent { get; set; }
        public string NoiDung { get; set; }

        public string Position { get; set; }

        /*Model Báo cáo BM05 -BM06*/
        public string sTenNhaThau { get; set; }
        public string sTenHopDong { get; set; }
        public string sNoiDungChi { get; set; }
        public decimal? fPheDuyetCapKyNay_USD { get; set; }
        public float? fTyGia { get; set; }
        public decimal? fPheDuyetCapKyNay_VND { get; set; }

    }
    public class NS_DonViViewModel: NS_DonVi
    {
        public string sTenDonVi
        {
            get { return iID_MaDonVi + '-' + sTen; }
        }
    }

    public class DM_ChuDauTuViewModel : DM_ChuDauTu
    {
        public string sTenChuDauTu
        {
            get { return sId_CDT + " - " + sTenCDT; }
        }
    }

}