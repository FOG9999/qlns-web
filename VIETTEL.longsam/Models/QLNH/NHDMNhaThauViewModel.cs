using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class NHDMNhaThauViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NHDMNhaThauModel> Items { get; set; }
    }

    public class NHDMNhaThauModel : NH_DM_NhaThau
    {
        public string dNgayCapCMNDStr
        {
            get
            {
                return dNgayCapCMND.HasValue ? dNgayCapCMND.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
    }

    public class NHDMNhaThau_Dropdown_LoaiNhaThau
    {
        public virtual int valueId { get; set; }
        public virtual string labelName { get; set; }

    }

    public class NhaThauImportModel 
    {
        public string STT { get; set; }
        public string sMaLoai { get; set; }
        public string sTenLoai { get; set; }
        public int? loai { get; set; }
        public bool IsMaLoaiWrong { get; set; }
        public string sMaNhaThau { get; set; }
        public bool IsMaNhaThauWrong { get; set; }
        public string sDonViUyThac { get; set; }
        public bool IsDonViUyThacWrong { get; set; }
        public string sDiaChi { get; set; }
        public bool IsDiaChiWrong { get; set; }
        public string sDaiDien { get; set; }
        public bool IsDaiDienWrong { get; set; }
        public string sChucVu { get; set; }
        public bool IsChucVuWrong { get; set; }
        public string sSoDienThoai { get; set; }
        public bool IsSoDienThoaiWrong { get; set; }
        public string sFax { get; set; }
        public bool IsFaxWrong { get; set; }
        public string sEmail { get; set; }
        public bool IsEmailWrong { get; set; }
        public string sWebsite { get; set; }
        public bool IsWebsiteWrong { get; set; }
        public string sSoTaiKhoan { get; set; }
        public bool IsSoTaiKhoanWrong { get; set; }
        public string sNganHang { get; set; }
        public bool IsNganHangWrong { get; set; }
        public string MaNganHang { get; set; }
        public bool IsMaNganHangWrong { get; set; }
        public string sMaSoThue { get; set; }
        public bool IsMaSoThueWrong { get; set; }
        public string sNguoiLienHe { get; set; }
        public bool IsNguoiLienHeWrong { get; set; }
        public string sDienThoaiLienHe { get; set; }
        public bool IsDienThoaiLienHeWrong { get; set; }
        public string sSoCMND{ get; set; }
        public bool IsSoCMNDWrong { get; set; }
        public string sNoiCapCMND{ get; set; }
        public bool IsNoiCapCMNDWrong { get; set; }
        public string sNgayCap { get; set; }
        public bool IsNgayCapWrong { get; set; }
        public string sErrorMessage { get; set; }
        public bool IsDataWrong { get; set; }
    }

}

