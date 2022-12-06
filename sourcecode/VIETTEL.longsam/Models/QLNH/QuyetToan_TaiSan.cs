using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class QuyetToan_TaiSanModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_QT_TaiSanViewModel> Items { get; set; }
    }
    public class QuyetToan_ChungTuModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_QT_ChungTuTaiSan> Items { get; set; }

    }
    public class QuyetToan_ChungTuTaiSanModel
    {
        public virtual NH_QT_ChungTuTaiSan ChungTuModel { get; set; }
        public virtual QuyetToan_TaiSanModelPaging ListTaiSan { get; set; }
    }
    public class NH_QT_TaiSanViewModel : NH_QT_TaiSan
    {
        public virtual string BDonVi { get; set; }
        public virtual string BDuAn { get; set; }
        public virtual string bHopDong { get; set; }
        public string dNgayBatDauSuDungStr
        {
            get
            {
                return dNgayBatDauSuDung.HasValue ? dNgayBatDauSuDung.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
    }
    public class Dropdown_ChungTuTaiSan
    {
        public virtual int valueId { get; set; }
        public virtual string labelName { get; set; }

    }

    public class GetTaiSan : NH_DM_LoaiTaiSan
    {
        public int STT {get; set;}
    }
}
