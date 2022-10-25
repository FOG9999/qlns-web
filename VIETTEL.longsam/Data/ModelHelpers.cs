using DapperExtensions.Mapper;
using Viettel.Domain.DomainModel;

namespace Viettel.Data
{
    public class ModelHelpers
    {
    }

    public class QTA_ChungTuChiTietMapper : ClassMapper<QTA_ChungTuChiTiet>
    {
        public QTA_ChungTuChiTietMapper()
        {
            Map(x => x.iID_MaChungTuChiTiet).Key(KeyType.Identity);
            AutoMap();
        }
    }


    public class NS_MucLucNganSachMapper : ClassMapper<NS_MucLucNganSach>
    {
        public NS_MucLucNganSachMapper()
        {
            Map(x => x.iID_MaMucLucNganSach).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class NS_DanhMuc_BaoCao_ChuKyMapper : ClassMapper<NS_DanhMuc_BaoCao_ChuKy>
    {
        public NS_DanhMuc_BaoCao_ChuKyMapper()
        {
            Map(x => x.iID_MaBaoCao_ChuKy).Key(KeyType.Identity);
            AutoMap();
        }
    }

    public class NS_DanhMucChuKyMapper : ClassMapper<NS_DanhMucChuKy>
    {
        public NS_DanhMucChuKyMapper()
        {
            Map(x => x.iID_MaChuKy).Key(KeyType.Identity);
            AutoMap();
        }
    }

    public class CP_CapPhatChiTietMapper : ClassMapper<CP_CapPhatChiTiet>
    {
        public CP_CapPhatChiTietMapper()
        {
            Map(x => x.iID_MaCapPhatChiTiet).Key(KeyType.Identity);
            AutoMap();
        }
    }

    public class CP_CapPhatMapper : ClassMapper<CP_CapPhat>
    {
        public CP_CapPhatMapper()
        {
            Map(x => x.iID_MaCapPhat).Key(KeyType.Identity);
            AutoMap();
        }
    }

    public class NNS_DuToanChiTietMapper : ClassMapper<NNS_DuToanChiTiet>
    {
        public NNS_DuToanChiTietMapper()
        {
            Map(x => x.iID_DuToanChiTiet).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class VDT_DA_ChuTruongDauTuMapper : ClassMapper<VDT_DA_ChuTruongDauTu>
    {
        public VDT_DA_ChuTruongDauTuMapper()
        {
            Map(x => x.iID_ChuTruongDauTuID).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class VDT_DA_ChuTruongDauTu_ChiPhiMapper : ClassMapper<VDT_DA_ChuTruongDauTu_ChiPhi>
    {
        public VDT_DA_ChuTruongDauTu_ChiPhiMapper()
        {
            Map(x => x.iID_ChuTruongDauTu_ChiPhiID).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class VDT_DA_ChuTruongDauTu_NguonVonMapper : ClassMapper<VDT_DA_ChuTruongDauTu_NguonVon>
    {
        public VDT_DA_ChuTruongDauTu_NguonVonMapper()
        {
            Map(x => x.iID_ChuTruongDauTu_NguonVonID).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class NNS_DuToanMapper : ClassMapper<NNS_DuToan>
    {
        public NNS_DuToanMapper()
        {
            Map(x => x.iID_DuToan).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class NNS_DotNhanMapper : ClassMapper<NNS_DotNhan>
    {
        public NNS_DotNhanMapper()
        {
            Map(x => x.iID_DotNhan).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class NNS_DotNhanChiTietMapper : ClassMapper<NNS_DotNhanChiTiet>
    {
        public NNS_DotNhanChiTietMapper()
        {
            Map(x => x.iID_DotNhanChiTiet).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class NNS_DotNhanChiTiet_NDChiMapper : ClassMapper<NNS_DotNhanChiTiet_NDChi>
    {
        public NNS_DotNhanChiTiet_NDChiMapper()
        {
            Map(x => x.iID_DotNhanChiTiet_NDChi).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class NNS_NDChi_MLNhuCauMapper : ClassMapper<NNS_NDChi_MLNhuCau>
    {
        public NNS_NDChi_MLNhuCauMapper()
        {
            Map(x => x.Id).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class DM_NguonMapper : ClassMapper<DM_Nguon>
    {
        public DM_NguonMapper()
        {
            Map(x => x.iID_Nguon).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class DM_NoiDungChiMapper : ClassMapper<DM_NoiDungChi>
    {
        public DM_NoiDungChiMapper()
        {
            Map(x => x.iID_NoiDungChi).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class VDT_QT_QuyetToan_NguonVon_ChenhLechMapper : ClassMapper<VDT_QT_QuyetToan_NguonVon_ChenhLech>
    {
        public VDT_QT_QuyetToan_NguonVon_ChenhLechMapper()
        {
            Map(x => x.iID_QuyetToan_NguonVonCL).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class NNS_DonViMapper : ClassMapper<NS_DonVi>
    {
        public NNS_DonViMapper()
        {
            Map(x => x.iID_Ma).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class VDT_DA_DuAn_HangMucMapper : ClassMapper<VDT_DA_DuAn_HangMuc>
    {
        public VDT_DA_DuAn_HangMucMapper()
        {
            Map(x => x.iID_DuAn_HangMucID).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class VDT_DM_DonViThucHienDuAnMapper : ClassMapper<VDT_DM_DonViThucHienDuAn>
    {
        public VDT_DM_DonViThucHienDuAnMapper()
        {
            Map(x => x.iID_DonVi).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class DM_ChuDauTuMapper : ClassMapper<DM_ChuDauTu>
    {
        public DM_ChuDauTuMapper()
        {
            Map(x => x.ID).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class VDT_DM_ChiPhiMapper : ClassMapper<VDT_DM_ChiPhi>
    {
        public VDT_DM_ChiPhiMapper()
        {
            Map(x => x.iID_ChiPhi).Key(KeyType.Guid);
            AutoMap();
        }
    }
    
    public class VDT_DM_DuAn_ChiPhiMapper : ClassMapper<VDT_DM_DuAn_ChiPhi>
    {
        public VDT_DM_DuAn_ChiPhiMapper()
        {
            Map(x => x.iID_DuAn_ChiPhi).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class QLNH_DM_TyGiaMapper : ClassMapper<NH_DM_TiGia>
    {
        public QLNH_DM_TyGiaMapper()
        {
            Map(x => x.ID).Key(KeyType.Guid);
            AutoMap();
        }
    }

    public class QLNH_DM_NhuCauChiQuyMapper : ClassMapper<NH_NhuCauChiQuy>
    {
        public QLNH_DM_NhuCauChiQuyMapper()
        {
            Map(x => x.ID).Key(KeyType.Guid);
            AutoMap();
        }
    }
}
