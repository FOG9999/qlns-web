using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class NHDAGoiThauViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_DA_GoiThauModel> Items { get; set; }
    }

    public class NH_DA_GoiThauModel : NH_DA_GoiThau
    {
        public string sTenDonVi { get; set; }
        public string sTenDuAn { get; set; }
        public string sTenChuongTrinh { get; set; }
        public string sTenLoaiHopDong { get; set; }
        public string sTenNhaThau { get; set; }
        public string sTenHinhThuc { get; set; }
        public string sTenPhuongThuc { get; set; }
        public string sTenTiGia { get; set; }
        public string sMaTienTe { get; set; }
        public string sSoLanDieuChinh { get; set; }
        public string sDieuChinhTu { get; set; }
        public string sLoai
        {
            get
            {
                switch (iPhanLoai)
                {
                    case 1:
                        return "Dự án, Trong nước";
                    case 2:
                        return "Mua sắm, Trong nước";
                    case 3:
                        return "Dự án, Ngoại thương";
                    case 4:
                        return "Mua sắm, Ngoại thương";
                    default:
                        return string.Empty;
                }
            }
        }
        public string sLabelNhaThau
        {
            get
            {
                switch (iPhanLoai)
                {
                    case 1:
                    case 2:
                        return "Nhà thầu";
                    case 3:
                    case 4:
                        return "Đơn vị ủy thác";
                    default:
                        return string.Empty;
                }
            }
        }
        public string[] sLabelByLoai
        {
            get
            {
                switch (iPhanLoai)
                {
                    case 1:
                    case 2:
                        return new string[] { "Kế hoạch lựa chọn nhà thầu", "Kết quả lựa chọn nhà thầu" };
                    case 3:
                    case 4:
                        return new string[] { "Phương án nhập khẩu", "Phê duyệt kết quả đàm phán" };
                    default:
                        return new string[] { "", "" };
                }
            }
        }
        public string[] sValueByLoai
        {
            get
            {
                switch (iPhanLoai)
                {
                    case 1:
                    case 2:
                        return new string[] { sSoKeHoachLCNT, dNgayKeHoachLCNTStr, sSoKetQuaLCNT, dNgayKetQuaLCNTStr };
                    case 3:
                    case 4:
                        return new string[] { sSoPANK, dNgayPANKStr, sSoKetQuaDamPhan, dNgayKetQuaDamPhanStr };
                    default:
                        return new string[] { "", "", "", "" };
                }
            }
        }
        public string dNgayTaoStr
        {
            get
            {
                return dNgayTao.HasValue ? dNgayTao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string dNgayKeHoachLCNTStr
        {
            get
            {
                return dNgayKeHoachLCNT.HasValue ? dNgayKeHoachLCNT.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string dNgayKetQuaLCNTStr
        {
            get
            {
                return dNgayKetQuaLCNT.HasValue ? dNgayKetQuaLCNT.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string dNgayKetQuaDamPhanStr
        {
            get
            {
                return dNgayKetQuaDamPhan.HasValue ? dNgayKetQuaDamPhan.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string dNgayPANKStr
        {
            get
            {
                return dNgayPANK.HasValue ? dNgayPANK.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
    }

    public class NH_DA_GoiThau_TienTeModel
    {
        public string sGiaTriUSD { get; set; }
        public string sGiaTriVND { get; set; }
        public string sGiaTriEUR { get; set; }
        public string sGiaTriNgoaiTeKhac { get; set; }
    }

    public class NH_DA_GoiThau_LoaiModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class NH_DA_GoiThau_ImportModel
    {
        public string sSTT { get; set; }
        public string sTenGoiThau { get; set; }
        public bool IsTenGoiThauWrong { get; set; }
        public string sMaLoai { get; set; }
        public string sTenLoai { get; set; }
        public int? loai { get; set; }
        public bool IsMaLoaiWrong { get; set; }
        public string sSoQuyetDinh1 { get; set; }
        public bool IsSoQuyetDinh1Wrong { get; set; }
        public string sSoQuyetDinh2 { get; set; }
        public bool IsSoQuyetDinh2Wrong { get; set; }
        public string sNgayQuyetDinh1 { get; set; }
        public bool IsNgayQuyetDinh1Wrong { get; set; }
        public string sNgayQuyetDinh2 { get; set; }
        public bool IsNgayQuyetDinh2Wrong { get; set; }
        public string sMaLoaiHopDong { get; set; }
        public Guid? iID_LoaiHopDong { get; set; }
        public bool IsMaLoaiHopDongWrong { get; set; }
        public string sMaNhaThau { get; set; }
        public Guid? iID_NhaThau { get; set; }
        public bool IsMaNhaThauWrong { get; set; }
        public string sMaTienTe { get; set; }
        public bool IsMaTienTeWrong { get; set; }
        public string sMaHinhThucCNT { get; set; }
        public Guid? iID_HinhThucChonNhaThauID { get; set; }
        public bool IsMaHinhThucCNTWrong { get; set; }
        public string sMaPhuongThucCNT { get; set; }
        public Guid? iID_PhuongThucChonNhaThauID { get; set; }
        public bool IsMaPhuongThucCNTWrong { get; set; }
        public string sThoiGianThucHien { get; set; }
        public bool IsThoiGianThucHienWrong { get; set; }
        public string sErrorMessage { get; set; }
        public bool IsDataWrong { get; set; }
    }
}
