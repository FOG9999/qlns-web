using DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace VIETTEL.Models
{
    /// <summary>
    /// Lớp CapPhat_BangDuLieu cho phần bảng của Cấp phát
    /// </summary>
    public class CapPhat_BangDuLieu_C : BangDuLieu
    {
        // loai hien thi chung tu chi tiet: tat ca, cap phat, chua cap phat

        public static string MaDonViChoCapPhat = "";
        private List<List<Double>> _arrGiaTriNhom = new List<List<Double>>();
        private List<int> _arrChiSoNhom = new List<int>();
        private List<String> _arrMaMucLucNganSach = new List<String>();
        private DataTable _dtDonVi = null;
        public DataRow chungTu = null;

        public String strDSChiSoNhom
        {
            get
            {
                String vR = "";
                for (int i = 0; i < _arrChiSoNhom.Count; i++)
                {
                    if (i > 0) vR += ",";
                    vR += _arrChiSoNhom[i];
                }
                return vR;
            }
        }
        private string _dsTruong = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa";
        private string _dsTruongTieuDe = "LNS,L,K,M,TM,TTM,NG,Nội dung";
        private string _dsTruongDoRong = "60,30,30,40,40,30,30,350";
        String sMucCap = "sNG";
        public String strMaMucLucNganSach
        {
            get
            {
                String vR = "";
                for (int i = 0; i < _arrMaMucLucNganSach.Count; i++)
                {
                    if (i > 0) vR += ",";
                    vR += _arrMaMucLucNganSach[i];
                }
                return vR;
            }
        }

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaCapPhat"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        public CapPhat_BangDuLieu_C(String iID_MaCapPhat, Dictionary<String, String> arrGiaTriTimKiem, String maLoai, String MaND, String IPSua, String Loai)
        {
            this._iID_Ma = iID_MaCapPhat;
            this._MaND = MaND;
            this._IPSua = IPSua;

            String SQL;
            SqlCommand cmd;
            SQL = "SELECT * FROM CP_CapPhat WHERE iID_MaCapPhat=@iID_MaCapPhat AND iTrangThai=1";
            cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@iID_MaCapPhat", _iID_Ma);
            cmd.CommandText = SQL;
            this.chungTu = Connection.GetDataTable(cmd).Rows[0];
            cmd.Dispose();
            _dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(_MaND);
            sMucCap = Convert.ToString(chungTu["sLoai"]);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(chungTu["iID_MaTrangThaiDuyet"]);

            Boolean ND_DuocSuaChungTu = LuongCongViecModel.NguoiDung_DuocSuaChungTu(CapPhatModels.iID_MaPhanHe, MaND, iID_MaTrangThaiDuyet);
            if (LuongCongViecModel.KiemTra_TrangThaiDaDuyet(CapPhatModels.iID_MaPhanHe, iID_MaTrangThaiDuyet))
            {
                _ChiDoc = true;
            }

            if (ND_DuocSuaChungTu == false)
            {
                _ChiDoc = true;
            }

            if (LuongCongViecModel.KiemTra_TrangThaiTrinhDuyet(CapPhatModels.iID_MaPhanHe, iID_MaTrangThaiDuyet) &&
                ND_DuocSuaChungTu)
            {
                _CoCotDuyet = true;
                _DuocSuaDuyet = true;
            }

            if (LuongCongViecModel.KiemTra_TrangThaiTuChoi(CapPhatModels.iID_MaPhanHe, iID_MaTrangThaiDuyet))
            {
                _CoCotDuyet = true;
            }

            _DuocSuaChiTiet = LuongCongViecModel.NguoiDung_DuocThemChungTu(CapPhatModels.iID_MaPhanHe, MaND);


            //trolytonghop dc them chung tu
            Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
            Boolean CheckTrangThaiDuyetMoiTao = LuongCongViecModel.KiemTra_TrangThaiKhoiTao(CapPhatModels.iID_MaPhanHe, iID_MaTrangThaiDuyet);
            if (checkTroLyTongHop && CheckTrangThaiDuyetMoiTao)
            {
                ND_DuocSuaChungTu = true;
                _DuocSuaChiTiet = true;
                _ChiDoc = false;

            }

            if (Loai == "4")
            {
                _dtChiTiet = CapPhat_ChungTuChiTietModels.LayDtChungTuChiTietDMNB(_iID_Ma, arrGiaTriTimKiem, MaND);
            }
            else
            {
                _dtChiTiet = CapPhat_ChungTuChiTietModels.LayDtChungTuChiTietCuc(_iID_Ma, arrGiaTriTimKiem, MaND);
            }

            _dtChiTiet_Cu = _dtChiTiet.Copy();


            DienDuLieu(maLoai, Loai);
        }
        /// <summary>
        /// Hàm hủy bỏ sẽ hủy dữ liệu của bảng _dtDonVi
        /// </summary>
        ~CapPhat_BangDuLieu_C()
        {
            if (_dtDonVi != null) _dtDonVi.Dispose();
        }

        /// <summary>
        /// Thuộc tính lấy danh sách mã đơn vị và tên đơn vị cho Javascript
        /// </summary>
        public String strDSDonVi
        {
            get
            {
                String _strDSDonVi = "";
                for (int csDonVi = 0; csDonVi < _dtDonVi.Rows.Count; csDonVi++)
                {
                    if (csDonVi > 0) _strDSDonVi += "##";
                    _strDSDonVi += String.Format("{0}##{1}", _dtDonVi.Rows[csDonVi]["iID_MaDonVi"], _dtDonVi.Rows[csDonVi]["sTen"]);
                }
                return _strDSDonVi;
            }
        }
        /// <summary>
        /// Hàm điền dữ liệu
        /// Mục đích: Điền tất cả thông tin vào các tham số của đối tượng Bảng dữ liệu
        /// </summary>
        protected void DienDuLieu(String sMaLoai, String Loai)
        {
            // lấy danh sách trường tiền chứng từ
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien.Split(',');
            if (_arrDuLieu == null)
            {
                if (sMaLoai == "ChiTieu")
                {
                    CapNhapHangTongCong();
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if ((String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_PhanBo"])) || Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) <= 0))
                            _dtChiTiet.Rows.RemoveAt(i);
                    }
                }
                else if (sMaLoai == "ConChiTieu")
                {
                    CapNhapHangTongCong();
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_PhanBo"])) && !String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_DaCap"])))
                        {
                            if (((Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) - Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_DaCap"])) == 0))
                                _dtChiTiet.Rows.RemoveAt(i);
                        }
                    }
                }
                else if (sMaLoai == "CapPhat")
                {
                    CapNhapHangTongCong();
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if ((String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi"])) || Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi"]) <= 0))
                            _dtChiTiet.Rows.RemoveAt(i);
                    }
                }
                else if (sMaLoai == "QuaChiTieu")
                {
                    CapNhapHangTongCong();
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_PhanBo"])) && !String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_DaCap"])) && !String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi"])))
                        {
                            if (((Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi"]) - (Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) - Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_DaCap"]))) <= 0))
                                _dtChiTiet.Rows.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    CapNhapHangTongCong();
                }
                DeleteRowsZero();
                CapNhapDanhSachMaHang();
                CapNhapDanhSachMaCot_Fixed(Loai);
                CapNhapDanhSachMaCot_Slide();
                CapNhapDanhSachMaCot_Them();
                CapNhap_arrLaHangCha(Loai);
                CapNhap_arrEdit();
                CapNhap_arrDuLieu();
                CapNhap_arrThayDoi();
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
                String MaHang = "";
                if ("sM" == sMucCap)
                {
                    if (!String.IsNullOrEmpty(Convert.ToString((R["sM"]))))
                    {
                        MaHang = String.Format("{0}_{1}_{2}", R["iID_MaCapPhatChiTiet"], R["iID_MaMucLucNganSach"], R["iID_MaDonVi"]);
                    }
                }
                else if ("sTTM" == sMucCap)
                {
                    if (!String.IsNullOrEmpty(Convert.ToString((R["sTM"]))))
                    {
                        MaHang = String.Format("{0}_{1}_{2}", R["iID_MaCapPhatChiTiet"], R["iID_MaMucLucNganSach"], R["iID_MaDonVi"]);
                    }
                }
                else
                {
                    if (Convert.ToBoolean(R["bLaHangCha"]) == false)
                    {
                        MaHang = String.Format("{0}_{1}_{2}", R["iID_MaCapPhatChiTiet"], R["iID_MaMucLucNganSach"], R["iID_MaDonVi"]);
                    }
                }

                _arrDSMaHang.Add(MaHang);

                _arrMaMucLucNganSach.Add(Convert.ToString(R["iID_MaMucLucNganSach"]));
            }
        }

        /// <summary>
        /// Hàm thêm danh sách cột Fixed vào bảng
        ///     - Cột Fixed của bảng gồm:
        ///         + Các trường của mục lục ngân sách
        ///         + Trường sMaCongTrinh, sTenCongTrinh của cột tiền
        ///     - Cập nhập số lượng cột Fixed
        /// </summary>
        protected void CapNhapDanhSachMaCot_Fixed(String Loai)
        {
            //Khởi tạo các mảng
            _arrHienThiCot = new List<Boolean>();
            _arrTieuDe = new List<string>();
            _arrDSMaCot = new List<string>();
            _arrWidth = new List<int>();

            String[] arrDSTruong = _dsTruong.Split(',');
            String[] arrDSTruongTieuDe = _dsTruongTieuDe.Split(',');
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien.Split(',');
            String[] arrDSTruongTienTieuDe = MucLucNganSachModels.strDSTruongTienTieuDe.Split(',');
            String[] arrDSTruongTienDoRong = MucLucNganSachModels.strDSTruongTienDoRong.Split(',');
            String[] arrDSTruongDoRong = _dsTruongDoRong.Split(',');

            if (Loai == "4")
            {
                arrDSTruong = "sNG,sMoTa".Split(',');
                arrDSTruongTieuDe = "Mã Ngành,Nội dung".Split(',');
                arrDSTruongDoRong = "60,350".Split(',');
            }

            //Xác định các cột tiền sẽ hiển thị
            _arrCotTienDuocHienThi = new Dictionary<String, Boolean>();
            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {
                _arrCotTienDuocHienThi.Add(arrDSTruongTien[j], false);
                for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
                {
                    DataRow R = _dtChiTiet.Rows[i];
                    //HungPX: NghiepNC yêu cầu chỉ để trường rTuChi
                    if (String.IsNullOrEmpty(R["iID_MaMucLucNganSach"].ToString()))
                    {
              
                    }
                    else
                    {
                        _arrCotTienDuocHienThi[arrDSTruongTien[j]] = true;
                        break;
                    }
                }
            }
            DataTable dtCapPhat = CapPhat_ChungTuModels.LayToanBoThongTinChungTu(_iID_Ma);
            DataRow dr = dtCapPhat.Rows[0];
            int index = -1;
            if (dtCapPhat != null && dtCapPhat.Rows.Count > 0)
            {
                index = getIndex(Convert.ToString(dr["sLoai"]));
            }
            //Tiêu đề fix: Thêm trường sMaCongTrinh, sTenCongTrinh
            for (int j = 0; j < arrDSTruongTieuDe.Length; j++)
            {
                _arrDSMaCot.Add(arrDSTruong[j]);
                _arrTieuDe.Add(arrDSTruongTieuDe[j]);
                _arrWidth.Add(Convert.ToInt32(arrDSTruongDoRong[j]));
                if (arrDSTruong[j] == "rTongSo")
                {
                    _arrHienThiCot.Add(false);
                }
                else
                {
                    if (arrDSTruong[j] != "sMoTa" && (getIndex(arrDSTruong[j]) > index || index == -1))
                        _arrHienThiCot.Add(false);
                    else
                        _arrHienThiCot.Add(true);
                }

                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
            }

            _nCotFixed = _arrDSMaCot.Count;
        }

        /// <summary>
        /// Hàm thêm danh sách cột Slide vào bảng
        ///     - Cột Slide của bảng gồm:
        ///         + Trường iID_MaDonVi
        ///         + Trường của cột tiền trừ sMaCongTrinh, sTenCongTrinh
        ///             - Cột phân bổ: Cột tổng phân bổ cho đơn vị
        ///             - Cột đã cấp: Cột đã cấp cho đơn vị trong năm ngân sách
        ///             - Cột còn lại: Cột còn lại chưa cấp cho đơn vị
        ///         + Trường sTongSo
        ///         + Trường bDongY, sLyDo
        ///     - Cập nhập số lượng cột Slide
        /// </summary>
        protected void CapNhapDanhSachMaCot_Slide()
        {
            String[] arrDSTruongTien = "rTuChi".Split(',');
            String[] arrDSTruongTienTieuDe = "Tự chi".Split(',');
            String[] arrDSTruongTienDoRong = "130".Split(',');

            Boolean bHienThiCot = true;

            if (!String.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()))
            {
                bHienThiCot = false;
            }

            //HungPX: Add cột mã đơn vị
            _arrDSMaCot.Add("iID_MaDonVi");
            _arrTieuDe.Add("Đơn vị");
            _arrWidth.Add(40);
            _arrHienThiCot.Add(bHienThiCot);
            _arrSoCotCungNhom.Add(1);
            _arrTieuDeNhomCot.Add("");

            //HungPX: Add cột Tên đơn vị
            _arrDSMaCot.Add("sTenDonVi");
            _arrTieuDe.Add("Tên Đơn vị");
            _arrWidth.Add(150);
            _arrHienThiCot.Add(bHienThiCot);
            _arrSoCotCungNhom.Add(1);
            _arrTieuDeNhomCot.Add("");

            if (Convert.ToString(chungTu["iID_MaTinhChatCapThu"]) != "1" && Convert.ToString(chungTu["iID_MaTinhChatCapThu"]) != "2")
            {
                bHienThiCot = true;
            }
            else
            {
                bHienThiCot = false;
            }

            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {

                if (arrDSTruongTien[j] != "sTenCongTrinh" && arrDSTruongTien[j] != "rTongSo"
                    && _arrCotTienDuocHienThi[arrDSTruongTien[j]])
                {
                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_Phanbo");
                    _arrTieuDe.Add("Chỉ tiêu");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(bHienThiCot);
                    _arrSoCotCungNhom.Add(1);
                    _arrTieuDeNhomCot.Add("");

                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_DaCap");
                    _arrTieuDe.Add("Đã cấp");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(bHienThiCot);
                    _arrSoCotCungNhom.Add(1);
                    _arrTieuDeNhomCot.Add("");

                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_ConLai");
                    _arrTieuDe.Add("Còn lại");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(bHienThiCot);
                    _arrSoCotCungNhom.Add(1);
                    _arrTieuDeNhomCot.Add("");

                    _arrDSMaCot.Add(arrDSTruongTien[j]);
                    _arrTieuDe.Add("Cấp phát");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(true);
                    _arrSoCotCungNhom.Add(1);
                    _arrTieuDeNhomCot.Add("");

                }
            }

            //Them cot duyet
            if (CoCotDuyet)
            {
                //Cột đồng ý
                _arrDSMaCot.Add("bDongY");
                if (_ChiDoc)
                {
                    _arrTieuDe.Add("<div class='check'></div>");
                }
                else
                {
                    _arrTieuDe.Add("<div class='check' onclick='BangDuLieu_CheckAll();'></div>");
                }
                _arrWidth.Add(20);
                _arrHienThiCot.Add(false);
                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
                //Cột Lý do
                _arrDSMaCot.Add("sLyDo");
                _arrTieuDe.Add("Nhận xét");
                _arrWidth.Add(200);
                _arrHienThiCot.Add(true);
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
            String strDSTruong = "";

            strDSTruong =
                "sMauSac,sFontColor,sFontBold";

            String[] arrDSTruong = strDSTruong.Split(',');
            for (int j = 0; j < arrDSTruong.Length; j++)
            {
                _arrDSMaCot.Add(arrDSTruong[j]);
                _arrTieuDe.Add(arrDSTruong[j]);
                _arrWidth.Add(0);
                _arrHienThiCot.Add(false);
                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
            }
        }

        /// <summary>
        /// Hàm xác định hàng cha, con
        /// </summary>
        protected void CapNhap_arrLaHangCha(String Loai)
        {
            //Xác định hàng là hàng cha, con
            _arrCSCha = new List<int>();
            _arrLaHangCha = new List<bool>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                DataRow R = _dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                if (Loai == "4")
                {
                    if (!String.IsNullOrEmpty(Convert.ToString((R["sNG"]))) && !String.IsNullOrEmpty(Convert.ToString((R["iID_MaDonVi"]))))
                    {
                        _arrLaHangCha.Add(false);
                    }
                    else
                        _arrLaHangCha.Add(true);
                }
                else
                {
                    _arrLaHangCha.Add(Convert.ToBoolean(R["bLaHangCha"]));                    
                }

                int CSCha = -1;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Convert.ToString(R["iID_MaMucLucNganSach"]) == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]) && String.IsNullOrEmpty(Convert.ToString((_dtChiTiet.Rows[j]["iID_MaDonVi"]))))
                    {
                        CSCha = j;
                        break;
                    }

                    if (Convert.ToString(R["iID_MaMucLucNganSach_Cha"]) == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]) && String.IsNullOrEmpty(Convert.ToString((R["iID_MaDonVi"]))))
                    {
                        CSCha = j;
                        break;
                    }

                    if (!String.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()))
                    {
                        if (Convert.ToString(R["iID_MaMucLucNganSach_Cha"]) == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]))
                        {
                            CSCha = j;
                            break;
                        }
                    }

                }

                _arrCSCha.Add(CSCha);

            }
        }

        /// <summary>
        /// Hàm xác định các ô có được Edit hay không
        /// </summary>
        protected void CapNhap_arrEdit()
        {
            _arrEdit = new List<List<string>>();
            // HungPX: lấy giá trị chi tiết đến của chứng từ
            DataTable dtCapPhat;
            SqlCommand cmdCapPhat = new SqlCommand();
            String sqlCapPhat = "Select * from Cp_CapPhat where iID_MaCapPhat = @iID_MaCapPhat";
            cmdCapPhat.Parameters.AddWithValue("@iID_MaCapPhat", _iID_Ma);
            cmdCapPhat.CommandText = sqlCapPhat;
            dtCapPhat = Connection.GetDataTable(cmdCapPhat);
            String sLoai = Convert.ToString(dtCapPhat.Rows[0]["sLoai"]);
            dtCapPhat.Dispose();
            cmdCapPhat.Dispose();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                Boolean okHangChiDoc = false;
                _arrEdit.Add(new List<string>());
                DataRow R = _dtChiTiet.Rows[i];
                //HungPX : cho phép nhập vào hàng cha "sloai"
                if (Convert.ToBoolean(R["bLaHangCha"]))
                {
                    okHangChiDoc = true;                 
                }

                for (int j = 0; j < _arrDSMaCot.Count; j++)
                {
                    Boolean okOChiDoc = true;
                    var te = _arrDSMaCot[j];
                    //Xac dinh o chi doc
                    if (_arrDSMaCot[j] == "iID_MaDonVi" && chungTu["iID_MaTinhChatCapThu"].ToString() == "1" && chungTu["iID_MaTinhChatCapThu"].ToString() == "2")
                    {
                        // Cot don vi
                        if (_DuocSuaChiTiet && _ChiDoc == false && okHangChiDoc == false)
                        {
                            okOChiDoc = false;
                        }
                    }
                    else if (_arrDSMaCot[j] == "bDongY" || _arrDSMaCot[j] == "sLyDo")
                    {
                        //Cot duyet
                        if (_DuocSuaDuyet && _ChiDoc == false && okHangChiDoc == false)
                        {
                            okOChiDoc = false;
                        }
                    }
                    else
                    {
                        //Cot tien
                        if (_DuocSuaChiTiet &&
                                _ChiDoc == false &&
                                okHangChiDoc == false &&
                                _arrDSMaCot[j] != "rTongSo" &&
                                _arrDSMaCot[j].EndsWith("_PhanBo") == false &&
                                _arrDSMaCot[j].EndsWith("_DaCap") == false &&
                                _arrDSMaCot[j].EndsWith("_ConLai") == false
                                )
                        {
                            okOChiDoc = false;
                        }
                    }
                    if (okOChiDoc)
                    {
                        _arrEdit[i].Add("");
                    }
                    else
                    {
                        _arrEdit[i].Add("1");
                    }
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
                for (int j = 0; j < _arrDSMaCot.Count - 3; j++)
                {
                    //Xac dinh gia tri
                    if (_arrDSMaCot[j].EndsWith("_ConLai"))
                    {
                    }
                    else if (_arrDSMaCot[j].Equals("rTuChi"))
                    {
                        Double GT1 = Convert.ToDouble(_arrDuLieu[i][j - 2]);
                        Double GT2 = Convert.ToDouble(_arrDuLieu[i][j - 3]);
                        Double GT3 = Convert.ToDouble(R[_arrDSMaCot[j]]);
                        _arrDuLieu[i].Add(Convert.ToString(GT2 - GT1 - GT3));
                        _arrDuLieu[i].Add(Convert.ToString(R[_arrDSMaCot[j]]));
                    }
                    else
                    {
                        _arrDuLieu[i].Add(Convert.ToString(R[_arrDSMaCot[j]]));
                    }
                }
                if (Convert.ToDouble(_dtChiTiet.Rows[i]["rTuChi"]) > ((Convert.ToDouble(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) - Convert.ToDouble(_dtChiTiet.Rows[i]["rTuChi_DaCap"]))))
                {
                    _arrDuLieu[i].Add("#FF0000");
                    _arrDuLieu[i].Add("");
                    _arrDuLieu[i].Add("");
                }
                else
                {
                    _arrDuLieu[i].Add("");
                    _arrDuLieu[i].Add("");
                    _arrDuLieu[i].Add("");
                }
            }
        }

        /// <summary>
        /// Hàm tính lại các ô tổng số và tổng cộng các hàng cha
        /// </summary>
        protected void CapNhapHangTongCong()
        {           
            String strDSTruongTien = "rTuChi_Phanbo,rTuChi_DaCap,rTuChi";
            String[] arrDSTruongTien = strDSTruongTien.Split(',');

            int len = arrDSTruongTien.Length;

            //Tinh lai cac hang cha
            for (int i = _dtChiTiet.Rows.Count - 1; i >= 0; i--)
            {
                if ((sMucCap == "sTM" && !String.IsNullOrEmpty(_dtChiTiet.Rows[i]["sTM"].ToString()))
                    || (sMucCap == "sM" && !String.IsNullOrEmpty(_dtChiTiet.Rows[i]["sM"].ToString())))
                {
                    _dtChiTiet.Rows[i]["bLaHangCha"] = false;
                }

                String iID_MaMucLucNganSach = Convert.ToString(_dtChiTiet.Rows[i]["iID_MaMucLucNganSach"]);
                for (int k = 0; k < len; k++)
                {
                    double S;
                    S = 0;
                    double S1 = 0;
                    for (int j = i + 1; j < _dtChiTiet.Rows.Count; j++)
                    {
                        if (String.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()) && chungTu["iID_MaTinhChatCapThu"].ToString() != "1" && chungTu["iID_MaTinhChatCapThu"].ToString() != "2")
                        {
                            if (iID_MaMucLucNganSach == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach_Cha"]) && Convert.ToString(_dtChiTiet.Rows[j]["iID_MaDonVi"]) == "")
                            {
                                S += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien[k]]);
                                _dtChiTiet.Rows[j]["bLaHangCha"] = true;
                            }
                            if (iID_MaMucLucNganSach == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]) && Convert.ToString(_dtChiTiet.Rows[j]["iID_MaDonVi"]) != "")
                            {
                                S1 += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien[k]]);
                                _dtChiTiet.Rows[j]["bLaHangCha"] = false;
                            }
                        }
                        else
                        {
                            if (iID_MaMucLucNganSach == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach_Cha"]))
                            {
                                S += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien[k]]);
                            }
                        }
                    }
                    if (Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien[k]]) != S && S != 0)
                    {
                        _dtChiTiet.Rows[i][arrDSTruongTien[k]] = S;
                    }
                    if (S1 != 0 && Convert.ToString(_dtChiTiet.Rows[i]["iID_MaDonVi"]) == "")
                    {
                        _dtChiTiet.Rows[i][arrDSTruongTien[k]] = S1;
                    }
                }
            }
            //Them Hàng tổng cộng
            DataRow dr = _dtChiTiet.NewRow();
            DataRow dr1 = _dtChiTiet.NewRow();
            DataRow[] arrdr = _dtChiTiet.Select("bLaHangCha=0");
            double Tong = 0;
            for (int k = 0; k < len; k++)
            {
                Tong = 0;
                for (int i = 0; i < arrdr.Length; i++)
                {

                    if (!String.IsNullOrEmpty(Convert.ToString(arrdr[i][arrDSTruongTien[k]])))
                    {
                        Tong += Convert.ToDouble(arrdr[i][arrDSTruongTien[k]]);
                    }

                }
                dr[arrDSTruongTien[k]] = Tong;
                dr1[arrDSTruongTien[k]] = Tong;
            }
            dr["sMoTa"] = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TỔNG CỘNG:";
            dr["bLaHangCha"] = true;
            dr1["sMoTa"] = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TỔNG CỘNG:";
            dr1["bLaHangCha"] = true;
            _dtChiTiet.Rows.Add(dr);
            _dtChiTiet.Rows.InsertAt(dr1, 0);            
        }

        protected void DeleteRowsZero()
        {
            int count = dtChiTiet.Rows.Count - 1;
            for (int i = count; i >= 0; i--)
            {               
                if (Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi"]) == 0 && Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) == 0 && Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_DaCap"]) == 0)
                    _dtChiTiet.Rows.RemoveAt(i);                
            }
        }

        /*
         * <summary>
         * lấy index của chuỗi loại ngân sách: LNS, L, K, M, TM, NG 
         * index cha lớn hơn index con
         * </summary>
         * */
        public static int getIndex(string val)
        {
            String[] arrDSTruong = MucLucNganSachModels.arrDSTruong;
            int index = -1;
            for (int i = 0; i < arrDSTruong.Length; i++)
                if (val == arrDSTruong[i])
                {
                    index = i;
                    break;
                }
            return index;
        }
    }
}
