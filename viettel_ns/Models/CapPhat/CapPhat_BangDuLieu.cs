using DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace VIETTEL.Models
{
    /// <summary>
    /// Lớp CapPhat_BangDuLieu cho phần bảng của Cấp phát
    /// </summary>
    public class CapPhat_BangDuLieu : BangDuLieu_E
    {
        // loai hien thi chung tu chi tiet: tat ca, cap phat, chua cap phat
        private List<int> _arrChiSoNhom = new List<int>();
        private DataTable _dtDonVi = null;
        public DataRow chungTu = null;        
        private string _sMucCap = "sNG";
        private string _iLoai = "1";

        public string strDSChiSoNhom
        {
            get
            {
                string vR = "";
                for (int i = 0; i < _arrChiSoNhom.Count; i++)
                {
                    if (i > 0) vR += ",";
                    vR += _arrChiSoNhom[i];
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
        public CapPhat_BangDuLieu(string iID_MaCapPhat, Dictionary<string, string> arrGiaTriTimKiem = null, string maLoai = null, string MaND = "", string IPSua = "")
        {
            this._iID_Ma = iID_MaCapPhat;
            this._MaND = MaND;
            this._IPSua = IPSua;
            this.CotId = "iID_MaCapPhatChiTiet";
            this._arrGiaTriTimKiem = arrGiaTriTimKiem;

            SqlCommand cmd = new SqlCommand("SELECT * FROM CP_CapPhat WHERE iID_MaCapPhat=@iID_MaCapPhat AND iTrangThai=1");
            cmd.Parameters.AddWithValue("@iID_MaCapPhat", _iID_Ma);
            this.chungTu = Connection.GetDataTable(cmd).Rows[0];
            cmd.Dispose();
            this._dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(_MaND);
            this._sMucCap = Convert.ToString(chungTu["sLoai"]);
            this._iLoai = Convert.ToString(chungTu["iLoai"]);
            _DuocSuaChiTiet = true;
            _ChiDoc = false;            

            DienDuLieu(maLoai, _iLoai);
        }
        /// <summary>
        /// Hàm hủy bỏ sẽ hủy dữ liệu của bảng _dtDonVi
        /// </summary>
        ~CapPhat_BangDuLieu()
        {
            if (_dtDonVi != null) _dtDonVi.Dispose();
        }

        /// <summary>
        /// Thuộc tính lấy danh sách mã đơn vị và tên đơn vị cho Javascript
        /// </summary>
        public string strDSDonVi
        {
            get
            {
                string _strDSDonVi = "";
                for (int csDonVi = 0; csDonVi < _dtDonVi.Rows.Count; csDonVi++)
                {
                    if (csDonVi > 0) _strDSDonVi += "##";
                    _strDSDonVi += string.Format("{0}##{1}", _dtDonVi.Rows[csDonVi]["iID_MaDonVi"], _dtDonVi.Rows[csDonVi]["sTen"]);
                }
                return _strDSDonVi;
            }
        }
        /// <summary>
        /// Hàm điền dữ liệu
        /// Mục đích: Điền tất cả thông tin vào các tham số của đối tượng Bảng dữ liệu
        /// </summary>
        protected void DienDuLieu(string sMaLoai, string Loai)
        {
            if (Loai == "4")
            {
                _dtChiTiet = CapPhat_ChungTuChiTietModels.LayDtChungTuChiTietDMNB(_iID_Ma, _arrGiaTriTimKiem, _MaND);
            }
            else
            {
                _dtChiTiet = CapPhat_ChungTuChiTietModels.LayDtChungTuChiTietCuc(_iID_Ma, _arrGiaTriTimKiem, _MaND);
            }
            _dtChiTiet_Cu = _dtChiTiet.Copy();

            if (_arrDuLieu == null)
            {
                CapNhap_HangTongCong();
                if (sMaLoai == "ChiTieu")
                {
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if ((string.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_PhanBo"])) || Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) <= 0))
                            _dtChiTiet.Rows.RemoveAt(i);
                    }
                }
                else if (sMaLoai == "ConChiTieu")
                {
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_PhanBo"])) && !string.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_DaCap"])))
                        {
                            if (((Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) - Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_DaCap"])) == 0))
                                _dtChiTiet.Rows.RemoveAt(i);
                        }
                    }
                }
                else if (sMaLoai == "CapPhat")
                {
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if ((string.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi"])) || Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi"]) <= 0))
                            _dtChiTiet.Rows.RemoveAt(i);
                    }
                }
                else if (sMaLoai == "QuaChiTieu")
                {
                    int count = dtChiTiet.Rows.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_PhanBo"])) && !string.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi_DaCap"])) && !string.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i]["rTuChi"])))
                        {
                            if (((Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi"]) - (Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) - Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_DaCap"]))) <= 0))
                                _dtChiTiet.Rows.RemoveAt(i);
                        }
                    }
                }
                
                DeleteRowsZero();
                CapNhap_CotId("iID_MaCapPhatChiTiet,iID_MaMucLucNganSach,iID_MaDonVi");
                CapNhapDSCots();
                CapNhap_arrLaHangCha();
                CapNhap_arrEdit();
                CapNhap_arrDuLieu();
                CapNhap_arrThayDoi();
            }
        }

        protected override IEnumerable<Cot> LayDSCot()
        {
            var items = new List<Cot>();
            if (_iLoai == "4")
            {
                items.AddRange(
                    new List<Cot>()
                    {
                        new Cot(ten: "sNG", tieuDe: "NG", doRongCot:30, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sMoTa", tieuDe: "Nội dung", doRongCot:350, kieuCanLe: "left", chiDoc: true, coDinh: true),
                    });
            }
            else {
                items.AddRange(
                    new List<Cot>()
                    {
                        new Cot(ten: "sLNS", tieuDe: "LNS", doRongCot:60, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sL", tieuDe: "L", doRongCot:30, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sK", tieuDe: "K", doRongCot:30, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sM", tieuDe: "M", doRongCot:40, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sTM", tieuDe: "TM", doRongCot:40, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sTTM", tieuDe: "TTM", doRongCot:30, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sNG", tieuDe: "NG", doRongCot:30, kieuCanLe: "center", chiDoc: true, coDinh: true),
                        new Cot(ten: "sMoTa", tieuDe: "Nội dung", doRongCot:280, kieuCanLe: "left", chiDoc: true, coDinh: true),
                    }
                );
                switch (_sMucCap)
                {
                    case "sM":
                        items.First(x => x.Ten == "sTM").Hien = false;
                        items.First(x => x.Ten == "sTTM").Hien = false;
                        items.First(x => x.Ten == "sNG").Hien = false;
                        break;
                    case "sTM":
                        items.First(x => x.Ten == "sTTM").Hien = false;
                        items.First(x => x.Ten == "sNG").Hien = false;
                        break;
                    default:
                        break;
                };
            }
            items.AddRange(
                    new List<Cot>()
                    {
                        new Cot(ten: "iID_MaDonVi", tieuDe: "Mã đơn vị", doRongCot:40, kieuCanLe: "center", chiDoc: true, coDinh: true, hien: false),
                        new Cot(ten: "sTenDonVi", tieuDe: "Tên đơn vị", doRongCot:150, kieuCanLe: "left", chiDoc: true, coDinh: true, kieuNhap: "3"),
                        new Cot(ten: "rTuChi_PhanBo", tieuDe: "Chỉ tiêu", doRongCot:130, kieuCanLe: "left", chiDoc: true, kieuNhap: "1"),
                        new Cot(ten: "rTuChi_DaCap", tieuDe: "Đã cấp", doRongCot:130, kieuCanLe: "left", chiDoc: true, kieuNhap: "1"),
                        new Cot(ten: "rTuChi_ConLai", tieuDe: "Còn lại", doRongCot:130, kieuCanLe: "left", chiDoc: true, kieuNhap: "1"),
                        new Cot(ten: "rTuChi", tieuDe: "Cấp phát", doRongCot:130, kieuCanLe: "left", chiDoc: false, kieuNhap: "1"),
                        new Cot(ten: "sMauSac", hien: false),
                        new Cot(ten: "sFontColor", hien: false),
                        new Cot(ten: "sFontBold",hien: false),
                    }
                );
            if (!string.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()))
            {
                items.First(x => x.Ten == "sTenDonVi").Hien = false;
            }            
            if (Convert.ToString(chungTu["iID_MaTinhChatCapThu"]) == "1")
            {
                items.First(x => x.Ten == "rTuChi_PhanBo").Hien = false;
                items.First(x => x.Ten == "rTuChi_DaCap").Hien = false;
                items.First(x => x.Ten == "rTuChi_ConLai").Hien = false;
                if (string.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()))
                {
                    items.First(x => x.Ten == "sTenDonVi").CoDinh = false;
                    items.First(x => x.Ten == "sTenDonVi").ChiDoc = false;
                }
            }
            return items;
        }  

        /// <summary>
        /// Hàm xác định hàng cha, con
        /// </summary>
        protected override void CapNhap_arrLaHangCha()
        {
            //Xác định hàng là hàng cha, con
            _arrCSCha = new List<int>();
            _arrLaHangCha = new List<bool>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                DataRow R = _dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                if (_iLoai == "4")
                {
                    if (!string.IsNullOrEmpty(Convert.ToString((R["sNG"]))) && !string.IsNullOrEmpty(Convert.ToString((R["iID_MaDonVi"]))))
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
                    if (Convert.ToString(R["iID_MaMucLucNganSach"]) == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]) && string.IsNullOrEmpty(Convert.ToString((_dtChiTiet.Rows[j]["iID_MaDonVi"]))))
                    {
                        CSCha = j;
                        break;
                    }

                    if (Convert.ToString(R["iID_MaMucLucNganSach_Cha"]) == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]) && (string.IsNullOrEmpty(Convert.ToString((R["iID_MaDonVi"]))) 
                                                                                                                                                || (!string.IsNullOrEmpty(Convert.ToString((R["iID_MaDonVi"]))) && !string.IsNullOrEmpty(_arrGiaTriTimKiem["iID_MaDonVi"]))))
                    {
                        CSCha = j;
                        break;
                    }

                    if (!string.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()))
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
        protected override void CapNhap_arrEdit()
        {
            _arrEdit = new List<List<string>>();            
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
                    if (_arrDSMaCot[j] == "iID_MaDonVi" && chungTu["iID_MaTinhChatCapThu"].ToString() == "1")
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
        protected override void CapNhap_arrDuLieu()
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
        protected override void CapNhap_HangTongCong()
        {           
            string strDSTruongTien = "rTuChi_Phanbo,rTuChi_DaCap,rTuChi";
            string[] arrDSTruongTien = strDSTruongTien.Split(',');

            int len = arrDSTruongTien.Length;

            //Tinh lai cac hang cha
            for (int i = _dtChiTiet.Rows.Count - 1; i >= 0; i--)
            {
                string iID_MaMucLucNganSach = Convert.ToString(_dtChiTiet.Rows[i]["iID_MaMucLucNganSach"]);
                for (int k = 0; k < len; k++)
                {
                    double S;
                    S = 0;
                    double S1 = 0;
                    for (int j = i + 1; j < _dtChiTiet.Rows.Count; j++)
                    {
                        if (string.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()) && string.IsNullOrEmpty(_arrGiaTriTimKiem["iID_MaDonVi"]) && chungTu["iID_MaTinhChatCapThu"].ToString() != "1")
                        {
                            if (iID_MaMucLucNganSach == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach_Cha"]) && Convert.ToString(_dtChiTiet.Rows[j]["iID_MaDonVi"]) == "")
                            {
                                S += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien[k]]);
                            }
                            if (iID_MaMucLucNganSach == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]) && Convert.ToString(_dtChiTiet.Rows[j]["iID_MaDonVi"]) != "")
                            {
                                S1 += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien[k]]);
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
                        _dtChiTiet.Rows[i]["bLaHangCha"] = true;
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

                    if (!string.IsNullOrEmpty(Convert.ToString(arrdr[i][arrDSTruongTien[k]])))
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
            if (chungTu["iID_MaTinhChatCapThu"].ToString() != "1" && string.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString())) { 
                int count = dtChiTiet.Rows.Count - 1;
                for (int i = count; i >= 0; i--)
                {               
                    if (Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi"]) == 0 && Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_PhanBo"]) == 0 && Convert.ToDecimal(_dtChiTiet.Rows[i]["rTuChi_DaCap"]) == 0)
                        _dtChiTiet.Rows.RemoveAt(i);                
                }
            }
        }        
    }
}
