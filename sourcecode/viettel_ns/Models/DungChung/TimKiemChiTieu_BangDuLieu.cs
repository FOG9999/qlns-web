using System;
using System.Collections.Generic;
using System.Data;
using VIETTEL.Models.TimKiemChiTieu;

namespace VIETTEL.Models
{
    public class TimKiemChiTieu_BangDuLieu : BangDuLieu
    {

        /// /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        public TimKiemChiTieu_BangDuLieu(Dictionary<String, String> arrGiaTriTimKiem, String MaND, String IPSua)
        {
            this._iID_Ma = "";
            this._MaND = MaND;
            this._IPSua = IPSua;

            _dtBang = null;

            _ChiDoc = false;
            _DuocSuaChiTiet = true;
            _dtChiTiet = TimKiemChiTieuModels.GetDuLieu(MaND, null, arrGiaTriTimKiem);
            _dtChiTiet_Cu = _dtChiTiet.Copy();
            DienDuLieu();
        }

        /// <summary>
        /// Hàm điền dữ liệu
        /// Mục đích: Điền tất cả thông tin vào các tham số của đối tượng Bảng dữ liệu
        /// </summary>
        protected void DienDuLieu()
        {
            if (_arrDuLieu == null)
            {
                CapNhapHangTongCong();
                CapNhapDanhSachMaHang();
                CapNhapDanhSachMaCot_Fixed();
                CapNhapDanhSachMaCot_Slide();
                CapNhapDanhSachMaCot_Them();
                CapNhap_arrType_Rieng();
                CapNhap_arrEdit();
                CapNhap_arrDuLieu();
                CapNhap_arrThayDoi();
                CapNhap_arrLaHangCha();
            }
        }

        /// <summary>
        /// Hàm cập nhập vào tham số _arrDSMaHang
        /// </summary>
        protected void CapNhapDanhSachMaHang()
        {
            _arrDSMaHang = new List<string>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                DataRow R = _dtChiTiet.Rows[i];
                String MaHang = String.Format("{0}_{1}_{3}_{4}_{5}_{6}_{7}_{8}", R["dDotNgay"], R["sLNS"], R["sL"], R["sK"], R["sM"], R["sTM"], R["sTTM"], R["sNG"], R["iID_MaDonVi"]);
                _arrDSMaHang.Add(MaHang);
            }
        }

        /// <summary>
        /// Hàm thêm danh sách cột Fixed vào bảng
        ///     - Không có cột Fixed
        ///     - Cập nhập số lượng cột Fixed
        /// </summary>
        protected void CapNhapDanhSachMaCot_Fixed()
        {
            //Khởi tạo các mảng
            _arrHienThiCot = new List<Boolean>();
            _arrTieuDe = new List<string>();
            _arrDSMaCot = new List<string>();
            _arrWidth = new List<int>();

            String[] arrDSTruong = ("dDotNgay,sQD," + ChiTieuModels.strDSTruong + ",iID_MaPhongBan,sID_MaNguoiDungTao").Split(',');
            String[] arrDSTruongTieuDe = ChiTieuModels.strDSTruong_TieuDe.Split(',');
            String[] arrDSTruongDoRong = ChiTieuModels.strDSTruongDoRong.Split(',');
            String[] arrDSTruongAlign = ChiTieuModels.strDSTruongAlign.Split(',');

            //Xác định các cột tiền sẽ hiển thị
            _arrCotTienDuocHienThi = new Dictionary<String, Boolean>();
            _arrAlign = new List<String>();

            for (int j = 0; j < arrDSTruongTieuDe.Length; j++)
            {
                _arrDSMaCot.Add(arrDSTruong[j]);
                _arrTieuDe.Add(arrDSTruongTieuDe[j]);
                _arrWidth.Add(Convert.ToInt32(arrDSTruongDoRong[j]));
                _arrHienThiCot.Add(true);
                _arrAlign.Add(arrDSTruongAlign[j]);
                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
            }

            _nCotFixed = _arrDSMaCot.Count;
        }

