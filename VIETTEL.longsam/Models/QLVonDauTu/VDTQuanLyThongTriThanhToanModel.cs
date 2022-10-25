using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQuanLyThongTriThanhToanModel
    {
    }

    public class VDTThongTriViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTThongTriModel> Items { get; set; }
    }

    public class VDTThongTriModel : VDT_ThongTri
    {
        public string sMaDonVi { get; set; }
        public string sTenDonVi { get; set; }
        public string sTenNguonNganSach { get; set; }
        public DateTime? dNgayLapGanNhat { get; set; }
        public int iSTT { get; set; }
        public string sNgayThongTri
        {
            get
            {
                return this.dNgayThongTri.HasValue ? this.dNgayThongTri.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string sLoaiThongTri
        {
            get
            {
                switch (iLoaiThongTri)
                {
                    case (int)Constants.LoaiThongTriThanhToan.Type.CAP_THANH_TOAN:
                        return Constants.LoaiThongTriThanhToan.TypeName.CAP_THANH_TOAN;

                    case (int)Constants.LoaiThongTriThanhToan.Type.CAP_TAM_UNG:
                        return Constants.LoaiThongTriThanhToan.TypeName.CAP_TAM_UNG;

                    case (int)Constants.LoaiThongTriThanhToan.Type.CAP_KINH_PHI:
                        return Constants.LoaiThongTriThanhToan.TypeName.CAP_KINH_PHI;

                    case (int)Constants.LoaiThongTriThanhToan.Type.CAP_HOP_THUC:
                        return Constants.LoaiThongTriThanhToan.TypeName.CAP_HOP_THUC;
                    default: return "Chưa biết";
                }
            }
        }
        public double? fSoTien { get; set; }
        public string sSoTien
        {
            get
            {
                return this.fSoTien != 0 ? this.fSoTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : 0.ToString();
            }
        }
    }

        public class VDTTTDeNghiThanhToanChiTiet : VDT_TT_DeNghiThanhToan_ChiTiet
        {
            public string sLevelTab { get; set; }
            public bool bHasChild { get; set; }
            public Guid? iID_Parent { get; set; }
            public string sM { get; set; }
            public string sTM { get; set; }
            public string sTTM { get; set; }
            public string sNG { get; set; }
            public string sNoiDung { get; set; }
            public Guid? iID_LoaiCongTrinhID { get; set; }
            public string sTenLoaiCongTrinh { get; set; }
            public double? fGiaTriThuHoiUngNgoaiChiTieu { get; set; }
        }

        public class VDTThongTriChiTiet : VDT_ThongTri_ChiTiet
        {
            public string sLevelTab { get; set; }
            public bool bHasChild { get; set; }
            public Guid? iID_Parent { get; set; }
            public string sM { get; set; }
            public string sTM { get; set; }
            public string sTTM { get; set; }
            public string sNG { get; set; }
            public string sNoiDung { get; set; }
            public string sTenLoaiCongTrinh { get; set; }
        }
    }
