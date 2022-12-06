using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using System.Collections.Specialized;
using DomainModel;
using System.Data;
using DomainModel.Controls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace VIETTEL.Models
{
    public class BangDuLieu
    {
        public int ColHeight = 25;
        public const string DauCachHang = "#|";
        public const string DauCachO = "##";

        public string sMauSac_ChuaDuyet = WebConfigurationManager.AppSettings["ChuaDuyet"];
        public string sMauSac_TuChoi = WebConfigurationManager.AppSettings["TuChoi"];
        public string sMauSac_DongY = WebConfigurationManager.AppSettings["DongY"];

        public int Viewport_N = 50;
        protected DataTable _dtBang = null;
        protected DataTable _dtChiTiet = null;
        protected DataTable _dtChiTiet_Cu = null;
        protected List<string> _arrDSMaHang = null;
        protected List<string> _arrDSMaCot = null;
        protected List<int> _arrWidth = null;
        protected List<bool> _arrHienThiCot = null;
        protected List<bool> _arrCotDuocPhepNhap;
        protected List<string> _arrTieuDe = null;
        protected List<int> _arrSoCotCungNhom = new List<int>();
        protected List<string> _arrTieuDeNhomCot = new List<string>();
        protected int _nCotFixed = 0, _nCotSlide = 0;

        protected List<List<string>> _arrDuLieu = null;
        protected List<List<bool>> _arrThayDoi = null;
        
        protected List<string> _arrType = null;
        protected List<string> _arrFormat = null;
        protected List<string> _arrAlign = null;
        protected List<List<string>> _arrEdit = null;

        protected List<int> _arrCSCha = null;
        protected List<bool> _arrLaHangCha = null;

        protected string _iID_Ma;
        protected bool _ChiDoc = false;
        protected bool _CoCotDuyet = false;
        protected bool _DuocSuaChiTiet = false;
        protected bool _DuocSuaDuyet = false;
        protected string _MaND = "";
        protected string _IPSua = "";

        protected Dictionary<string, bool> _arrCotTienDuocHienThi;

        ~BangDuLieu()
        {
            if (_dtBang != null) _dtBang.Dispose();
            if (_dtChiTiet != null) _dtChiTiet.Dispose();
            if (_dtChiTiet_Cu != null) _dtChiTiet_Cu.Dispose();            
        }

        public DataTable dtBang { get { return _dtBang; } }
        public DataTable dtChiTiet { get { return _dtChiTiet; } }
        public bool ChiDoc { get { return _ChiDoc; } }
        public string strChiDoc
        {
            get
            {
                string vR = "0";
                if (_ChiDoc) vR = "1";
                return vR;
            }
        }
        public List<bool> arrCotDuocPhepNhap { get { return _arrCotDuocPhepNhap; } }
        public string strDSCotDuocPhepNhap
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int j = 0; j < _arrDSMaCot.Count; j++)
                {
                    if (builder.Length>0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                    if (_arrCotDuocPhepNhap == null || _arrCotDuocPhepNhap.Count <= j)
                    {
                        builder.Append(string.Format("{0}", "1")); //vR += "1";
                    }
                    else
                    {
                        if (_arrCotDuocPhepNhap[j])
                        {
                            builder.Append(string.Format("{0}", "1")); //vR += "1";
                        }
                        else
                        {
                            builder.Append(string.Format("{0}", "0")); //vR += "0";
                        }
                    }
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public bool CoCotDuyet { get { return _CoCotDuyet; } }
        public bool DuocSuaChiTiet { 
            get 
            {
                if (_ChiDoc == false && _DuocSuaChiTiet)
                {
                    return true;
                }
                return false;
            } 
        }
        public string strCoCotDuyet
        {
            get
            {
                string vR = "0";
                if (_CoCotDuyet) vR = "1";
                return vR;
            }
        }
        public List<int> arrCSCha { get { return _arrCSCha; } }
        public string strCSCha
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                if (_arrCSCha != null)
                {
                    for (int i = 0; i < _arrCSCha.Count; i++)
                    {
                        if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                        builder.Append(string.Format("{0}", _arrCSCha[i])); //vR += _arrCSCha[i];
                    }
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<bool> arrLaHangCha { get { return _arrLaHangCha; } }
        public string strLaHangCha
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                if (_arrLaHangCha != null)
                {
                    for (int i = 0; i < _arrLaHangCha.Count; i++)
                    {
                        if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                        builder.Append(string.Format("{0}", (_arrLaHangCha[i]) ? "1" : "0")); //vR += (_arrLaHangCha[i]) ? "1" : "0";
                    }
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<string> arrDSMaHang { get { return _arrDSMaHang; } }
        public string strDSMaHang
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrDSMaHang.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                    builder.Append(string.Format("{0}", _arrDSMaHang[i])); //vR += _arrDSMaHang[i];
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<string> arrDSMaCot { get { return _arrDSMaCot; } }
        public string strDSMaCot
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrDSMaCot.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                    builder.Append(string.Format("{0}", _arrDSMaCot[i])); //vR += _arrDSMaCot[i];
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<List<string>> arrDuLieu { get { return _arrDuLieu; } }
        public string strDuLieu
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrDuLieu.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", DauCachHang)); //vR += DauCachHang;
                    for (int j = 0; j < _arrDuLieu[i].Count; j++)
                    {
                        if (j > 0) builder.Append(string.Format("{0}", DauCachO)); //vR += DauCachO;
                        builder.Append(string.Format("{0}", _arrDuLieu[i][j])); //vR += _arrDuLieu[i][j];
                    }
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<string> arrType { get { return _arrType; } }
        public string strType
        {
            get
            {
                if (_arrType == null)
                {
                    CapNhap_arrType();
                }
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrType.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                    builder.Append(string.Format("{0}", _arrType[i])); //vR += _arrType[i];
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<string> arrFormat { get { return _arrFormat; } }
        public string strFormat
        {
            get
            {
                if (_arrFormat == null)
                {
                    CapNhap_arrFormat();
                }
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrFormat.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                    builder.Append(string.Format("{0}", _arrFormat[i])); //vR += _arrFormat[i];
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<List<string>> arrEdit { get { return _arrEdit; } }
        public string strEdit
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrEdit.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", DauCachHang)); //vR += DauCachHang;
                    for (int j = 0; j < _arrEdit[i].Count; j++)
                    {
                        if (j > 0) builder.Append(string.Format("{0}", DauCachO)); //vR += DauCachO;
                        if (_arrEdit[i][j] == "")
                        {
                            builder.Append(string.Format("{0}", "0")); //vR += "0";
                        }
                        else
                        {
                            builder.Append(string.Format("{0}", "1")); //vR += "1";
                        }
                    }
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public string strThayDoi
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrDuLieu.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", DauCachHang)); //vR += DauCachHang;
                    for (int j = 0; j < _arrDuLieu[i].Count; j++)
                    {
                        if (j > 0) builder.Append(string.Format("{0}", DauCachO)); //vR += DauCachO;
                        builder.Append(string.Format("{0}", _arrThayDoi[i][j] ? "1" : "0")); //vR += _arrThayDoi[i][j] ? "1" : "0";
                    }
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<int> arrWidth
        {
            get
            {
                return _arrWidth;
            }
        }
        public string strDSDoRongCot
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrDSMaCot.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                    builder.Append(string.Format("{0}", _arrWidth[i])); //vR += _arrWidth[i];
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public int Height
        {
            get
            {
                int vR = ColHeight * dtChiTiet.Rows.Count;
                return vR;
            }
        }
        public int Width
        {
            get
            {
                int vR = 0;
                for (int i = 0; i < _nCotFixed + _nCotSlide; i++)
                {
                    vR += _arrWidth[i];
                }
                return vR;
            }
        }
        public int nC_Fixed { get { return _nCotFixed; } }
        public int nC_Slide { get { return _nCotSlide; } }
        public int WidthFixed
        {
            get
            {
                int vR = 0;
                for (int i = 0; i < _nCotFixed; i++)
                {
                    if (arrHienThiCot[i])
                    {
                        vR += _arrWidth[i];
                    }
                }
                return vR;
            }
        }
        public int WidthSlide
        {
            get
            {
                int vR = 0;
                for (int i = _nCotFixed; i < _nCotFixed + _nCotSlide; i++)
                {
                    if (arrHienThiCot[i])
                    {
                        vR += _arrWidth[i];
                    }
                }
                return vR;
            }
        }
        public List<int> arrSoCotCungNhom { get { return _arrSoCotCungNhom; } }
        public List<string> arrTieuDeNhomCot { get { return _arrTieuDeNhomCot; } }
        public bool CoNhomCot_Fixed 
        {
            get
            {
                for (int j = 0; j < _nCotFixed; j++)
                {
                    if (_arrSoCotCungNhom[j] > 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool CoNhomCot_Slide
        {
            get
            {
                for (int j = _nCotFixed; j < _nCotFixed + _nCotSlide; j++)
                {
                    if (_arrSoCotCungNhom[j] > 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public List<bool> arrHienThiCot
        { 
            get 
            {
                if (_arrHienThiCot == null)
                {
                    _arrHienThiCot = new List<bool>();
                    for (int j = 0; j < arrDSMaCot.Count; j++)
                    {
                        _arrHienThiCot.Add(true);
                    }
                }
                return _arrHienThiCot; 
            } 
        }
        public string strDSHienThiCot
        {
            get
            {
                string vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < arrHienThiCot.Count; i++)
                {
                    if (i > 0) builder.Append(string.Format("{0}", ",")); //vR += ",";
                    builder.Append(string.Format("{0}", (arrHienThiCot[i]) ? "1" : "0")); //vR += (arrHienThiCot[i]) ? "1" : "0";
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public List<string> arrTieuDe
        {
            get
            {
                return _arrTieuDe;
            }
        }
        public List<string> arrAlign
        {
            get
            {
                if (_arrAlign == null)
                {
                    CapNhap_arrAlign();
                }
                return _arrAlign;
            }
        }
        /// <summary>
        /// Hàm cập nhập kiểu nhập cho các cột
        ///     - Cột có prefix 'd': kiểu '4' (datetime)
        ///     - Cột có prefix 'b': kiểu '2' (checkbox)
        ///     - Cột có prefix 'r' hoặc 'i' (trừ 'iID'): kiểu '1' (textbox number)
        ///     - Ngược lại: kiểu '0' (textbox)
        /// </summary>
        protected void CapNhap_arrType()
        {
            string[] arrDSTruongAutocomplete = ("sTenDonVi,sTenDonVi_BaoDam,iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG," +
                                                "sTenCongTrinh,sTenTaiSan,sTenPhongBan,iID_MaPhongBanDich").Split(',');
            //Xac dinh kieu truong nhap du lieu
            _arrType = new List<string>();
            string KieuNhap;
            for (int j = 0; j < _arrDSMaCot.Count; j++)
            {
                KieuNhap = "0";
                if (_arrDSMaCot[j].StartsWith("d"))
                {
                    //Nhap Kieu datetime
                    KieuNhap = "4";
                }
                if (_arrDSMaCot[j].StartsWith("b"))
                {
                    //Nhap Kieu checkbox
                    KieuNhap = "2";
                }
                else if (_arrDSMaCot[j].StartsWith("r") || (_arrDSMaCot[j].StartsWith("iID") == false && _arrDSMaCot[j].StartsWith("i")))
                {
                    //Nhap Kieu so
                    KieuNhap = "1";
                }
                else
                {
                    //Nhap kieu xau
                    for (int i = 0; i < arrDSTruongAutocomplete.Length; i++)
                    {
                        if (_arrDSMaCot[j] == arrDSTruongAutocomplete[i])
                        {
                            //Nhap Kieu autocomplete
                            KieuNhap = "3";
                            break;
                        }
                    }
                }
                _arrType.Add(KieuNhap);
            }
        }
        /// <summary>
        /// Hàm cập nhập định dạng cho các cột
        ///     - Cột có prefix 'd': 'dd/MM/yyyy'(datetime)
        ///     - Ngược lại: kiểu ''(textbox)
        /// </summary>
        protected void CapNhap_arrFormat()
        {
            //Xac dinh kieu truong nhap du lieu
            _arrFormat = new List<string>();
            string strTG;
            for (int j = 0; j < _arrDSMaCot.Count; j++)
            {
                strTG = "";
                if (_arrDSMaCot[j].StartsWith("d"))
                {
                    //Nhap Kieu datetime
                    strTG = "MM/yyyy";
                }
                _arrFormat.Add(strTG);
            }
        }
        /// <summary>
        /// Hàm cập nhập mảng căn lề cho các cột
        ///     - Cột có prefix 'b': 'center'
        ///     - Cột có prefix 'r' hoặc 'i' (trừ 'iID'): 'right'
        ///     - Ngược lại: 'left'
        ///     - Các cột 'iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG': 'right'
        /// </summary>
        protected void CapNhap_arrAlign()
        {
            //Xac dinh kieu truong nhap du lieu
            _arrAlign = new List<string>();
            for (int j = 0; j < _arrDSMaCot.Count; j++)
            {
                if (_arrDSMaCot[j].StartsWith("b"))
                {
                    //Nhap Kieu checkbox
                    _arrAlign.Add("center");
                }
                else if (_arrDSMaCot[j].StartsWith("r") || (_arrDSMaCot[j].StartsWith("iID") == false && _arrDSMaCot[j].StartsWith("i")))
                {
                    //Nhap Kieu so
                    _arrAlign.Add("right");
                }
                else if (_arrDSMaCot[j] == "sLNS")
                {
                    //LNS can trai
                    _arrAlign.Add("left");
                }
                else
                {
                    //Nhap kieu xau
                    _arrAlign.Add("left");
                }
            }
            string[] arrDSTruongChuyenSangPhai = "iID_MaDonVi,sL,sK,sM,sTM,sTTM,sNG,sTNG".Split(',');

            for (int j = 0; j < _arrDSMaCot.Count; j++)
            {
                for (int i = 0; i < arrDSTruongChuyenSangPhai.Length; i++)
                {
                    if (_arrDSMaCot[j] == arrDSTruongChuyenSangPhai[i])
                    {
                        _arrAlign[j] = "right";
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Hàm cập nhập mảng thay đổi dữ liệu
        /// </summary>
        protected void CapNhap_arrThayDoi()
        {
            _arrThayDoi = new List<List<bool>>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                _arrThayDoi.Add(new List<bool>());
                for (int j = 0; j < _arrDSMaCot.Count; j++)
                {
                    _arrThayDoi[i].Add(false);
                }
            }
        }
    }
}