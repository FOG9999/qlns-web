using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class DanhmucNgoaiHoi_TiGiaModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhmucNgoaiHoi_TiGiaModel> Items { get; set; }
    }

    public class DanhmucNgoaiHoi_TiGiaModel : NH_DM_TiGia
    {
        public string dNgayLap
        {
            get
            {
                return dThangLapTiGia.HasValue ? dThangLapTiGia.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string dNgayTaoStr
        {
            get
            {
                return dNgayTao.HasValue ? dNgayTao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
    }

    public class NHDMTiGiaChiTietParam
    {
        public Guid ID { get; set; }
        public Guid? iID_TiGiaID { get; set; }
        public Guid? iID_TienTeID { get; set; }
        public string sMaTienTeQuyDoi { get; set; }
        public string fTiGia { get; set; }
    }

    public class NH_DM_TiGiaChiTiet_TableModel
    {
        public Guid IdTiGiaChiTiet { get; set; }
        public Guid? IdTienTe { get; set; }
        public string sMaTienTeGoc { get; set; }
        public string sFTiGia { get; set; }
        public string sMaTienTeQuyDoi { get; set; }
    }
}
