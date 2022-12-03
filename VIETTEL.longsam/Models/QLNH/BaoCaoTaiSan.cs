using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class BaoCaoTaiSanModelViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<BaoCaoTaiSanModel> Items { get; set; }
        public List<BaoCaoTaiSanModel2> Items2 { get; set; }
        public SelectList ListDonVi { get; set; }
        public SelectList ListDuAn { get; set; }
        public SelectList ListHopDong { get; set; }
        public SelectList ListDonVi2 { get; set; }
    }

    public class BaoCaoTaiSanModel : NH_QT_TaiSan
    {
        public string sDonVi { get; set; }
        public string sDuAn { get; set; }
        public string sHopDong { get; set; }

    }
    public class BaoCaoTaiSanModel2 : NH_QT_TaiSan
    {
        public string sDonVi2 { get; set; }
        public string slHH { get; set; }
        public string slVH { get; set; }
        public string slChua { get; set; }
        public string slDang { get; set; }
        public string slKhong { get; set; }
        public string slMoi { get; set; }
        public string slCu { get; set; }
        public string slHet { get; set; }
    }
    public class NS_DonViModel : NS_DonVi
    {
        public string sDonVi { get; set; }
    }
}
