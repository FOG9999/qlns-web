using AutoMapper.Attributes;
using Viettel.Domain.DomainModel;

namespace VIETTEL.Models
{
    [MapsFrom(typeof(NS_DanhMuc_BaoCao_ChuKy), ReverseMap = true)]
    public class ChuKyViewModel
    {
        public virtual int iID_MaBaoCao_ChuKy { get; set; }
        public virtual string sTenBaoCao { get; set; }
        public virtual string sController { get; set; }
        public virtual int iID_MaPhanHe { get; set; }

        #region chuky1

        public virtual int iID_MaChucDanh1 { get; set; }
        public virtual string sTenChucDanh1 { get; set; }

        public virtual int iID_MaThuaLenh1 { get; set; }
        public virtual string sTenThuaLenh1 { get; set; }

        public virtual int iID_MaTen1 { get; set; }
        public virtual string sTen1 { get; set; }

        #endregion

        #region chuky khac
        public virtual string sTenChucDanh2 { get; set; }
        public virtual int iID_MaChucDanh2 { get; set; }
        public virtual int iID_MaChucDanh3 { get; set; }
        public virtual int iID_MaChucDanh4 { get; set; }
        public virtual int iID_MaChucDanh5 { get; set; }
        public virtual string sTenChucDanh3 { get; set; }
        public virtual string sTenChucDanh4 { get; set; }
        public virtual string sTenChucDanh5 { get; set; }
        public virtual int iID_MaThuaLenh2 { get; set; }
        public virtual int iID_MaThuaLenh3 { get; set; }
        public virtual int iID_MaThuaLenh4 { get; set; }
        public virtual int iID_MaThuaLenh5 { get; set; }
        public virtual string sTenThuaLenh2 { get; set; }
        public virtual string sTenThuaLenh3 { get; set; }
        public virtual string sTenThuaLenh4 { get; set; }
        public virtual string sTenThuaLenh5 { get; set; }
        public virtual int iID_MaTen2 { get; set; }
        public virtual int iID_MaTen3 { get; set; }
        public virtual int iID_MaTen4 { get; set; }
        public virtual int iID_MaTen5 { get; set; }
        public virtual string sTen2 { get; set; }
        public virtual string sTen3 { get; set; }
        public virtual string sTen4 { get; set; }
        public virtual string sTen5 { get; set; }
        #endregion



    }
}
