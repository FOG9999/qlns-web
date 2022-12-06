using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.BaoHiemXaHoi
{
    public class QLBenhNhanBHXHPagingViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<QLBenhNhanBHXHViewModel> Items { get; set; }
        public string htmlDonViNS { get; set; }
        public string htmlDonViBHXH { get; set; }
        public string sMaDonViNS { get; set; }
        public string sMaDonViBHXH { get; set; }
        public string iThangFrom { get; set; }
        public string iThangTo { get; set; }
        public string iSoNgayDieuTri { get; set; }
        public string sHoTen { get; set; }
        public string sMaThe { get; set; }

    }
    public class QLBenhNhanBHXHViewModel : BHXH_BenhNhan
    {
        public string sTenDonViMapping { get; set; }
        public string sSoTTBN { get; set; }
        public string sSoNgayDieuTriBN { get; set; }
        public string sNgaySinhBN
        {
            get
            {
                if (string.IsNullOrEmpty(sNgaySinh) || sNgaySinh.Trim().Length != 8)
                    return string.Empty;
                else
                    return sNgaySinh.Substring(6, 2) + "/" + sNgaySinh.Substring(4, 2) + "/" + sNgaySinh.Substring(0, 4);
            }
        }

        public string sNgayVaoVienBN
        {
            get
            {
                if (string.IsNullOrEmpty(sNgayVaoVien) || sNgayVaoVien.Trim().Length != 8)
                    return string.Empty;
                else
                    return sNgayVaoVien.Substring(6, 2) + "/" + sNgayVaoVien.Substring(4, 2) + "/" + sNgayVaoVien.Substring(0, 4);
            }
        }

        public string sNgayRaVienBN
        {
            get
            {
                if (string.IsNullOrEmpty(sNgayRaVien) || sNgayRaVien.Trim().Length != 8)
                    return string.Empty;
                else
                    return sNgayRaVien.Substring(6, 2) + "/" + sNgayRaVien.Substring(4, 2) + "/" + sNgayRaVien.Substring(0, 4);
            }
        }
        public bool bIsFalse;
        public string sTenDonViBHXH { get; set; }
        public Dictionary<string, bool> lstValidate { get; set; }
        public string sNoiDungLoi { get; set; }
        public bool bIsTrungBenhNhan { get; set; }
    }

    public class TongHopBenhNhanDonVi
    {
        public Guid iID_BHXH_DonViID { get; set; }
        public string iID_MaDonVi { get; set; }
        public string sTenDonVi { get; set; }
        public Guid iID_ParentId { get; set; }
        public int iThang { get; set; }
        public int iLuotDieuTri { get; set; }
        public int iSoNgayDieuTri { get; set; }
        public List<int> listLuotDieuTri { get; set; }
        public List<int> listSoNgayDieuTri { get; set; }
    }

    public class BHXHDonViTree: BHXH_DonVi
    {
        public Guid? parent { get; set; }
        public int depth { get; set; }
        public string location { get; set; }
        public int iThang { get; set; }
        public int iLuotDieuTriT1 { get; set; }
        public int iSoNgayDieuTriT1 { get; set; }
        public int iLuotDieuTriT2 { get; set; }
        public int iSoNgayDieuTriT2 { get; set; }
        public int iLuotDieuTriT3 { get; set; }
        public int iSoNgayDieuTriT3 { get; set; }
        public int iLuotDieuTriT4 { get; set; }
        public int iSoNgayDieuTriT4 { get; set; }
        public int iLuotDieuTriT5 { get; set; }
        public int iSoNgayDieuTriT5 { get; set; }
        public int iLuotDieuTriT6 { get; set; }
        public int iSoNgayDieuTriT6 { get; set; }
        public int iLuotDieuTriT7 { get; set; }
        public int iSoNgayDieuTriT7 { get; set; }
        public int iLuotDieuTriT8 { get; set; }
        public int iSoNgayDieuTriT8 { get; set; }
        public int iLuotDieuTriT9 { get; set; }
        public int iSoNgayDieuTriT9 { get; set; }
        public int iLuotDieuTriT10 { get; set; }
        public int iSoNgayDieuTriT10 { get; set; }
        public int iLuotDieuTriT11 { get; set; }
        public int iSoNgayDieuTriT11 { get; set; }
        public int iLuotDieuTriT12 { get; set; }
        public int iSoNgayDieuTriT12 { get; set; }

        public string sSTT { get; set; }
        public int iTongSoNguoiDieuTri { get; set; }
        public int iTongSoNgayDieuTri { get; set; }

    }
}
