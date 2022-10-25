using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using static Viettel.Extensions.Constants;

namespace Viettel.Models.QLVonDauTu
{
    public class VdtQtBcQuyetToanNienDoViewModel : VDT_QT_BCQuyetToanNienDo
    {
        public string sTenDonVi { get; set; }
        public string sTenNguonVon { get; set; }
        public string dNgayDeNghiStr
        {
            get
            {
                return dNgayDeNghi.HasValue ? dNgayDeNghi.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string sThanhToan
        {
            get
            {
                switch (iLoaiThanhToan)
                {
                    case 1:
                        return "Thanh toán";
                    case 2:
                        return "Tạm ứng";
                    default:
                        return string.Empty;
                }
            }
        }
        public string sLoaiThanhToan
        {
            get
            {
                switch (iLoaiThanhToan)
                {
                    case (int)LOAI_QUYET_TOAN_NIEN_DO.Type.THANH_TOAN:
                        return LOAI_QUYET_TOAN_NIEN_DO.TypeName.THANH_TOAN;
                    case (int)LOAI_QUYET_TOAN_NIEN_DO.Type.TAM_UNG:
                        return LOAI_QUYET_TOAN_NIEN_DO.TypeName.TAM_UNG;
                    default:
                        return string.Empty;
                }
            }
        }
    }
    
    public class VdtQtBcQuyetToanNienDoPrintDataExportModel
    {
        public int INamKeHoach { get; set; }
        public string sTenDonViQuanLy { get; set; }
        public Guid iID_DonViQuanLyID { get; set; }
        public int IIdNguonVonId { get; set; }        
        public string sTenNguonVon { get; set; }
        public int ILoaiThanhToan { get; set; }        
        public string SLoaiThanhToan { get; set; }        
        public string sDonViTinh { get; set; }
        public string sValueDonViTinh { get; set; }
        public string txt_TieuDe1 { get; set; }
        public string txt_TieuDe2 { get; set; }
        public string txt_TieuDe3 { get; set; }
    }
    public partial class VdtQtBcQuyetToanNienDoReturnData
    {
        public virtual VDT_QT_BCQuyetToanNienDo VDT_QT_BCQuyetToanNienDoData { get; set; }
        public virtual bool IsReturn { get; set; }
        public virtual string errorMess { get; set; }


    }
}
