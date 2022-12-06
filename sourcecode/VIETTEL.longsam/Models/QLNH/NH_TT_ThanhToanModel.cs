using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLNH
{
    public class NH_TT_ThanhToanModel
    {
        public NH_ThanhToan_ChuyenDoi TT_ThanToan {get;set;}
        public IEnumerable<NH_TT_ThanhToan_ChiTiet_ChuyenDoi> lstTTThanToan_ChiTiet { get; set; }

    }
    public class NH_TT_ThanhToan_ChiTiet_ChuyenDoi : NH_TT_ThanhToan_ChiTiet
    {
        public string sDeNghiCapKyNay_USD { get; set; }
        public string sDeNghiCapKyNay_VND { get; set; }
        public string sPheDuyetCapKyNay_USD { get; set; }
        public string sPheDuyetCapKyNay_VND { get; set; }
      
    }

    public class NH_ThanhToan_ChuyenDoi: NH_TT_ThanhToan
    {
        public string sLuyKeUSD { get; set; }
        public string sLuyKeVND { get; set; }
        public string sChuyenKhoanBangSo { get; set; }
    }
}
