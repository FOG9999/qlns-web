using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class NH_QT_ThongTriQuyetToanViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE, CurrentPage = 1 };
        public List<NH_QT_ThongTriQuyetToanModel> Items { get; set; }
    }

    public class NH_QT_ThongTriQuyetToanModel : NH_QT_ThongTriQuyetToan
    {
        public virtual string sTenDonVi { get; set; }
        public virtual string sTenNhiemVuChi { get; set; }
        public virtual string sThongTri_USD { get; set; }
        public virtual string sThongTri_VND { get; set; }
    }

    public class NH_QT_ThongTriQuyetToanFilter
    { 
        public virtual Guid? iID_DonViID { get; set; }
        public virtual Guid? iID_KHCTBQP_ChuongTrinhID { get; set; }
        public virtual string sSoThongTri { get; set; }
        public virtual DateTime? dNgayLap { get; set; }
        public virtual int? iNamThucHien { get; set; }
        public virtual int? iLoaiThongTri { get; set; }
        public virtual int? iLoaiNoiDungChi { get; set; }
    }

    public class NH_QT_ThongTriQuyetToan_ChiTietModel : NH_QT_ThongTriQuyetToan_ChiTiet
    { 
        public virtual string sM { get; set; }
        public virtual string sTM { get; set; }
        public virtual string sTTM { get; set; }
        public virtual string sNG { get; set; }
        public virtual Guid? iID_KHCTBQP_NhiemVuChiID { get; set; }
        public virtual string sTenNhiemVuChi { get; set; }
        public virtual string sTenDuAn { get; set; }
        public virtual string sTenHopDong { get; set; }
        public virtual Guid? iID_MucLucNganSachIO { get; set; } 
        public virtual int? iLoaiNoiDungChi { get; set; }
        public virtual Guid? iID_ParentID { get; set; }
        public virtual bool? bIsTittle { get; set; } = false;
        public virtual bool? bIsData { get; set; } = false;
        public virtual Guid? iID_ThanhToanID { get; set; }
        public virtual bool? bIsNhiemVuChi { get; set; } = false;
        public virtual string sDeNghiQuyetToanNam_VND { get; set; }
        public virtual string sDeNghiQuyetToanNam_USD { get; set; }
        public virtual string sThuaNopTraNSNN_VND { get; set; }
        public virtual string sThuaNopTraNSNN_USD { get; set; }
        public virtual string sMoneyReport { get; set; }
    }

    public class NH_QT_ThongTriQuyetToanCreateDto
    {
        public virtual NH_QT_ThongTriQuyetToanModel ThongTriQuyetToan { get; set; } = new NH_QT_ThongTriQuyetToanModel();
        public virtual List<NH_QT_ThongTriQuyetToan_ChiTietModel> ThongTriQuyetToan_ChiTiet { get; set; } = new List<NH_QT_ThongTriQuyetToan_ChiTietModel>();
    }
}
