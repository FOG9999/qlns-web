using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLNguonNganSach
{
    public class RptGiaoDuToanTheoLNS
    {
        #region Phan nguon
        public decimal? fTongPhanNguonNSNN { get; set; }
        public string sTongPhanNguonNSNN
        {
            get
            {
                return this.fTongPhanNguonNSNN.HasValue ? this.fTongPhanNguonNSNN.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public decimal? fTongPhanNguonTXQP { get; set; }
        public string sTongPhanNguonTXQP
        {
            get
            {
                return this.fTongPhanNguonTXQP.HasValue ? this.fTongPhanNguonTXQP.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public decimal? fTongTienPhanNguon
        {
            get
            {
                return (fTongPhanNguonNSNN ?? 0) + (fTongPhanNguonTXQP ?? 0);
            }
        }
        public string sTongTienPhanNguon
        {
            get
            {
                return this.fTongTienPhanNguon.HasValue ? this.fTongTienPhanNguon.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        #endregion
        #region Du toan
        public decimal? fTongDuToanNSNN { get; set; }
        public string sTongDuToanNSNN
        {
            get
            {
                return this.fTongDuToanNSNN.HasValue ? this.fTongDuToanNSNN.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public decimal? fTongDuToanTXQP { get; set; }
        public string sTongDuToanTXQP
        {
            get
            {
                return this.fTongDuToanTXQP.HasValue ? this.fTongDuToanTXQP.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public decimal? fTongTienDuToan
        {
            get
            {
                return (fTongDuToanNSNN ?? 0) + (fTongDuToanTXQP ?? 0);
            }
        }
        public string sTongTienDuToan
        {
            get
            {
                return this.fTongTienDuToan.HasValue ? this.fTongTienDuToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        #endregion

        public List<ChiTietBoSung> lstDuToanChiTiet { get; set; }
        public List<ChiTietBoSung> lstDuToanChoPheDuyet { get; set; }
    }

    public class TongTien
    {
        //public decimal? fTongTien { get; set; }
        //public string sTongTien
        //{
        //    get
        //    {
        //        return this.fTongTien.HasValue ? this.fTongTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
        //    }
        //}

        public decimal? fTongNSNN { get; set; }
        //public string sTongNSNN
        //{
        //    get
        //    {
        //        return this.fTongNSNN.HasValue ? this.fTongNSNN.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
        //    }
        //}

        public decimal? fTongTXQP { get; set; }
        //public string sTongTXQP
        //{
        //    get
        //    {
        //        return this.fTongTXQP.HasValue ? this.fTongTXQP.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
        //    }
        //}
    }

    public class ChiTietBoSung
    {
        public string GhiChu { get; set; }
        public decimal? fTienNSNN { get; set; }
        public string sTienNSNN
        {
            get
            {
                return this.fTienNSNN.HasValue ? this.fTienNSNN.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public decimal? fTienTXQP { get; set; }
        public string sTienTXQP
        {
            get
            {
                return this.fTienTXQP.HasValue ? this.fTienTXQP.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }
        public decimal? iTongTien
        {
            get
            {
                return (fTienNSNN ?? 0) + (fTienTXQP ?? 0);
            }
        }

        public string sTongTien
        {
            get
            {
                return this.iTongTien.HasValue ? this.iTongTien.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : string.Empty;
            }
        }

        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }

        public Guid iID_NhiemVu { get; set; }
        public string sMaNoiDungChi { get; set; }
        public string sSoQuyetDinh { get; set; }
    }
}