        /// <summary>
        /// Hàm thêm danh sách cột Slide vào bảng
        /// <param name="iLoai"></param>
        /// </summary>
        protected void CapNhapDanhSachMaCot_Slide()
        {
            String[] arrDSTruongTien = ChiTieuModels.strDSTruongTien_So_Xau.Split(',');
            String[] arrDSTruongTienTieuDe = ChiTieuModels.strDSTruongTien_TieuDe.Split(',');
            String[] arrDSTruongTien_DoRong = ChiTieuModels.strDSTruongTienDoRong_So.Split(',');
            String[] arrDSTruongTienAlign = ChiTieuModels.strDSTruongTienAlign.Split(',');

            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {
                if (arrDSTruongTien[j] == "rChiTapTrung")
                    continue;

                _arrDSMaCot.Add(arrDSTruongTien[j]);
                _arrTieuDe.Add(arrDSTruongTienTieuDe[j]);
                _arrWidth.Add(Convert.ToInt32(arrDSTruongTien_DoRong[j]));
                _arrHienThiCot.Add(true);
                _arrAlign.Add(arrDSTruongTienAlign[j]);
                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
            }

            _nCotSlide = _arrDSMaCot.Count - _nCotFixed;
        }

        /// <summary>
        /// Hàm thêm các cột thêm của bảng
        /// </summary>
        protected void CapNhapDanhSachMaCot_Them()
        {
        }


        /// <summary>
        /// Hàm xác định các ô có được Edit hay không
        /// </summary>
        protected void CapNhap_arrEdit()
        {
            _arrEdit = new List<List<string>>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                _arrEdit.Add(new List<string>());

                for (int j = 0; j < _arrDSMaCot.Count; j++)
                {
                    _arrEdit[i].Add("");
                }
            }
        }

        /// <summary>
        /// Hàm cập nhập kiểu nhập cho các cột
        ///     - Cột có prefix 'b': kiểu '2' (checkbox)
        ///     - Cột có prefix 'r' hoặc 'i' (trừ 'iID'): kiểu '1' (textbox number)
        ///     - Ngược lại: kiểu '0' (textbox)
        /// </summary>
        protected void CapNhap_arrType_Rieng()
        {
            //Xac dinh kieu truong nhap du lieu
            _arrType = new List<string>();
            for (int j = 0; j < _arrDSMaCot.Count; j++)
            {
                if (_arrDSMaCot[j].StartsWith("b"))
                {
                    //Nhap Kieu checkbox
                    _arrType.Add("2");
                }
                else if (_arrDSMaCot[j].StartsWith("r") || (_arrDSMaCot[j].StartsWith("iID") == false && _arrDSMaCot[j].StartsWith("i")))
                {
                    //Nhap Kieu so
                    _arrType.Add("1");
                }
                else
                {
                    //Nhap kieu xau
                    _arrType.Add("0");
                }
            }
        }

        /// <summary>
        /// Hàm cập nhập mảng dữ liệu
        /// </summary>
        protected void CapNhap_arrDuLieu()
        {
            _arrDuLieu = new List<List<string>>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                _arrDuLieu.Add(new List<string>());
                DataRow R = _dtChiTiet.Rows[i];
                for (int j = 0; j < _arrDSMaCot.Count; j++)
                {
                    //Xac dinh gia tri
                    _arrDuLieu[i].Add(Convert.ToString(R[_arrDSMaCot[j]]));
                }
            }
        }

        protected void CapNhap_arrLaHangCha()
        {
            //Xác định hàng là hàng cha, con
            _arrCSCha = new List<int>();
            _arrLaHangCha = new List<bool>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                DataRow R = _dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                if (i > 0)
                {
                    _arrLaHangCha.Add(false);
                    _arrCSCha.Add(-1);
                }
                else
                {
                    _arrLaHangCha.Add(true);
                    _arrCSCha.Add(-1);
                }
            }
        }

        /// <summary>
        /// Hàm tính lại các ô tổng số và tổng cộng các hàng cha
        /// </summary>
        protected void CapNhapHangTongCong()
        {
            String strDSTruongTien = ChiTieuModels.strDSTruongTien_So;
            String[] arrDSTruongTien = strDSTruongTien.Split(',');

            int len = arrDSTruongTien.Length;

            //Them Hàng tổng cộng
            DataRow dr = _dtChiTiet.NewRow();

            double Tong = 0;
            for (int k = 0; k < len; k++)
            {
                Tong = 0;
                for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
                {

                    if (!String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i][arrDSTruongTien[k]])))
                    {
                        Tong += Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien[k]]);
                    }

                }
                dr[arrDSTruongTien[k]] = Tong;
            }
            dr["sMoTa"] = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TỔNG CỘNG:";
            _dtChiTiet.Rows.InsertAt(dr, 0);

        }
    }
}
