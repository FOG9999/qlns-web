using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH.QuyetToan.ChuyenQuyetToan
{
    
    public class QuyetToan_ChuyenQuyetToanModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_QT_ChuyenQuyetToanData> Items { get; set; }
    }

    public partial class NH_QT_ChuyenQuyetToanData : NH_QT_ChuyenQuyetToan
    {
        public virtual string sTenDonVi { get; set; }
        public virtual string sLoaiThoiGian { get; set; }


    }
    public partial class NH_QT_ChuyenQuyetToanReturnData
    {
        public virtual NH_QT_ChuyenQuyetToan ChuyenQuyetToanData { get; set; }
        public virtual bool IsReturn { get; set; }
        public virtual string errorMess { get; set; }
    }
    public partial class NH_QT_ChuyenQuyetToan_ChiTietView
    {
        public virtual NH_QT_ChuyenQuyetToanData ChuyenQuyetToanDetail { get; set; }
        public virtual NH_DA_HopDong HopDongDetail { get; set; }
        public virtual List<NH_QT_ChuyenQuyetToanData> ListDetailChuyenQuyetToan { get; set; }


    }
}
