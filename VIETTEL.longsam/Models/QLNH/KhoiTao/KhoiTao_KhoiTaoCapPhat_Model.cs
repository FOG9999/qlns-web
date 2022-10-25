using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH.KhoiTao
{
    public class KhoiTao_KhoiTaoCapPhatModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_KT_KhoiTaoCapPhatData> Items { get; set; }
    }
    public partial class NH_KT_KhoiTaoCapPhatData : NH_KT_KhoiTaoCapPhat
    {
        public virtual string sTenDonVi { get; set; }
        public virtual string sTenTiGia { get; set; }
        public virtual string sMaTienTeGoc { get; set; }

        public virtual double? fDonViTiGia { get; set; }

        public string dNgayKhoiTaoStr
        {
            get
            {
                return dNgayKhoiTao.HasValue ? dNgayKhoiTao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

    }
    public class Dropdown_SelectValue
    {
        public int? Value { get; set; }
        public string Label { get; set; }
    }

    public partial class NH_KT_KhoiTaoCapPhatReturnData
    {
        public virtual NH_KT_KhoiTaoCapPhat KhoiTaoCapPhatData { get; set; }
        public virtual bool IsReturn { get; set; }
        public virtual string errorMess { get; set; }


    }

    public partial class NH_QT_KhoiTaoCapPhat_ChiTietView
    {
        public virtual NH_KT_KhoiTaoCapPhatData KhoiTaoCapPhatDetail { get; set; }
        public virtual List<NH_KT_KhoiTaoCapPhat_ChiTietData> ListDetailKhoiTaoCapPhat{ get; set; }


    }

    public partial class NH_KT_KhoiTaoCapPhat_ChiTietData : NH_KT_KhoiTaoCapPhat_ChiTiet
    {
       
        public virtual string sTenHopDong { get; set; }
        public virtual string sTenDuAn { get; set; }


    }

    public partial class NH_KT_KhoiTaoCapPhat_ChiTietReturnData
    {
        public virtual NH_KT_KhoiTaoCapPhat_ChiTiet KhoiTaoCapPhatChiTietData { get; set; }
        public virtual bool IsReturn { get; set; }

    }


}
