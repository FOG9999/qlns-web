using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class NH_KHTongTheTTCPViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE, CurrentPage = 1 };
        public List<NH_KHTongTheTTCPModel> Items { get; set; }
    }

    public class NH_KHTongTheTTCPModel : NH_KHTongTheTTCP
    {
        public virtual string sNam { get; set; }
        public virtual string sDieuChinhTu { get; set; }
        public virtual Guid? iID_BQuanLyID { get; set; }
    }

    public class NH_KHTongTheTTCPFilter
    {
        public virtual int? GiaiDoanTu { get; set; }
        public virtual int? GiaiDoanDen { get; set; }
        public virtual string SoKeHoach { get; set; }
        public virtual DateTime? NgayBanHanh { get; set; }
    }

    public class NH_KHTongTheTTCP_NVCViewModel
    {
        public virtual Guid ID { get; set; }
        public virtual int? iGiaiDoanTu { get; set; }
        public virtual int? iGiaiDoanDen { get; set; }
        public virtual int? iNamKeHoach { get; set; }
        public virtual string sSoKeHoach { get; set; }
        public virtual DateTime? dNgayKeHoach { get; set; }
        public virtual int? iLoai { get; set; }
        public virtual List<NH_KHTongTheTTCP_NVCModel> Items { get; set; }
        public virtual bool IsEdit { get; set; }
        public virtual string State { get; set; }
    }

    public class NH_KHTongTheTTCP_NVCModel : NH_KHTongTheTTCP_NhiemVuChi
    {
        public virtual string sTenPhongBan { get; set; }
        public virtual bool bIsHasChild { get; set; }
    }

    public class NH_KHTongTheTTCP_NhiemVuChiDto : NH_KHTongTheTTCP_NhiemVuChi
    {
        public virtual string sGiaTri { get; set; }
    }

    public class LookupKHTTCP
    {
        public virtual Guid Id { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int? iGiaiDoanTu { get; set; }
        public virtual int? iGiaiDoanDen { get; set; }
        public virtual int? iNamKeHoach { get; set; }
        public virtual int iLoai { get; set; }
    }
}
