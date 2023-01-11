using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace VIETTEL.Models
{
    public abstract class SheetTable
    {
        protected Dictionary<string, string> _filters;
        public Dictionary<string, string> Filters
        {
            get
            {
                return _filters ?? (_filters = new Dictionary<string, string>());
            }
        }

        #region fields

        public int ColHeight = 22;
        public const string DauCachHang = "#|";
        public const string DauCachO = "##";

        public String sMauSac_ChuaDuyet = WebConfigurationManager.AppSettings["ChuaDuyet"];
        public String sMauSac_TuChoi = WebConfigurationManager.AppSettings["TuChoi"];
        public String sMauSac_DongY = WebConfigurationManager.AppSettings["DongY"];

        public int Viewport_N = 50;
        protected DataTable dtChiTiet_Cu = null;

        protected List<String> _arrDSMaHang = null;
        protected List<int> _arrWidth = null;
        protected List<bool> _arrHienThiCot = null;
        protected List<bool> _arrCotDuocPhepNhap;
        protected List<int> _arrSoCotCungNhom = new List<int>();
        protected List<String> _arrTieuDeNhomCot = new List<string>();
        protected int _nCotFixed = 0;
        protected int _nCotSlide = 0;

        protected List<List<String>> _arrDuLieu = null;
        protected List<List<bool>> _arrThayDoi = null;

        protected List<String> _arrType = null;
        protected List<String> _arrFormat = null;
        protected List<String> _arrAlign = null;
        protected List<List<String>> _arrEdit = null;

        protected String _iID_Ma;
        protected bool _CoCotDuyet = false;
        protected bool _DuocSuaDuyet = false;
        protected String _MaND = "";
        protected String _IPSua = "";

        protected Dictionary<String, bool> _arrCotTienDuocHienThi;

        public string SheetId { get; set; }

        #endregion

        #region ctor

        public SheetTable()
        {
            arrCSCha = new List<int>();
            arrLaHangCha = new List<bool>();

            _arrHienThiCot = new List<bool>();
            arrTieuDe = new List<string>();
            arrDSMaCot = new List<string>();
            _arrWidth = new List<int>();

            SheetId = "BangDuLieu";
            ColumnNameIsParent = "bLaHangCha";
        }

        public SheetTable(bool isReadonly = true, bool parentRowEditable = false) : this()
        {
            IsReadonly = isReadonly;
            ParentRowEditable = parentRowEditable;
        }
        public SheetTable(string id_ma, Dictionary<String, String> arrGiaTriTimKiem,
                                            String MaND, String IPSua, bool isReadonly = true, bool parentRowEditable = false) : this()
        {
            IsReadonly = isReadonly;
            ParentRowEditable = parentRowEditable;
        }

        ~SheetTable()
        {
            if (dtBang != null) dtBang.Dispose();
            if (dtChiTiet != null) dtChiTiet.Dispose();
            if (dtChiTiet_Cu != null) dtChiTiet_Cu.Dispose();
        }


        #endregion

        #region properties

        public DataTable dtBang { get; protected set; }
        public DataTable dtChiTiet { get; protected set; }

        public bool IsReadonly { get; protected set; }
        public string IsReadonlyString
        {
            get
            {
                return IsReadonly ? "1" : "0";
            }
        }

        public bool ParentRowEditable { get; protected set; }

        #region columns

        public IEnumerable<SheetColumn> Columns { get { return _columns ?? (_columns = GetColumns()); } }

        public IEnumerable<SheetColumn> ColumnsSearch { get { return Columns.Where(x => x.HasSearch); } }

        public string ColumnNameIsParent { get; set; }

        public string ColumnNameParentId { get; set; }

        public string ColumnNameId { get; set; }


        private IEnumerable<SheetColumn> _columns;
        //protected virtual IEnumerable<SheetColumn> GetColumns()
        //{
        //    return new List<SheetColumn>();
        //}

        protected abstract IEnumerable<SheetColumn> GetColumns();


        #endregion


        #region arrays

        public List<bool> arrCotDuocPhepNhap { get { return _arrCotDuocPhepNhap; } }
        public string strDSCotDuocPhepNhap
        {
            get
            {
                String vR = "";
                StringBuilder builder = new StringBuilder();
                for (int j = 0; j < arrDSMaCot.Count; j++)
                {
                    if (builder.Length > 0) builder.Append(String.Format("{0}", ",")); //vR += ",";
                    if (_arrCotDuocPhepNhap == null || _arrCotDuocPhepNhap.Count <= j)
                    {
                        builder.Append(String.Format("{0}", "1")); //vR += "1";
                    }
                    else
                    {
                        if (_arrCotDuocPhepNhap[j])
                        {
                            builder.Append(String.Format("{0}", "1")); //vR += "1";
                        }
                        else
                        {
                            builder.Append(String.Format("{0}", "0")); //vR += "0";
                        }
                    }
                }
                vR = builder.ToString();
                return vR;
            }
        }
        public bool CoCotDuyet { get { return _CoCotDuyet; } }


        protected bool _isEditable = false;
        public bool IsEditable
        {
            get
            {
                //if (!IsReadonly && _isEditable)
                //{
                //    return true;
                //}
                //return false;
                return !IsReadonly;
            }
        }

        public string strCoCotDuyet
        {
            get
            {
                String vR = "0";
                if (_CoCotDuyet) vR = "1";
                return vR;
            }
        }


        public List<int> arrCSCha { get; protected set; }
        public string strCSCha { get { return arrCSCha.Join(); } }

        public List<bool> arrLaHangCha { get; protected set; }
        public string strLaHangCha
        {
            get
            {
                return arrLaHangCha.Select(x => x ? "1" : "0").Join();

            }
        }



        public List<string> arrDSMaHang { get { return _arrDSMaHang; } }
        public string strDSMaHang { get { return arrDSMaHang.Join(); } }


        public List<String> arrDSMaCot { get; protected set; }
        public String strDSMaCot { get { return arrDSMaCot.Join(); } }


        public List<List<String>> arrDuLieu { get { return _arrDuLieu; } }
        public String strDuLieu
        {
            get
            {
                return arrDuLieu.Select(x => x.Join(DauCachO)).Join(DauCachHang);
            }
        }

        public List<String> arrType { get { return _arrType; } }
        public String strType
        {
            get
            {
                if (_arrType == null)
                {
                    CapNhap_arrType();
                }

                return arrType.Join();
            }
        }
        public List<String> arrFormat { get { return _arrFormat; } }

        public String strFormat
        {
            get
            {
                if (_arrFormat == null)
                {
                    CapNhap_arrFormat();
                }

                return arrFormat.Join();
            }
        }


        public List<List<String>> arrEdit { get { return _arrEdit; } }

        public String strEdit
        {
            get
            {
                return arrEdit.Select(x => x.Select(y => string.IsNullOrWhiteSpace(y) ? "0" : "1").Join(DauCachO))
                                .Join(DauCachHang);
            }
        }


        public String strThayDoi
        {
            get
            {
                String vR = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _arrDuLieu.Count; i++)
                {
                    if (i > 0) builder.Append(String.Format("{0}", DauCachHang)); //vR += DauCachHang;
                    for (int j = 0; j < _arrDuLieu[i].Count; j++)
                    {
                        if (j > 0) builder.Append(String.Format("{0}", DauCachO)); //vR += DauCachO;
                        builder.Append(String.Format("{0}", _arrThayDoi[i][j] ? "1" : "0")); //vR += _arrThayDoi[i][j] ? "1" : "0";
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

        public string strDSDoRongCot { get { return arrWidth.Join(); } }

        public int Height { get { return ColHeight * dtChiTiet.Rows.Count; } }
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
                int r = 0;
                for (int i = 0; i < _nCotFixed; i++)
                {
                    if (arrHienThiCot[i])
                    {
                        r += _arrWidth[i];
                    }
                }
                return r;
            }
        }
        public int WidthSlide
        {
            get
            {
                int r = 0;
                for (int i = _nCotFixed; i < _nCotFixed + _nCotSlide; i++)
                {
                    if (arrHienThiCot[i])
                    {
                        r += _arrWidth[i];
                    }
                }
                return r;
            }
        }
        public List<int> arrSoCotCungNhom { get { return _arrSoCotCungNhom; } }
        public List<String> arrTieuDeNhomCot { get { return _arrTieuDeNhomCot; } }
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

        public string strDSHienThiCot { get { return arrHienThiCot.Select(x => x.ToStringBool()).Join(); } }


        public List<String> arrTieuDe { get; protected set; }
        public List<String> arrAlign { get { return _arrAlign ?? CapNhap_arrAlign(); } }

        #endregion

        #endregion


        /// <summary>
        /// Hàm cập nhập kiểu nhập cho các cột
        ///     - Cột có prefix 'd': kiểu '4' (datetime)
        ///     - Cột có prefix 'b': kiểu '2' (checkbox)
        ///     - Cột có prefix 'r' hoặc 'i' (trừ 'iID'): kiểu '1' (textbox number)
        ///     - Ngược lại: kiểu '0' (textbox)
        /// </summary>
        //protected virtual void CapNhap_arrType()
        //{
        //    String[] arrDSTruongAutocomplete = ("sTenDonVi,sTenDonVi_BaoDam,iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG," +
        //                                        "sTenCongTrinh,sTenTaiSan,sTenPhongBan,iID_MaPhongBanDich").Split(',');
        //    //Xac dinh kieu truong nhap du lieu
        //    _arrType = new List<string>();
        //    String KieuNhap;
        //    for (int j = 0; j < arrDSMaCot.Count; j++)
        //    {
        //        KieuNhap = "0";
        //        if (arrDSMaCot[j].StartsWith("d"))
        //        {
        //            //Nhap Kieu datetime
        //            KieuNhap = "4";
        //        }
        //        if (arrDSMaCot[j].StartsWith("b"))
        //        {
        //            //Nhap Kieu checkbox
        //            KieuNhap = "2";
        //        }
        //        else if (arrDSMaCot[j].StartsWith("r") || (arrDSMaCot[j].StartsWith("iID") == false && arrDSMaCot[j].StartsWith("i")))
        //        {
        //            //Nhap Kieu so
        //            KieuNhap = "1";
        //        }
        //        else
        //        {
        //            //Nhap kieu xau
        //            for (int i = 0; i < arrDSTruongAutocomplete.Length; i++)
        //            {
        //                if (arrDSMaCot[j] == arrDSTruongAutocomplete[i])
        //                {
        //                    //Nhap Kieu autocomplete
        //                    KieuNhap = "3";
        //                    break;
        //                }
        //            }
        //        }
        //        _arrType.Add(KieuNhap);
        //    }
        //}

        protected virtual void CapNhap_arrType()
        {
            _arrType = new List<string>();
            Columns.ToList().ForEach(c =>
            {
                _arrType.Add(c.DataType.ToString());
            });
        }

        /// <summary>
        /// Hàm cập nhập định dạng cho các cột
        ///     - Cột có prefix 'd': 'dd/MM/yyyy'(datetime)
        ///     - Ngược lại: kiểu ''(textbox)
        /// </summary>
        protected virtual void CapNhap_arrFormat()
        {
            //Xac dinh kieu truong nhap du lieu
            _arrFormat = new List<string>();
            Columns.ToList().ForEach(c =>
            {
                _arrFormat.Add(c.Format.ToString());
            });


            //for (int j = 0; j < arrDSMaCot.Count; j++)
            //{
            //    var strTG = "";
            //    if (arrDSMaCot[j].StartsWith("d"))
            //    {
            //        //Nhap Kieu datetime
            //        strTG = "MM/yyyy";
            //    }
            //    _arrFormat.Add(strTG);
            //}


        }
        /// <summary>
        /// Hàm cập nhập mảng căn lề cho các cột
        ///     - Cột có prefix 'b': 'center'
        ///     - Cột có prefix 'r' hoặc 'i' (trừ 'iID'): 'right'
        ///     - Ngược lại: 'left'
        ///     - Các cột 'iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG': 'right'
        /// </summary>
        /// 
        protected virtual List<string> CapNhap_arrAlign()
        {
            //Xac dinh kieu truong nhap du lieu
            var items = new List<string>();
            Columns.ToList().ForEach(c =>
            {
                items.Add(c.Align.ToString());
            });


            //for (int j = 0; j < arrDSMaCot.Count; j++)
            //{
            //    if (arrDSMaCot[j].StartsWith("b"))
            //    {
            //        //Nhap Kieu checkbox
            //        items.Add("center");
            //    }
            //    else if (arrDSMaCot[j].StartsWith("r") || (arrDSMaCot[j].StartsWith("iID") == false && arrDSMaCot[j].StartsWith("i")))
            //    {
            //        //Nhap Kieu so
            //        items.Add("right");
            //    }
            //    else if (arrDSMaCot[j] == "sLNS")
            //    {
            //        //LNS can trai
            //        items.Add("left");
            //    }
            //    else
            //    {
            //        //Nhap kieu xau
            //        items.Add("left");
            //    }
            //}
            //String[] arrDSTruongChuyenSangPhai = "iID_MaDonVi,sL,sK,sM,sTM,sTTM,sNG,sTNG".Split(',');

            //for (int j = 0; j < arrDSMaCot.Count; j++)
            //{
            //    for (int i = 0; i < arrDSTruongChuyenSangPhai.Length; i++)
            //    {
            //        if (arrDSMaCot[j] == arrDSTruongChuyenSangPhai[i])
            //        {
            //            //items[j] = "right";
            //            items[j] = "center";
            //            break;
            //        }
            //    }
            //}

            return items;
        }

        #region longsam add features

        protected virtual void updateColumnsParent()
        {
            //Xác định hàng là hàng cha, con
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                arrLaHangCha.Add(Convert.ToBoolean(r[ColumnNameIsParent]));


                int parent = -1;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Convert.ToString(r[ColumnNameParentId]) ==
                        Convert.ToString(dtChiTiet.Rows[j][ColumnNameId]))
                    {
                        parent = j;
                        break;
                    }
                }
                arrCSCha.Add(parent);
            }
        }

        protected virtual void updateCellsEditable()
        {
            _arrEdit = new List<List<string>>();

            dtChiTiet.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var isReadonlyRow = false;

                    //if (!string.IsNullOrWhiteSpace(ColumnNameIsParent) && Convert.ToBoolean(r[ColumnNameIsParent]) && !ParentRowEditable)
                    if (!string.IsNullOrWhiteSpace(ColumnNameIsParent) && Convert.ToBoolean(r[ColumnNameIsParent].ToString()) && !ParentRowEditable)
                    {
                        isReadonlyRow = true;
                    }

                    var items = new List<string>();
                    Columns.ToList()
                    .ForEach(c =>
                    {
                        if (IsEditable && !isReadonlyRow)
                        {
                            items.Add(c.IsReadonly ? "" : "1");
                        }
                        else
                        {
                            items.Add("");
                        }
                    });

                    _arrEdit.Add(items);
                });
        }

        protected virtual void updateCellsValue()
        {
            _arrDuLieu = new List<List<string>>();

            var columnsName = dtChiTiet.GetColumnNames();
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                var items = new List<string>();

                arrDSMaCot.ToList().ForEach(c =>
                    {
                        items.Add(getCellValue(r, i, c, columnsName));
                    });
                _arrDuLieu.Add(items);

            }
        }

        protected virtual string getCellValue(DataRow r, int i, string c, IEnumerable<string> columnsName)
        {
            try
            {
                if (columnsName.Contains(c))
                {
                    var cell = r[c].ToString();
                    return cell;
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region columns

        //protected virtual void BuildColumns(IEnumerable<SheetColumn> columns)
        //{
        //    _columns = columns;
        //}

        protected virtual void updateColumnIDs(string columns)
        {
            _arrDSMaHang = new List<string>();

            var fields = columns.Split(',').ToList();
            dtChiTiet.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var id = !string.IsNullOrWhiteSpace(ColumnNameIsParent) && r.Field<bool>(ColumnNameIsParent) && !ParentRowEditable ? "" : fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    _arrDSMaHang.Add(id);
                });
        }

        protected virtual void updateColumnIDsCT(string columns)
        {
            _arrDSMaHang = new List<string>();
            var fields = columns.Split(',').ToList();
            var numRow = dtChiTiet.Rows.Count;
            dtChiTiet.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var id = !string.IsNullOrWhiteSpace(ColumnNameIsParent) && r.Field<bool>(ColumnNameIsParent) && !ParentRowEditable ? "" : fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    if (numRow ==  1 && ( id == "" || id == null))
                    {
                        id = Guid.Empty.ToString();
                    }
                    _arrDSMaHang.Add(id);
                });
        }


        //protected virtual void updateColumnsFixed()
        //{
        //    var columns = _columns.Where(x => x.IsFixed && !x.IsHidden).ToList();
        //    columns.ForEach(addColumn);


        //    _nCotFixed = columns.Count;
        //}

        //protected virtual void updateColumnsSlide()
        //{
        //    var columns = _columns.Where(x => !x.IsFixed && !x.IsHidden).ToList();
        //    columns.ForEach(addColumn);

        //    _nCotSlide = columns.Count;
        //}


        protected virtual void updateColumns()
        {
            // fixed
            var columns = Columns.Where(x => x.IsFixed && !x.IsHidden).ToList();
            columns.ForEach(addColumn);


            _nCotFixed = columns.Count;

            // slide
            columns = Columns.Where(x => !x.IsFixed && !x.IsHidden).ToList();
            columns.ForEach(addColumn);

            _nCotSlide = columns.Count;

            // hidden
            columns = Columns.Where(x => x.IsHidden).ToList();
            columns.ForEach(addColumn);
        }


        private void addColumn(SheetColumn c)
        {
            arrDSMaCot.Add(c.ColumnName);
            arrTieuDe.Add(c.Header);
            arrWidth.Add(c.ColumnWidth);

            arrHienThiCot.Add(!c.IsHidden);


            arrSoCotCungNhom.Add(c.HeaderGroupIndex);
            arrTieuDeNhomCot.Add(c.HeaderGroup);
        }
        #endregion

        /// <summary>
        /// Hàm cập nhập mảng thay đổi dữ liệu
        /// </summary>
        protected void updateChanges()
        {
            _arrThayDoi = new List<List<bool>>();
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var row = dtChiTiet.Rows[i];
                var items = new List<bool>();
                for (int j = 0; j < arrDSMaCot.Count; j++)
                {
                    items.Add(false);
                }

                _arrThayDoi.Add(items);
            }
        }

        /// <summary>
        /// Tính tổng cho các hàng cha tại các cột là number
        /// </summary>
        protected void updateSummaryRows()
        {
            var columns = Columns.Where(x => x.DataType == 1).ToList();

            //Tinh lai cac hang cha
            for (int i = dtChiTiet.Rows.Count - 1; i >= 0; i--)
            {
                var r = dtChiTiet.Rows[i];
                var isParent = r.Field<bool>(ColumnNameIsParent);

                if (isParent)
                {
                    var id = r[ColumnNameId].ToString();

                    columns.ToList()
                               .ForEach(c =>
                               {
                                   var sum = 0d;

                                   for (int j = i + 1; j < dtChiTiet.Rows.Count; j++)
                                   {
                                       var r1 = dtChiTiet.Rows[j];
                                       if (id == r1[ColumnNameParentId].ToString() && !string.IsNullOrWhiteSpace(r1[c.ColumnName].ToString()))
                                       {
                                           sum += double.Parse(r1[c.ColumnName].ToString());
                                       }
                                   }

                                   r[c.ColumnName] = sum;

                               });
                }
            }

        }


        protected void insertSummaryRows(string columnName)
        {
            var sumText = "TỔNG CỘNG";

            var exist = dtChiTiet.Rows.Count > 0 &&
                dtChiTiet.Rows[0][columnName] == sumText;

            if (exist) return;


            var columns = Columns.Where(x => x.DataType == 1).ToList();

            // add summary rows

            var sumFirstRow = dtChiTiet.NewRow();
            var sumlastRow = dtChiTiet.NewRow();
            var childRows = dtChiTiet.AsEnumerable()
                    .ToList()
                    .Where(x => !x.Field<bool>(ColumnNameIsParent));
            columns.ToList()
                .ForEach(c =>
                {

                    var sum = childRows.Sum(x => x[c.ColumnName].ToString() == "" ? 0 : double.Parse(x[c.ColumnName].ToString()));
                    sumFirstRow[c.ColumnName] = sum;
                    sumlastRow[c.ColumnName] = sum;

                });

            sumFirstRow[ColumnNameIsParent] = true;
            sumFirstRow[columnName] = sumText;

            sumlastRow[ColumnNameIsParent] = true;
            sumlastRow[columnName] = sumText;

            dtChiTiet.Rows.InsertAt(sumFirstRow, 0);
            dtChiTiet.Rows.Add(sumlastRow);
        }

        protected virtual void filterParentsRow(DataTable dt, string checkedColumn = "IsChecked")
        {
            // trong truong hop cache, hoac da check thi ko check lai nua
            if (!dt.GetColumnNames().Contains(checkedColumn))
            {
                dt.Columns.Add(new DataColumn()
                {
                    DefaultValue = false,
                    ColumnName = checkedColumn,
                    DataType = typeof(bool)
                });
            }

            var items = new List<DataRow>();
            // loc cac hang la cha nhung ko co nhanh
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var row = dt.Rows[i];
                var isParent = row.Field<bool>(ColumnNameIsParent);
                if (i == dt.Rows.Count - 1 && isParent)
                {
                    items.Add(row);
                }
                else
                {
                    for (int j = i; j < dt.Rows.Count; j++)
                    {
                        var rowChild = dt.Rows[j];
                        if (!rowChild.Field<bool>(ColumnNameIsParent))
                        {
                            rowChild[checkedColumn] = true;
                        }

                        if (row[ColumnNameId].ToString() == rowChild[ColumnNameParentId].ToString() && rowChild.Field<bool>(checkedColumn))
                        {
                            row[checkedColumn] = true;
                            break;
                        }

                    }

                }
            }

            items = dt.AsEnumerable().Where(x => !x.Field<bool>(checkedColumn)).ToList();
            items.ForEach(dt.Rows.Remove);
        }

        #endregion
    }


    public class SheetColumn
    {
        public const string AlignCenter = "center";
        public const string AlignRight = "right";
        public const string AlignLeft = "left";


        public string ColumnName { get; set; }

        public string Header { get; set; }

        public string HeaderGroup { get; set; }

        public int HeaderGroupIndex { get; set; }


        public int ColumnWidth { get; set; }


        /// <summary>
        /// Xác định kiểu dữ liệu của cột nhập, trong đó:
        /// 0:  String
        /// 1:  Number
        /// 2:  Bool
        /// 3:  Autocomplete
        /// 4:  Datetime
        /// 5:  AutocompleteMultiple
        /// </summary>
        public int DataType { get; set; }

        public string Align { get; set; }
        public string Format { get; set; }

        public bool IsHidden { get; set; }

        public bool IsReadonly { get; set; }

        public bool IsFixed { get; set; }

        public bool HasSearch { get; set; }

        public SheetColumn()
        {

        }
        public SheetColumn(
            string columnName,
            string header = "",
            string headerGroup = "",
            int headerGroupIndex = 1,
            int columnWidth = 0,
            int dataType = 0,
            string align = AlignLeft,
            string format = "",
            bool isHidden = false,
            bool isReadonly = true,
            bool isFixed = false,
            bool hasSearch = false)
        {
            ColumnName = columnName;
            Header = header;
            HeaderGroup = headerGroup;
            HeaderGroupIndex = headerGroupIndex;
            ColumnWidth = columnWidth;
            DataType = dataType;
            Align = align;
            IsHidden = isHidden;
            IsReadonly = isReadonly;
            IsFixed = isFixed;
            HasSearch = hasSearch;
            Format = format;


            if (string.IsNullOrWhiteSpace(Format))
            {
                if (DataType == 4)
                {
                    format = "dd/MM/yyyy";
                }
            }

            if (DataType == 1)
            {
                Align = AlignRight;
            }
        }

    }


}
