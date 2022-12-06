using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQuanLyDuAnInfoModel
    {
        public VDTDuAnInfoModel dataDuAn { get; set; }
        public VDT_DA_ChuTruongDauTu dataCTDT { get; set; }
        public VDTDuAnThongTinPheDuyetModel dataQDDT { get; set; }
        public VDTDuAnThongTinDuToanModel dataDuToan { get; set; }
        public VDT_QT_QuyetToan dataQuyetToan { get; set; }
        public VDT_DM_PhanCapDuAn dataPhanCapDuAn { get; set; }
        public IEnumerable<VDT_DA_ChuTruongDauTu> listDSPDCTDT { get; set; }
        public IEnumerable<VDT_DA_DuToan> listDSTKTC { get; set; }
        public IEnumerable<VDTDuAnListCTDTChiPhiModel> listCTDTChiPhi { get; set; }
        public IEnumerable<VDTDuAnListCTDTNguonVonModel> listCTDTNguonVon { get; set; }
        public IEnumerable<VDTDuAnListQDDTChiPhiModel> listQDDTChiPhi { get; set; }
        public IEnumerable<VDTDuAnListQDDTNguonVonModel> listQDDTNguonVon { get; set; }
        public IEnumerable<VDTDuAnListQDDTHangMucModel> listQDDTHangMuc { get; set; }
        public IEnumerable<VDTDuAnListDuToanChiPhiModel> listDuToanChiPhi { get; set; }
        public IEnumerable<VDTDuAnListDuToanNguonVonModel> listDuToanNguonVon { get; set; }
        public IEnumerable<VDT_DA_DuToan> listDuToanDieuChinh { get; set; }
        public IEnumerable<VDTDuAnListNguonVonTTDuAnModel> listNguonVonDuAn { get; set; }
        public IEnumerable<VDT_DA_DuAn_HangMucModel> listDuAnHangMuc { get; set; }

        public VDTQLPheDuyetQuyetToanModel dataPDQT { get; set; }
    }
}