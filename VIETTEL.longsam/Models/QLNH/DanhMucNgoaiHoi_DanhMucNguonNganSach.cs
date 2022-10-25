using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class  DanhMucNgoaiHoi_DanhMucNguonNganSachModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhmucNgoaiHoi_NguonNganSachModel> Items { get; set; }
    }
    public class DanhmucNgoaiHoi_NguonNganSachModel : NS_NguonNganSach
    {
        public int? iID_NguonNganSach { get; set; }
        public string sTrangThai
        {
            get
            {
                switch (iTrangThai)
                {
                    case 0:
                        return "Không sử dụng";
                    case 1:
                        return "Đang sử dụng";
                    default:
                        return string.Empty;
                }
            }
        }
    }

    public partial class NH_DM_NguonNganSachReturnData
    {
        public virtual NS_NguonNganSach LoaiNguonNganSachData { get; set; }
        public virtual bool IsReturn { get; set; }
        public virtual string errorMess { get; set; }
    }
}
