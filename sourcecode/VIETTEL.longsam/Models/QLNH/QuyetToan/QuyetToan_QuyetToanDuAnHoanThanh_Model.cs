using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH.QuyetToan
{
    public class QuyetToan_QuyetToanDuAnModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_QT_QuyetToanDAHTData> Items { get; set; }
    }
    public partial class NH_QT_QuyetToanDAHTData : NH_QT_QuyetToanDAHT
    {
        public virtual string sTenDonVi { get; set; }
        public virtual string sTenTiGia { get; set; }
        public virtual double? fDonViTiGia { get; set; }

        public virtual string sMaTienTeGoc { get; set; }
        public string dNgayDeNghiStr
        {
            get
            {
                return dNgayDeNghi.HasValue ? dNgayDeNghi.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string sTrangThai
        {
            get
            {
                if (iTrangThai == 1)
                {
                    return "Chưa được duyệt";
                }
                else
                {
                    return "Đã được duyệt";
                }
            }
        }

    }
    public partial class NH_QT_QuyetToanDAHT_ChiTietView
    {
        public virtual NH_QT_QuyetToanDAHTData QuyetToanDAHTDetail { get; set; }
        public virtual List<NH_QT_QuyetToanDAHT_ChiTietData> ListDetailQuyetToanDAHT { get; set; }
    }
    public partial class NH_QT_QuyetToanDuAnByDonVi
    {
        public virtual NH_QT_QuyetToanDAHTData quyetToanDuAn { get; set; }
        public virtual NS_DonVi donVi { get; set; }

    }
    public partial class NH_QT_QuyetToanDuAnGiaiDoan
    {
        public virtual string giaiDoan { get; set; }
        public virtual int iNamBaoCaoTu { get; set; }
        public virtual int iNamBaoCaoDen { get; set; }

    }

    public partial class NH_QT_QuyetToanDuAnDataGiaiDoan
    {
        public virtual double? value { get; set; }
        public virtual double? valueUSD { get; set; }
        public virtual double? valueVND { get; set; }
        public virtual int iNamBaoCaoTu { get; set; }
        public virtual int iNamBaoCaoDen { get; set; }

    }
    public partial class NH_QT_QuyetToanDAHT_ChiTietData : NH_QT_QuyetToanDAHT_ChiTiet
    {
        public virtual string sTenHopDong { get; set; }
        public virtual string isTitle { get; set; }
        public virtual string sTenDuAn { get; set; }
        public virtual string sTenNhiemVuChi { get; set; }
        public virtual string sTenNoiDungChi { get; set; }
        public virtual int? iLoaiNoiDungChi { get; set; }
        public virtual string STT { get; set; }
        public virtual bool? bIsTittle { get; set; } = false;
        public virtual float? fHopDong_USD_DuAn { get; set; }
        public virtual float? fHopDong_VND_DuAn { get; set; }
        public virtual float? fHopDong_USD_HopDong { get; set; }
        public virtual float? fHopDong_VND_HopDong { get; set; }
        public virtual Guid? iID_DonVi { get; set; }
        public virtual string sTenDonVi { get; set; }
        public virtual double? sumTTCP { get; set; }
        public virtual double? sumKPDCUSD { get; set; }
        public virtual double? sumKPDCVND { get; set; }
        public virtual double? sumQTDDUSD { get; set; }
        public virtual double? sumQTDDVND { get; set; }
        public int sLevel { get; set; }

        public int iGiaiDoanDen { get; set; }
        public int iGiaiDoanTu { get; set; }
        public List<NH_QT_QuyetToanDuAnDataGiaiDoan> listDataTTCP { get; set; }
        public List<NH_QT_QuyetToanDuAnDataGiaiDoan> listDataKPDC { get; set; }
        public List<NH_QT_QuyetToanDuAnDataGiaiDoan> listDataQTDD { get; set; }

    }
    public class Dropdown_QuyetToanDAHT
    {
        public int? Value { get; set; }
        public string Label { get; set; }
    }
    public partial class NH_QT_QuyetToanDuAnHTReturnData
    {
        public virtual NH_QT_QuyetToanDAHT QuyetToanDuAnHTData { get; set; }
        public virtual bool IsReturn { get; set; }

    }
    public partial class NH_QT_QuyetToanDAHTChiTietReturnData
    {
        public virtual NH_QT_QuyetToanDAHT_ChiTiet QuyetToanDuAnChiTietData { get; set; }
        public virtual bool IsReturn { get; set; }

    }
    public class BaoCaoKetLuanQuyetToan_ModelPaging
    {
        public IEnumerable<NH_QT_QuyetToanDAHT_ChiTietData> Items { get; set; }
    }
}