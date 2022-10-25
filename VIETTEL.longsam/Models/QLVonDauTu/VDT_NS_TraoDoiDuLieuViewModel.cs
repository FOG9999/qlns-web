using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_NS_TraoDoiDuLieuViewModel : VDT_NS_Traodoidulieu
    {
        public virtual string sDonViQuanLy { get; set; }
        public virtual string sNguonVon { get; set; }
        public string sTenLoaiTraoDoi
        {
            get
            {
                switch (iLoaiTraoDoi)
                {
                    case 1:
                        return Constants.LoaiTraoDoi.TypeName.DU_TOAN;
                    case 2:
                        return Constants.LoaiTraoDoi.TypeName.QUYET_TOAN;
                    default:
                        return Constants.LoaiTraoDoi.TypeName.THONG_TRI;
                }
            }
        }

        public string sTenTrangThai
        {
            get
            {
                switch (iTrangThai)
                {
                    case 1:
                        return Constants.TrangThai.TypeName.TAO_MOI;
                    case 2:
                        return Constants.TrangThai.TypeName.DA_CHUYEN;
                    case 3:
                        return Constants.TrangThai.TypeName.DANG_XU_LY;
                    default:
                        return Constants.TrangThai.TypeName.HOAN_THANH;
                }
            }
        }

        public string sTenQuy
        {
            get
            {
                switch (iThoiGian)
                {
                    case 1:
                        return Constants.LoaiQuy.TypeName.QUY_1;
                    case 2:
                        return Constants.LoaiQuy.TypeName.QUY_2;
                    case 3:
                        return Constants.LoaiQuy.TypeName.QUY_3;
                    default:
                        return Constants.LoaiQuy.TypeName.QUY_1;
                }
            }
        }


        public string sTenThongTri
        {
            get
            {
                switch (iTrangThai)
                {
                    case 1:
                        return Constants.LoaiThongTri.TypeName.TAM_UNG;
                    case 2:
                        return Constants.LoaiThongTri.TypeName.THANH_TOAN;
                    case 3:
                        return Constants.LoaiThongTri.TypeName.CAP_HOP_THUC;
                    default:
                        return Constants.LoaiThongTri.TypeName.CAP_KINH_DOANH;
                }
            }
        }

        public string sTenLoaiDuToan
        {
            get
            {
                switch (iTrangThai)
                {
                    case 1:
                        return Constants.LoaiDuToan.TypeName.DAU_NAM;
                    case 2:
                        return Constants.LoaiDuToan.TypeName.BO_SUNG;
                    case 3:
                        return Constants.LoaiDuToan.TypeName.NAM_TRUOC_CHUYEN_SANG;
                    default:
                        return Constants.LoaiDuToan.TypeName.DAU_NAM;
                }
            }
        }


    }
    public class VDT_NS_TraoDoiDuLieuChiTietViewModel : VDT_NS_TraoDoiDuLieu_ChiTiet
    {
        public virtual Guid iID_MaMuclucNganSach { get; set; }
        public virtual bool bLaHangCha { get; set; }
        public virtual string sXauNoiMa { get; set; }
        public virtual Guid? iID_MaMuclucNganSach_Cha { get; set; }
        public virtual string sMota { get; set; }

    }
    public class VDT_NS_TraoDoiDuLieuPagingInfo
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_NS_TraoDoiDuLieuViewModel> lstData { get; set; }
    }
}
