using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Models
{
    public class QuyetToan_ChungTu_SheetTableFilterType
    {
        public const int All = 0;
        public const int CoChiTieu = 1;
        public const int KhongCoChiTieu = 2;
        public const int CoQuyetToan = 3;
        public const int CoQuyetToanKhacDeNghi = 4;

        private static Dictionary<int, string> _items;
        public static Dictionary<int, string> Items
        {
            get
            {
                return _items ?? (_items = new Dictionary<int, string>()
                {
                    { All, "Tất cả"},
                    { CoChiTieu, "Có chỉ tiêu"},
                    { KhongCoChiTieu, "Không có chi tiêu"},
                    { CoQuyetToan, "Có quyết toán"},
                    { CoQuyetToanKhacDeNghi, "Quyết toán khác đề nghị"},

                });
            }
        }
    }

    /// <summary>
    /// Lớp QuyetToan_ChungTu_BangDuLieu cho phần bảng của Quyết toán
    /// </summary>
    public class QuyetToan_ChungTu_SheetTable : SheetTable
    {


        public QuyetToan_ChungTu_SheetTable()
        {

        }

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        public QuyetToan_ChungTu_SheetTable(String iID_MaChungTu, Dictionary<String, String> arrGiaTriTimKiem,
                                            String MaND, String IPSua, int option)
        {
            this._iID_Ma = iID_MaChungTu;
            this._MaND = MaND;
            this._IPSua = IPSua;

            var qtService = QuyetToanService.Default;

            var entity = qtService.GetChungTu(Guid.Parse(iID_MaChungTu));
            var iQuy = entity.iThang_Quy;
            var iID_MaTrangThaiDuyet = entity.iID_MaTrangThaiDuyet;

            //String SQL;
            //SqlCommand cmd;
            //SQL = "SELECT * FROM QTA_ChungTu WHERE iID_MaChungTu=@iID_MaChungTu AND iTrangThai=1";
            //cmd = new SqlCommand();
            //cmd.Parameters.AddWithValue("@iID_MaChungTu", _iID_Ma);
            //cmd.CommandText = SQL;
            //dtBang = Connection.GetDataTable(cmd);
            //cmd.Dispose();

            //String iQuy = Convert.ToString(dtBang.Rows[0]["iThang_Quy"]);
            //int iID_MaTrangThaiDuyet = Convert.ToInt32(dtBang.Rows[0]["iID_MaTrangThaiDuyet"]);



            var canEdit =
                LuongCongViecModel.NguoiDung_DuocSuaChungTu(QuyetToanModels.iID_MaPhanHeQuyetToan, MaND,
                                                            iID_MaTrangThaiDuyet);
            if (LuongCongViecModel.KiemTra_TrangThaiDaDuyet(QuyetToanModels.iID_MaPhanHeQuyetToan, iID_MaTrangThaiDuyet))
            {
                IsReadonly = true;
            }

            if (canEdit == false)
            {
                IsReadonly = true;
            }

            if (
                LuongCongViecModel.KiemTra_TrangThaiTrinhDuyet(QuyetToanModels.iID_MaPhanHeQuyetToan,
                                                               iID_MaTrangThaiDuyet) &&
                canEdit)
            {
                _CoCotDuyet = true;
                _DuocSuaDuyet = true;
            }

            if (LuongCongViecModel.KiemTra_TrangThaiTuChoi(QuyetToanModels.iID_MaPhanHeQuyetToan, iID_MaTrangThaiDuyet))
            {
                _CoCotDuyet = true;
            }

            _isEditable = LuongCongViecModel.NguoiDung_DuocThemChungTu(QuyetToanModels.iID_MaPhanHeQuyetToan, MaND);

            //dtChiTiet = QuyetToan_ChungTuChiTietModels.Get_dtQuyetToanChiTiet(_iID_Ma, MaND, arrGiaTriTimKiem);
            dtChiTiet = createTable_QuyetToanCache(entity).Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();


            ColumnNameId = "iID_MaMucLucNganSach";
            ColumnNameParentId = "iID_MaMucLucNganSach_Cha";
            ColumnNameIsParent = "bLaHangCha";

            fillterData(dtChiTiet, option);
            fillData(iQuy);

        }

        public QuyetToan_ChungTu_SheetTable(string id, int option, string username, Dictionary<string, string> filters)
        {
            fillSheet(id, option, username, filters);

        }

        public QuyetToan_ChungTu_SheetTable(string id, int option, string username, NameValueCollection query)
        {
            var filters = new Dictionary<string, string>();

            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query[c.ColumnName]);
                });
            fillSheet(id, option, username, filters);
        }

        private void fillSheet(string id, int option, string username, Dictionary<string, string> filters)
        {
            var qtService = QuyetToanService.Default;

            var entity = qtService.GetChungTu(Guid.Parse(id));
            var iQuy = entity.iThang_Quy;
            var iID_MaTrangThaiDuyet = entity.iID_MaTrangThaiDuyet;



            var canEdit =
                LuongCongViecModel.NguoiDung_DuocSuaChungTu(QuyetToanModels.iID_MaPhanHeQuyetToan, username,
                                                            iID_MaTrangThaiDuyet);
            if (LuongCongViecModel.KiemTra_TrangThaiDaDuyet(QuyetToanModels.iID_MaPhanHeQuyetToan, iID_MaTrangThaiDuyet))
            {
                IsReadonly = true;
            }

            if (canEdit == false)
            {
                IsReadonly = true;
            }

            //if (
            //    LuongCongViecModel.KiemTra_TrangThaiTrinhDuyet(QuyetToanModels.iID_MaPhanHeQuyetToan,
            //                                                   iID_MaTrangThaiDuyet) &&
            //    canEdit)
            //{
            //    _CoCotDuyet = true;
            //    _DuocSuaDuyet = true;
            //}

            //if (LuongCongViecModel.KiemTra_TrangThaiTuChoi(QuyetToanModels.iID_MaPhanHeQuyetToan, iID_MaTrangThaiDuyet))
            //{
            //    _CoCotDuyet = true;
            //}

            //_isEditable = LuongCongViecModel.NguoiDung_DuocThemChungTu(QuyetToanModels.iID_MaPhanHeQuyetToan, username);


            _filters = filters ?? new Dictionary<string, string>();
            dtChiTiet = createTable_QuyetToanCache(entity).Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();


            ColumnNameId = "iID_MaMucLucNganSach";
            ColumnNameParentId = "iID_MaMucLucNganSach_Cha";
            ColumnNameIsParent = "bLaHangCha";

            fillterData(dtChiTiet, option);
            fillData(iQuy);
        }

        /// <summary>
        /// Hàm điền dữ liệu
        /// Mục đích: Điền tất cả thông tin vào các tham số của đối tượng Bảng dữ liệu
        /// </summary>
        protected void fillData(int iQuy)
        {
            if (_arrDuLieu != null) return;

            #region old
            //if (MaLoai == "ChiTieu")
            //{
            //    int count = dtChiTiet.Rows.Count - 1;
            //    CapNhapHangTongCong();
            //    for (int i = count; i >= 0; i--)
            //    {
            //        if ((String.IsNullOrEmpty(Convert.ToString(dtChiTiet.Rows[i]["rChiTieu"])) || Convert.ToDecimal(dtChiTiet.Rows[i]["rChiTieu"]) <= 0))
            //            dtChiTiet.Rows.RemoveAt(i);
            //    }

            //}

            //else if (MaLoai == "KhongChiTieu")
            //{
            //    int count = dtChiTiet.Rows.Count - 1;
            //    for (int i = count; i >= 0; i--)
            //    {
            //        if (!String.IsNullOrEmpty(Convert.ToString(dtChiTiet.Rows[i]["rChiTieu"])))
            //        {
            //            if (Convert.ToDecimal(dtChiTiet.Rows[i]["rChiTieu"]) > 0 && Convert.ToBoolean(dtChiTiet.Rows[i]["bLaHangCha"]) == false)
            //                dtChiTiet.Rows.RemoveAt(i);
            //        }
            //    }
            //    CapNhapHangTongCong();
            //}
            //else if (MaLoai == "DonViDeNghiKhac")
            //{
            //    int count = dtChiTiet.Rows.Count - 1;
            //    CapNhapHangTongCong();
            //    for (int i = count; i >= 0; i--)
            //    {
            //        if (!String.IsNullOrEmpty(Convert.ToString(dtChiTiet.Rows[i]["rTuChi"])))
            //        {
            //            if (Convert.ToDecimal(dtChiTiet.Rows[i]["rTuChi"]) - Convert.ToDecimal(dtChiTiet.Rows[i]["rDonViDeNghi"]) == 0)
            //                dtChiTiet.Rows.RemoveAt(i);
            //        }
            //    }

            //}
            //else if (MaLoai == "QuyetToan")
            //{
            //    int count = dtChiTiet.Rows.Count - 1;
            //    CapNhapHangTongCong();
            //    for (int i = count; i >= 0; i--)
            //    {
            //        if ((String.IsNullOrEmpty(Convert.ToString(dtChiTiet.Rows[i]["rTuChi"])) || Convert.ToDecimal(dtChiTiet.Rows[i]["rTuChi"]) <= 0))
            //            dtChiTiet.Rows.RemoveAt(i);
            //    }
            //}
            //else
            //{
            //    CapNhapHangTongCong();
            //}





            //String SQL = "UPDATE QTA_ChungTu SET MaLoai='" + MaLoai + "' WHERE iID_MaChungTu='" + _iID_Ma + "'";
            //SqlCommand cmd = new SqlCommand(SQL);
            //Connection.UpdateDatabase(cmd);

            //CapNhapDanhSachMaHang();
            //CapNhapDanhSachMaCot_Fixed(iQuy);
            //CapNhapDanhSachMaCot_Slide(iQuy);
            //CapNhapDanhSachMaCot_Them();
            //CapNhap_arrLaHangCha();
            //CapNhap_arrEdit();
            //CapNhap_arrDuLieu();
            //updateChanges();
            #endregion

            //var columns = Columns;
            //BuildColumns(columns);

            updateSummaryRows();
            insertSummaryRows("sMoTa");

            updateColumnIDs("iID_MaChungTuChiTiet,iID_MaMucLucNganSach");
            updateColumns();
            updateColumnsParent();
            updateCellsEditable();

            //CapNhap_arrEdit();
            //CapNhap_arrDuLieu();
            updateCellsValue();
            updateChanges();

            //Khong ghi cac hang cha
            //for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            //{
            //    DataRow R = dtChiTiet.Rows[i];
            //    if (Convert.ToBoolean(R["bLaHangCha"]))
            //    {
            //        _arrDSMaHang[i] = "";
            //    }
            //}
        }

        #region Columns

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            return new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "sLNS", header: "LNS", columnWidth:80, isFixed: true, hasSearch: true),
                    new SheetColumn(columnName: "sL", header: "L", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sK", header: "K", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sM", header: "M", columnWidth:50, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sTM", header: "TM", columnWidth:50, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sTTM", header: "TTM", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sNG", header: "NG", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:280, isFixed: true),
                    new SheetColumn(columnName: "rChiTieu", header: "Chi tiêu ngân sách", columnWidth:140, isFixed: true,  dataType: 1),
                    new SheetColumn(columnName: "rDaQuyetToan", header: "Đã quyết toán", columnWidth:140, isFixed: true,  dataType: 1),

                    new SheetColumn(columnName: "rDonViDeNghi", header: "Đơn vị đề nghị", columnWidth:120, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "rTuChi", header: "Số quyết toán duyệt", columnWidth:120, dataType: 1, isReadonly: false),

                    //new SheetColumn(columnName: "rVuotChiTieu", header: "Vượt chỉ tiêu", headerGroup:"Đề nghị bộ xử lý", headerGroupIndex:2, columnWidth:120, isFixed: true, dataType: 1),
                    //new SheetColumn(columnName: "rTonThatTonDong", header: "Tổn thất-tồn đọng,", headerGroup:"Đề nghị bộ xử lý", headerGroupIndex: 2, columnWidth:120, isFixed: true, dataType: 1),
                    //new SheetColumn(columnName: "rDaCapTien", header: "Đã cấp tiền", headerGroup:"Chi tiêu ngân sách", headerGroupIndex: 3, columnWidth:120, isFixed: true, dataType: 1),
                    //new SheetColumn(columnName: "rChuaCapTien", header: "Chưa cấp tiền",headerGroup:"Chi tiêu ngân sách", headerGroupIndex: 3, columnWidth:120, isFixed: true, dataType: 1),
                    //new SheetColumn(columnName: "sGhiChu", header: "Ghi chú",headerGroup:"Chi tiêu ngân sách", headerGroupIndex: 3, columnWidth:300, isFixed: true, dataType: 1),

                    // cot khac
                    new SheetColumn(columnName: "iID_MaMucLucNganSach", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };
        }

        #endregion

        protected override string getCellValue(DataRow r, int i, string c, IEnumerable<string> columnsName)
        {
            if (i == 0 || i == dtChiTiet.Rows.Count - 1)
            {
                if (c == "sMauSac")
                {
                    return "LightGray";
                }
                else if (c == "sFontColor")
                {
                    return "OrangeRed";
                }
                else if (c == "sFontBold")
                {
                    return "bold";
                }
            }
            else if (r.Field<bool>(ColumnNameIsParent))
            {
                if (c == "sMauSac")
                {
                    return "#EFEFEF";
                }
            }

            return base.getCellValue(r, i, c, columnsName);
        }

        #region private methods

        private void fillterData(DataTable dt, int loai)
        {
            if (loai == QuyetToan_ChungTu_SheetTableFilterType.CoChiTieu)
            {
                dt.FilterRow(r => r.Field<bool>(ColumnNameIsParent) || (r["rChiTieu"] != DBNull.Value && r.Field<decimal>("rChiTieu") != 0));
            }
            else if (loai == QuyetToan_ChungTu_SheetTableFilterType.KhongCoChiTieu)
            {
                dt.FilterRow(r => r.Field<bool>(ColumnNameIsParent) || (r["rChiTieu"] == DBNull.Value || r.Field<decimal>("rChiTieu") == 0));
            }
            else if (loai == QuyetToan_ChungTu_SheetTableFilterType.CoQuyetToan)
            {
                dt.FilterRow(r => !r.Field<bool>(ColumnNameIsParent) && (r["rChiTieu"] == DBNull.Value || r.Field<decimal>("rChiTieu") == 0));
            }
            else if (loai == QuyetToan_ChungTu_SheetTableFilterType.CoQuyetToanKhacDeNghi)
            {
                dt.FilterRow(r => !r.Field<bool>(ColumnNameIsParent) && (r["rChiTieu"] == DBNull.Value || r.Field<decimal>("rChiTieu") == 0));
            }

            filterParentsRow(dt);
        }


        private DataTable createTable_QuyetToanCache(QTA_ChungTu chungtu)
        {
#if DEBUG
            return createTable_QuyetToan(chungtu);
#else
             var cacheKey = $"quyetoan_{chungtu.iID_MaChungTu}";
             return CacheService.Default.CachePerRequest(cacheKey, () => createTable_QuyetToan(chungtu), CacheTimes.OneMinute);
#endif

        }

        private DataTable createTable_QuyetToan(QTA_ChungTu chungtu)
        {

            //var dt = getTable_QuyetToanChiTiet(chungtu);

            //var dtDaQuyetToan = getTable_DaQuyetToan(chungtu);
            //dt = mergeTables(dt, dtDaQuyetToan, "sXauNoiMa,iID_MaDonVi", "rDaQuyetToan");

            //var dtChiTieu = getTable_ChiTieu(chungtu);
            //dt = mergeTables(dt, dtChiTieu, "sXauNoiMa,iID_MaDonVi", "rChiTieu");

            var dt = getTable_QuyetToanAll(chungtu);
            return dt;
        }
        //private DataTable createTable_QuyetToan(QTA_ChungTu chungtu)
        //{
        //    var dt = getTable_QuyetToanChiTiet(chungtu);

        //    var dtDaQuyetToan = getTable_DaQuyetToan(chungtu);
        //    dt = mergeTables(dt, dtDaQuyetToan, "sXauNoiMa,iID_MaDonVi", "rDaQuyetToan");

        //    var dtChiTieu = getTable_ChiTieu(chungtu);
        //    dt = mergeTables(dt, dtChiTieu, "sXauNoiMa,iID_MaDonVi", "rChiTieu");


        //    return dt;
        //}

        private DataTable getTable_QuyetToanAll(QTA_ChungTu chungtu)
        {
            var sql = FileHelpers.GetSqlQuery("qt_table_all.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    chungtu.iNamLamViec,
                    chungtu.iID_MaChungTu,
                    chungtu.iID_MaNguonNganSach,
                    chungtu.iID_MaNamNganSach,
                    //chungtu.dNgayChungTu,
                    //dNgayChungTu = DateTime.Now.Date.AddDays(1),
                    dNgayChungTu = DBNull.Value,
                    chungtu.iThang_Quy,
                    chungtu.iID_MaPhongBan,
                    iID_MaDonVi = chungtu.iID_MaDonVi,
                    sLNS1 = string.IsNullOrWhiteSpace(chungtu.sDSLNS)
                        ? (chungtu.iLoai == 2 ? "2,3" : chungtu.iLoai.ToString())
                        : chungtu.sDSLNS.ToParamString(),
                });

                cmdAddParams(cmd, _filters);
                return cmd.GetTable();

            }
        }

        private DataTable getTable_QuyetToanChiTiet(QTA_ChungTu chungtu)
        {
            var sql = FileHelpers.GetSqlQuery("qt_table.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    chungtu.iNamLamViec,
                    chungtu.iID_MaChungTu,
                    chungtu.iID_MaPhongBan,
                    chungtu.iID_MaDonVi,
                    //sLNS = string.IsNullOrWhiteSpace(chungtu.sDSLNS) ? chungtu.iLoai.ToString() : chungtu.sDSLNS.ToParamString(),
                    sLNS = string.IsNullOrWhiteSpace(chungtu.sDSLNS)
                        ? (chungtu.iLoai == 2 ? "2,3" : chungtu.iLoai.ToString())
                        : chungtu.sDSLNS.ToParamString(),
                });

                return cmd.GetTable();

            }
        }

        private DataTable getTable_DaQuyetToan(QTA_ChungTu chungtu)
        {
            var sql = FileHelpers.GetSqlQuery("qt_table_daquyettoan.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    chungtu.iNamLamViec,
                    chungtu.iID_MaNguonNganSach,
                    chungtu.iID_MaNamNganSach,
                    chungtu.dNgayChungTu,
                    chungtu.iThang_Quy,
                    chungtu.iID_MaPhongBan,
                    iID_MaDonVi = chungtu.iID_MaDonVi,
                    sLNS1 = string.IsNullOrWhiteSpace(chungtu.sDSLNS)
                        ? (chungtu.iLoai == 2 ? "2,3" : chungtu.iLoai.ToString())
                        : chungtu.sDSLNS.ToParamString(),
                });

                cmdAddParams(cmd, _filters);
                return cmd.GetTable();

            }
        }

        private DataTable getTable_ChiTieu(QTA_ChungTu chungtu)
        {
            var sql = FileHelpers.GetSqlQuery("qt_table_chitieu.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    chungtu.iNamLamViec,
                    chungtu.iID_MaNguonNganSach,
                    chungtu.iID_MaNamNganSach,
                    dNgayChungTu = DateTime.Now.Date.AddDays(1),
                    chungtu.iID_MaPhongBan,
                    iID_MaDonVi1 = chungtu.iID_MaDonVi,
                    sLNS1 = string.IsNullOrWhiteSpace(chungtu.sDSLNS)
                        ? (chungtu.iLoai == 2 ? "2,3" : chungtu.iLoai.ToString())
                        : chungtu.sDSLNS.ToParamString(),
                });

                cmdAddParams(cmd, _filters);
                return cmd.GetTable();
            }
        }

        private DataTable mergeTables(DataTable dt1, DataTable dt2, string id, string columnsName)
        {
            if (dt2 == null) return dt1;

            var columns = columnsName.Split(',').ToList();
            var ids = id.Split(',').ToList();

            var dt = dt2.Copy();
            dt1.AsEnumerable()
                .ToList()
                .ForEach(r1 =>
                {
                    if (r1.Field<bool>(ColumnNameIsParent))
                        return;

                    var items = new List<DataRow>();
                    dt.AsEnumerable().ToList()
                    .ForEach(r2 =>
                    {
                        var isEqual = false;
                        foreach (var key in ids)
                        {
                            isEqual = r1.Field<string>(key).Compare(r2.Field<string>(key));
                            if (!isEqual)
                                break;
                        }

                        if (isEqual)
                        {
                            columns.ForEach(c =>
                            {
                                r1[c] = double.Parse(r2[c].ToString());
                            });

                            items.Add(r2);
                        }

                    });

                    items.ForEach(dt.Rows.Remove);
                });

            return dt1;
        }

        private void cmdAddParams(SqlCommand cmd, Dictionary<string, string> filters)
        {
            filters.ToList().ForEach(x =>
            {
                if (x.Key == "sLNS")
                {

                }
                cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
            });
        }

        #endregion

        #region old

        ///// <summary>
        ///// Hàm xác định hàng cha, con
        ///// </summary>
        //protected void CapNhap_arrLaHangCha()
        //{
        //    //Xác định hàng là hàng cha, con
        //    arrCSCha = new List<int>();
        //    arrLaHangCha = new List<bool>();
        //    for (int i = 0; i < dtChiTiet.Rows.Count; i++)
        //    {
        //        DataRow R = dtChiTiet.Rows[i];
        //        //Xac dinh hang nay co phai la hang cha khong?
        //        arrLaHangCha.Add(Convert.ToBoolean(R["bLaHangCha"]));
        //        int CSCha = -1;
        //        for (int j = i - 1; j >= 0; j--)
        //        {
        //            if (Convert.ToString(R["iID_MaMucLucNganSach_Cha"]) ==
        //                Convert.ToString(dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]))
        //            {
        //                CSCha = j;
        //                break;
        //            }
        //        }
        //        arrCSCha.Add(CSCha);
        //    }
        //}


        /// <summary>
        /// Hàm cập nhập mảng dữ liệu
        /// </summary>

        /// <summary>
        /// Hàm tính lại các ô tổng số và tổng cộng các hàng cha
        /// </summary>
        //protected void CapNhapHangTongCong()
        //{
        //    String strDSTruongTien = "rChiTieu,rDaQuyetToan,rDonViDeNghi,rTuChi,rVuotChiTieu,rTonThatTonDong,rDaCapTien,rChuaCapTien";
        //    String[] arrDSTruongTien = strDSTruongTien.Split(',');

        //    int len = arrDSTruongTien.Length;
        //    //Tinh lai cot tong so
        //    double Tong = 0;
        //    //Tinh lai cac hang cha
        //    for (int i = dtChiTiet.Rows.Count - 1; i >= 0; i--)
        //    {
        //        if (Convert.ToBoolean(dtChiTiet.Rows[i]["bLaHangCha"]))
        //        {
        //            String iID_MaMucLucNganSach = Convert.ToString(dtChiTiet.Rows[i]["iID_MaMucLucNganSach"]);
        //            for (int k = 0; k < len; k++)
        //            {
        //                double S;
        //                //rTongSo
        //                S = 0;
        //                for (int j = i + 1; j < dtChiTiet.Rows.Count; j++)
        //                {
        //                    if (iID_MaMucLucNganSach == Convert.ToString(dtChiTiet.Rows[j]["iID_MaMucLucNganSach_Cha"]))
        //                    {
        //                        if (!String.IsNullOrEmpty(Convert.ToString(dtChiTiet.Rows[j][arrDSTruongTien[k]])))
        //                        {
        //                            S += Convert.ToDouble(dtChiTiet.Rows[j][arrDSTruongTien[k]]);
        //                        }
        //                    }
        //                }
        //                dtChiTiet.Rows[i][arrDSTruongTien[k]] = S;
        //            }
        //        }
        //    }


        //    //Them Hàng tổng cộng
        //    DataRow dr = dtChiTiet.NewRow();
        //    DataRow dr_f = dtChiTiet.NewRow();

        //    DataRow[] arrdr = dtChiTiet.Select("sNG<>''");

        //    for (int k = 0; k < len; k++)
        //    {
        //        Tong = 0;
        //        for (int i = 0; i < arrdr.Length; i++)
        //        {

        //            if (!String.IsNullOrEmpty(Convert.ToString(arrdr[i][arrDSTruongTien[k]])))
        //            {
        //                Tong += Convert.ToDouble(arrdr[i][arrDSTruongTien[k]]);
        //            }

        //        }
        //        dr[arrDSTruongTien[k]] = Tong;
        //        dr_f[arrDSTruongTien[k]] = Tong;
        //    }
        //    dr["sMoTa"] = "TỔNG CỘNG";
        //    dr["bLaHangCha"] = true;
        //    dr_f["sMoTa"] = "TỔNG CỘNG";
        //    dr_f["bLaHangCha"] = true;
        //    dtChiTiet.Rows.Add(dr);
        //    dtChiTiet.Rows.InsertAt(dr_f, 0);
        //}



        //protected void CapNhap_arrDuLieu()
        //{
        //    _arrDuLieu = new List<List<string>>();
        //    for (int i = 0; i < dtChiTiet.Rows.Count; i++)
        //    {
        //        _arrDuLieu.Add(new List<string>());
        //        DataRow R = dtChiTiet.Rows[i];
        //        for (int j = 0; j < arrDSMaCot.Count - 3; j++)
        //        {
        //            //Xac dinh gia tri
        //            if (arrDSMaCot[j].EndsWith("_ConLai"))
        //            {
        //                Double GT1 = Convert.ToDouble(_arrDuLieu[i][j - 1]);
        //                Double GT2 = Convert.ToDouble(_arrDuLieu[i][j - 2]);
        //                Double GT3 = Convert.ToDouble(_arrDuLieu[i][j - 3]);
        //                _arrDuLieu[i].Add(Convert.ToString(GT3 - GT2 - GT1));
        //            }
        //            else
        //            {
        //                _arrDuLieu[i].Add(Convert.ToString(R[arrDSMaCot[j]]));
        //            }
        //        }
        //        if (i == dtChiTiet.Rows.Count - 1)
        //        {
        //            _arrDuLieu[i].Add("#A0A0A0");
        //            _arrDuLieu[i].Add("#FF0000");
        //            _arrDuLieu[i].Add("bold");
        //        }
        //        else
        //        {
        //            _arrDuLieu[i].Add("");
        //            _arrDuLieu[i].Add("");
        //            _arrDuLieu[i].Add("");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Hàm xác định các ô có được Edit hay không
        ///// </summary>
        //protected void CapNhap_arrEdit()
        //{
        //    _arrEdit = new List<List<string>>();
        //    for (int i = 0; i < dtChiTiet.Rows.Count; i++)
        //    {
        //        Boolean okHangChiDoc = false;
        //        _arrEdit.Add(new List<string>());
        //        DataRow R = dtChiTiet.Rows[i];

        //        if (Convert.ToBoolean(R["bLaHangCha"]))
        //        {
        //            okHangChiDoc = true;
        //        }

        //        for (int j = 0; j < arrDSMaCot.Count; j++)
        //        {
        //            Boolean okOChiDoc = true;
        //            //Xac dinh o chi doc
        //            if (arrDSMaCot[j] == "bDongY" || arrDSMaCot[j] == "sLyDo")
        //            {
        //                //Cot duyet
        //                if (_DuocSuaDuyet && IsReadonly == false && okHangChiDoc == false)
        //                {
        //                    okOChiDoc = false;
        //                }
        //            }
        //            else
        //            {
        //                //Cot tien
        //                if ((_isEditable &&
        //                     IsReadonly == false &&
        //                     okHangChiDoc == false) &&
        //                    (arrDSMaCot[j] != "rTongSo" &&
        //                     //_arrDSMaCot[j].EndsWith("_DonVi") == false &&
        //                     arrDSMaCot[j].EndsWith("_ChiTieu") == false &&
        //                     arrDSMaCot[j].EndsWith("_DaQuyetToan") == false &&
        //                     arrDSMaCot[j].EndsWith("_ConLai") == false &&
        //                     dtChiTiet.Columns.IndexOf("b" + arrDSMaCot[j]) >= 0 &&
        //                     Convert.ToBoolean(R["b" + arrDSMaCot[j]])) || arrDSMaCot[j].EndsWith("_DonVi"))
        //                {
        //                    okOChiDoc = false;
        //                }
        //                if ((_isEditable &&
        //                     IsReadonly == false &&
        //                     okHangChiDoc == false) && arrDSMaCot[j] == "rDonViDeNghi")
        //                {
        //                    okOChiDoc = false;
        //                }
        //                if ((_isEditable &&
        //                      IsReadonly == false &&
        //                      okHangChiDoc == false) && arrDSMaCot[j] == "rTuChi")
        //                {
        //                    okOChiDoc = false;
        //                }
        //                if ((_isEditable &&
        //                     IsReadonly == false &&
        //                     okHangChiDoc == false) && arrDSMaCot[j] == "rVuotChiTieu")
        //                {
        //                    okOChiDoc = false;
        //                }
        //                if ((_isEditable &&
        //                     IsReadonly == false &&
        //                     okHangChiDoc == false) && arrDSMaCot[j] == "rTonThatTonDong")
        //                {
        //                    okOChiDoc = false;
        //                }
        //                if ((_isEditable &&
        //                     IsReadonly == false &&
        //                     okHangChiDoc == false) && arrDSMaCot[j] == "rDaCapTien")
        //                {
        //                    okOChiDoc = false;
        //                }
        //                if ((_isEditable &&
        //                     IsReadonly == false &&
        //                     okHangChiDoc == false) && arrDSMaCot[j] == "rChuaCapTien")
        //                {
        //                    okOChiDoc = false;
        //                }
        //                if ((_isEditable &&
        //                     IsReadonly == false &&
        //                     okHangChiDoc == false) && arrDSMaCot[j] == "sGhiChu")
        //                {
        //                    okOChiDoc = false;
        //                }
        //            }
        //            if (okOChiDoc)
        //            {
        //                _arrEdit[i].Add("");
        //            }
        //            else
        //            {
        //                _arrEdit[i].Add("1");
        //            }
        //        }
        //    }
        //}
        ///// <summary>
        ///// Hàm cập nhập vào tham số _arrDSMaHang
        ///// </summary>
        //protected void CapNhapDanhSachMaHang()
        //{
        //    _arrDSMaHang = new List<string>();
        //    for (int i = 0; i < dtChiTiet.Rows.Count; i++)
        //    {
        //        DataRow R = dtChiTiet.Rows[i];
        //        String MaHang = String.Format("{0}_{1}", R["iID_MaChungTuChiTiet"], R["iID_MaMucLucNganSach"]);
        //        _arrDSMaHang.Add(MaHang);
        //    }
        //}

        ///// <summary>
        ///// Hàm thêm danh sách cột Fixed vào bảng
        /////     - Cột Fixed của bảng gồm:
        /////         + Các trường của mục lục ngân sách
        /////         + Trường sMaCongTrinh, sTenCongTrinh của cột tiền
        /////     - Cập nhập số lượng cột Fixed
        ///// </summary>
        //protected void CapNhapDanhSachMaCot_Fixed(String iQuy)
        //{
        //    //Khởi tạo các mảng
        //    _arrHienThiCot = new List<Boolean>();
        //    arrTieuDe = new List<string>();
        //    arrDSMaCot = new List<string>();
        //    _arrWidth = new List<int>();

        //    String[] arrDSTruong = (MucLucNganSachModels.strDSTruong + ",rChiTieu,rDaQuyetToan").Split(',');
        //    String[] arrDSTruongTieuDe =
        //        (MucLucNganSachModels.strDSTruongTieuDe + ",Chi tiêu ngân sách,Đã quyết toán").Split(',');
        //    // String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien.Split(',');
        //    String[] arrDSTruongDoRong;
        //    if (iQuy == "4")
        //    {
        //        // arrDSTruongDoRong = "50,25,25,35,35,22,20,20,250,120,120".Split(',');
        //        arrDSTruongDoRong = (MucLucNganSachModels.strDSTruongDoRong + ",120,120").Split(',');
        //    }
        //    else
        //    {
        //        arrDSTruongDoRong = (MucLucNganSachModels.strDSTruongDoRong + ",120,120").Split(',');
        //    }

        //    //Xác định các cột tiền sẽ hiển thị
        //    //_arrCotTienDuocHienThi = new Dictionary<String, Boolean>();
        //    //for (int j = 0; j < arrDSTruongTien.Length; j++)
        //    //{
        //    //    _arrCotTienDuocHienThi.Add(arrDSTruongTien[j], false);
        //    //    for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
        //    //    {
        //    //        DataRow R = _dtChiTiet.Rows[i];
        //    //        if (Convert.ToBoolean(R["b" + arrDSTruongTien[j]]))
        //    //        {
        //    //            _arrCotTienDuocHienThi[arrDSTruongTien[j]] = true;
        //    //            break;
        //    //        }
        //    //    }
        //    //}

        //    //Tiêu đề fix: Thêm trường sMaCongTrinh, sTenCongTrinh
        //    for (int j = 0; j < arrDSTruongTieuDe.Length; j++)
        //    {
        //        arrDSMaCot.Add(arrDSTruong[j]);
        //        arrTieuDe.Add(arrDSTruongTieuDe[j]);
        //        _arrWidth.Add(Convert.ToInt32(arrDSTruongDoRong[j]));
        //        if (arrDSTruong[j] == "sTNG")
        //        {
        //            _arrHienThiCot.Add(false);
        //        }
        //        else
        //        {
        //            _arrHienThiCot.Add(true);
        //        }
        //        _arrSoCotCungNhom.Add(1);
        //        _arrTieuDeNhomCot.Add("");
        //    }

        //    _nCotFixed = arrDSMaCot.Count;
        //}

        ///// <summary>
        ///// Hàm thêm danh sách cột Slide vào bảng
        /////     - Cột Slide của bảng gồm:
        /////         + Trường của cột tiền trừ sMaCongTrinh, sTenCongTrinh
        /////         + Trường sTongSo
        /////         + Trường bDongY, sLyDo
        /////     - Cập nhập số lượng cột Slide
        ///// </summary>
        //protected void CapNhapDanhSachMaCot_Slide(String iQuy)
        //{
        //    //String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien.Split(',');
        //    //String[] arrDSTruongTienTieuDe = MucLucNganSachModels.strDSTruongTienTieuDe.Split(',');
        //    //String[] arrDSTruongTienDoRong = MucLucNganSachModels.strDSTruongTienDoRong.Split(',');

        //    String[] arrDSTruongTien =
        //        //"rChiTieu,rDaQuyetToan,rSoTien,rDonViDeNghi,rVuotChiTieu,rTonThatTonDong".Split(',');
        //        "rDonViDeNghi,rTuChi,rVuotChiTieu,rTonThatTonDong,rDaCapTien,rChuaCapTien,sGhiChu".Split(',');
        //    String[] arrDSTruongTienTieuDe =
        //        "Đơn vị đề nghị,Số quyết toán duyệt,Vượt chỉ tiêu, Tổn thất-tồn đọng,Đã cấp tiền,Chưa cấp tiền,Ghi chú".
        //            Split(',');
        //    String[] arrDSTruongTienDoRong = "115,115,115,115,115,115,300".Split(',');
        //    String[] arrDSNhom_TieuDe =
        //        ",,Đề nghị bộ xử lý,Đề nghị bộ xử lý,Chi tiêu ngân sách,Chi tiêu ngân sách,Chi tiêu ngân sách".Split(',');
        //    String[] arrDSNhom_SoCot = "1,1,2,2,3,3,3".Split(',');

        //    if (iQuy == "4")
        //    {
        //        for (int j = 0; j < arrDSTruongTien.Length; j++)
        //        {
        //            arrDSMaCot.Add(arrDSTruongTien[j]);
        //            arrTieuDe.Add(arrDSTruongTienTieuDe[j]);
        //            _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));

        //            _arrSoCotCungNhom.Add(Convert.ToInt32(arrDSNhom_SoCot[j]));
        //            _arrTieuDeNhomCot.Add(arrDSNhom_TieuDe[j]);
        //            // _arrAlign.Add(arrDSTruongAlign[j]);

        //            _arrHienThiCot.Add(true);

        //        }
        //    }
        //    //các quý khác sẽ ẩn các truong tien, trừ cot rTuChi
        //    else
        //    {
        //        for (int j = 0; j < arrDSTruongTien.Length; j++)
        //        {
        //            if (arrDSTruongTien[j] == "rTuChi" || arrDSTruongTien[j] == "rDonViDeNghi")
        //            {
        //                arrDSMaCot.Add(arrDSTruongTien[j]);
        //                arrTieuDe.Add(arrDSTruongTienTieuDe[j]);
        //                _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));

        //                _arrSoCotCungNhom.Add(Convert.ToInt32(arrDSNhom_SoCot[j]));
        //                _arrTieuDeNhomCot.Add(arrDSNhom_TieuDe[j]);
        //                // _arrAlign.Add(arrDSTruongAlign[j]);

        //                _arrHienThiCot.Add(true);
        //            }

        //        }
        //    }

        //    //Tiêu đề tiền
        //    //for (int j = 0; j < arrDSTruongTien.Length; j++)
        //    //{
        //    //    if (arrDSTruongTien[j] == "sTenCongTrinh" &&
        //    //        _arrCotTienDuocHienThi[arrDSTruongTien[j]])
        //    //    {
        //    //        _arrDSMaCot.Add(arrDSTruongTien[j]);
        //    //        _arrTieuDe.Add(arrDSTruongTienTieuDe[j]);
        //    //        _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //        _arrHienThiCot.Add(true);
        //    //        _arrSoCotCungNhom.Add(1);
        //    //        _arrTieuDeNhomCot.Add("");
        //    //    }
        //    //    if (arrDSTruongTien[j] == "rNgay" &&
        //    //        _arrCotTienDuocHienThi[arrDSTruongTien[j]])
        //    //    {
        //    //        _arrDSMaCot.Add(arrDSTruongTien[j]);
        //    //        _arrTieuDe.Add(arrDSTruongTienTieuDe[j]);
        //    //        _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //        _arrHienThiCot.Add(true);
        //    //        _arrSoCotCungNhom.Add(1);
        //    //        _arrTieuDeNhomCot.Add("");
        //    //    }
        //    //    if (arrDSTruongTien[j] == "rSoNguoi" &&
        //    //        _arrCotTienDuocHienThi[arrDSTruongTien[j]])
        //    //    {
        //    //        _arrDSMaCot.Add(arrDSTruongTien[j]);
        //    //        _arrTieuDe.Add(arrDSTruongTienTieuDe[j]);
        //    //        _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //        _arrHienThiCot.Add(true);
        //    //        _arrSoCotCungNhom.Add(1);
        //    //        _arrTieuDeNhomCot.Add("");
        //    //    }
        //    //}

        //    //for (int j = 0; j < arrDSTruongTien.Length; j++)
        //    //{
        //    //    if (arrDSTruongTien[j] == "rTongSo" ||
        //    //        (arrDSTruongTien[j] != "sTenCongTrinh" && arrDSTruongTien[j] != "rNgay" && arrDSTruongTien[j] != "rSoNguoi" && _arrCotTienDuocHienThi[arrDSTruongTien[j]]))
        //    //    {
        //    //        //_arrDSMaCot.Add(arrDSTruongTien[j] + "_DonVi");
        //    //        //_arrTieuDe.Add("Đơn vị đề nghị");
        //    //        //_arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //        //_arrHienThiCot.Add(true);
        //    //        //_arrSoCotCungNhom.Add(5);
        //    //        //_arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

        //    //        //_arrDSMaCot.Add(arrDSTruongTien[j] + "_ChiTieu");
        //    //        //_arrTieuDe.Add("Chỉ tiêu");
        //    //        //_arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //        //_arrHienThiCot.Add(true);
        //    //        //_arrSoCotCungNhom.Add(5);
        //    //        //_arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

        //    //        _arrDSMaCot.Add(arrDSTruongTien[j]);
        //    //        _arrTieuDe.Add("Quyết toán");
        //    //        _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //        _arrHienThiCot.Add(true);
        //    //        _arrSoCotCungNhom.Add(1);
        //    //        _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

        //    //_arrDSMaCot.Add(arrDSTruongTien[j] + "_DaQuyetToan");
        //    //_arrTieuDe.Add("Đã quyết toán");
        //    //_arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //_arrHienThiCot.Add(true);
        //    //_arrSoCotCungNhom.Add(5);
        //    //_arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

        //    //_arrDSMaCot.Add(arrDSTruongTien[j] + "_ConLai");
        //    //_arrTieuDe.Add("Còn lại");
        //    //_arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
        //    //_arrHienThiCot.Add(true);
        //    //_arrSoCotCungNhom.Add(5);
        //    //_arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);
        //    //    }
        //    //}

        //    //Them cot duyet
        //    if (CoCotDuyet)
        //    {
        //        //Cột đồng ý
        //        arrDSMaCot.Add("bDongY");
        //        if (IsReadonly)
        //        {
        //            arrTieuDe.Add("<div class='check'></div>");
        //        }
        //        else
        //        {
        //            arrTieuDe.Add("<div class='check' onclick='BangDuLieu_CheckAll();'></div>");
        //        }
        //        _arrWidth.Add(20);
        //        _arrHienThiCot.Add(true);
        //        _arrSoCotCungNhom.Add(1);
        //        _arrTieuDeNhomCot.Add("");
        //        //Cột Lý do
        //        arrDSMaCot.Add("sLyDo");
        //        arrTieuDe.Add("Nhận xét");
        //        _arrWidth.Add(200);
        //        _arrHienThiCot.Add(true);
        //        _arrSoCotCungNhom.Add(1);
        //        _arrTieuDeNhomCot.Add("");
        //    }

        //    _nCotSlide = arrDSMaCot.Count - _nCotFixed;
        //}

        ///// <summary>
        ///// Hàm thêm các cột thêm của bảng
        ///// </summary>
        //protected void CapNhapDanhSachMaCot_Them()
        //{
        //    String strDSTruong = "iID_MaMucLucNganSach,sMauSac,sFontColor,sFontBold";
        //    String[] arrDSTruong = strDSTruong.Split(',');
        //    for (int j = 0; j < arrDSTruong.Length; j++)
        //    {
        //        arrDSMaCot.Add(arrDSTruong[j]);
        //        arrTieuDe.Add(arrDSTruong[j]);
        //        _arrWidth.Add(0);
        //        _arrHienThiCot.Add(false);
        //        _arrSoCotCungNhom.Add(1);
        //        _arrTieuDeNhomCot.Add("");
        //    }
        //}

        #endregion
    }
}
