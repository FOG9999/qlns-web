using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class KeHoachChiTietTTCPViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_KHTongTheTTCP_NhiemVuChiModel> Items { get; set; }
        public NH_KHTongTheTTCP_NhiemVuChi DetailKHCT { get; set; }
        public NH_KHTongTheTTCP DetailKHTT { get; set; }
        public IEnumerable<NH_KHTongTheTTCP_NhiemVuChi_Parent> Parents { get; set; }
        public IEnumerable<NH_KHTongTheTTCP_NhiemVuChi> KHTT_NVC_List { get; set; }
        public IEnumerable<NH_KHTongTheTTCP_BQL> ListPhongBan { get; set; }
        public IEnumerable<NH_KHTongTheTTCP_SoKeHoach> ListSoKeHoach { get; set; }
    }

    public class NH_KHTongTheTTCP_NhiemVuChiModel : NH_KHTongTheTTCP_NhiemVuChi
    {

    }
    public class NH_KHTongTheTTCP_NhiemVuChi_Parent
    {
        public string sMaThuTu { get; set; }
        public Guid ID { get; set; }
        public string sTenNhiemVuChi { get; set; }
        public Guid iID_BQuanLyID { get; set; }
        public float fGiaTri { get; set; }
        public Guid iID_ParentID { get; set; }
        public Guid BQuanLyID { get; set; }
        public string BQuanLy { get; set; }
    }
    public class NH_KHTongTheTTCP_SoKeHoach
    {
        public Guid KHTTCP_ID { get; set; }
        public string KHTTCP { get; set; }
    }
    public class NH_KHTongTheTTCP_BQL
    {
        public Guid BQuanLyID { get; set; }
        public string BQuanLy { get; set; }
    }
}
