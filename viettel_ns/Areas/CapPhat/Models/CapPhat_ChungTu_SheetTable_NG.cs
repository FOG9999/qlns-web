using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.CapPhat.Models
{
    public class CapPhat_ChungTu_SheetTable_NG : SheetTable
    {
        private ICapPhatService _capPhatService = CapPhatService.Default;
        private INganSachService _nganSachService = NganSachService.Default;

        private CP_CapPhat _entity;

        public CapPhat_ChungTu_SheetTable_NG()
        {

        }

        public CapPhat_ChungTu_SheetTable_NG(string id, int option, Dictionary<string, string> query)
        {
            var filters = new Dictionary<string, string>();

            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            _entity = _capPhatService.GetChungTu(Guid.Parse(id));
            fillSheet(id, option, filters);
        }

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "sLNS", header: "LNS", columnWidth:60, isFixed: true, hasSearch: true),
                    new SheetColumn(columnName: "sL", header: "L", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sK", header: "K", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sM", header: "M", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sTM", header: "TM", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sTTM", header: "TTM", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sNG", header: "NG", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:240, isFixed: true, hasSearch: true),
                    //new SheetColumn(columnName: "sXauNoiMa", header: "sXauNoiMa", columnWidth:220, isFixed: true),
                    //new SheetColumn(columnName: "sXauNoiMa_Cha", header: "sXauNoiMa_Cha", columnWidth:220, isFixed: true),

                    new SheetColumn(columnName: "iID_MaDonVi", header: "Mã", columnWidth:60, isReadonly: true, hasSearch: true),
                    new SheetColumn(columnName: "sTenDonVi", header: "Tên đơn vị", columnWidth:160),

                    new SheetColumn(columnName: "rTuChi_PhanBo", header: "Số dự toán", columnWidth:140,  dataType: 1),
                    new SheetColumn(columnName: "rTuChi_DaCap", header: "Đã cấp phát", columnWidth:140,  dataType: 1),

                    new SheetColumn(columnName: "rTuChi_ConLai", header: "Còn lại", columnWidth:140, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "rTuChi", header: "Cấp phát", columnWidth:140, dataType: 1, isReadonly: false),

                    // cot khac
                    new SheetColumn(columnName: "iID_MaMucLucNganSach", isHidden: true),
                    new SheetColumn(columnName: "Id", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

            #endregion

            return list;
        }

        #region private methods

        private void fillSheet(string id, int option, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();

            //dtChiTiet = getDataChiTiet(_entity).Copy();
            dtChiTiet = getDataChiTiet2(_entity, option).Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();


            ColumnNameId = "sXauNoiMa";
            ColumnNameParentId = "sXauNoiMa_Cha";
            ColumnNameIsParent = "bLaHangCha";

            //fillterData(dtChiTiet, option);
            fillData();
        }

        private void fillData()
        {
            updateSummaryRows();
            insertSummaryRows("sMoTa");

            //updateColumnIDs("iID_MaCapPhatChiTiet,iID_MaMucLucNganSach");
            updateColumnIDs("iID_MaCapPhatChiTiet,sXauNoiMa,iID_MaDonVi");
            updateColumns();
            updateColumnsParent();
            updateCellsEditable();

            //CapNhap_arrEdit();
            //CapNhap_arrDuLieu();
            updateCellsValue();
            updateChanges();
        }

        #endregion

        #region capphat_ng


        /// <summary>
        /// Cấp có chỉ tiêu, ko chỉ tiêu, ...
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="filter_option">0: Tất cả, 1: Có số liệu</param>
        /// <returns></returns>
        private DataTable getDataChiTiet2(CP_CapPhat entity, int filter_option = 0)
        {

            #region lay mlns

            var sql = @"
select  
        sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa
        ,sXauNoiMa_Cha=''     
        ,bLaHangCha
        ,iID_MaDonVi =''
        ,sTenDonVi   =''
        ,iID_MaCapPhatChiTiet=''
from    dbo.f_mlns_full(@iNamLamViec)
where    
        (
                   (sXauNoiMa in (@lns1)      
                or sXauNoiMa in (@lns3)
                or sXauNoiMa in (@lns5)
                or sLNS in (@lns))
                and @search
        )
        -- NSBĐ lọc theo ngành
        and (sLNS<>'1040100' or sNG='' or sNG in (@ng))

";
            var mlns = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG";
            var search = Filters.Where(x => x.Key.IsContains(mlns)).Select(x => $"({x.Key}='' or {x.Key} like '{x.Value}%')").Join(" and ");
            var lns_list = entity.sDSLNS.ToList();

            sql = sql
                .Replace("@lns1", lns_list.Select(x => $"'{x.Substring(0, 1)}'").JoinDistinct())
                .Replace("@lns3", lns_list.Select(x => $"'{x.Substring(0, 3)}'").JoinDistinct())
                .Replace("@lns5", lns_list.Select(x => $"'{x.Substring(0, 5)}'").JoinDistinct())
                .Replace("@lns", entity.sDSLNS.ToList().Select(x => $"'{x}'").Join(","))
                .Replace("@ng", entity.sDSLNS.Contains("1040100") ? _nganSachService.Nganh_GetAll_ChuyenNganh(HttpContext.Current.User.Identity.Name, PhienLamViecViewModel.Current.NamLamViec).ToList().JoinFormat(format: "'{0}'") : "null")
                .Replace("@search", $"({search})");

            var lns_search = Filters["sLNS"] ?? string.Empty;
            if (lns_search.Length > 0)
            {
                sql += $" and (left(sLNS,{lns_search.Length}) in ({lns_search}))";
            }


            #endregion


            using (var con = ConnectionFactory.Default.GetConnection())
            {
                var dt = con.GetTable(sql, new
                {
                    entity.iNamLamViec,
                });

                #region lay xaunoima_cha

                var sXauNoiMa = "sXauNoiMa";
                var sXauNoiMa_Cha = "sXauNoiMa_Cha";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];

                    // lan nguoc len de lay hang cha gan nhat
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var row_hangcha = dt.Rows[j];
                        if (row_hangcha.Field<bool>(ColumnNameIsParent) &&
                            row.Field<string>(sXauNoiMa).StartsWith(row_hangcha.Field<string>(sXauNoiMa)))
                        {
                            row.SetField<string>(sXauNoiMa_Cha, row_hangcha.Field<string>(sXauNoiMa));
                            break;
                        }
                    }
                    //row[ColumnNameIsParent] = true;
                }

                #endregion

                #region xu ly donvi

                var iID_MaDonVi = entity.iID_MaDonVi.IsEmpty() ?
                    PhienLamViecViewModel.Current.iID_MaDonVi :
                    entity.iID_MaDonVi;

                if (Filters.ContainsKey("iID_MaDonVi") && Filters["iID_MaDonVi"].IsNotEmpty())
                {
                    iID_MaDonVi = Filters["iID_MaDonVi"];
                }

                var donvis = iID_MaDonVi.ToList().Select(x => new NS_DonVi
                {
                    iID_MaDonVi = x,
                    sMoTa = _nganSachService.GetDonVi(entity.iNamLamViec.ToString(), x).sMoTa,
                }).ToList();

                #endregion

                #region tao bang so lieu

                var data = getData_CapPhat(
                    entity.iNamLamViec.ToString(),
                    entity.sDSLNS,
                    iID_MaDonVi,
                    PhienLamViecViewModel.Current.iID_MaPhongBan,
                    entity.iID_MaNamNganSach == 2 ? "2,4" : "1,5",
                    entity.dNgayCapPhat.ToParamDate(),
                    null,
                    entity.iID_MaCapPhat.ToString());


                dt.Columns.Add("rTuChi_PhanBo", typeof(double));
                dt.Columns.Add("rTuChi_DaCap", typeof(double));
                dt.Columns.Add("rTuChi_ConLai", typeof(double));
                dt.Columns.Add("rTuChi", typeof(double));

                var rows_null = new List<DataRow>();
                var rows = data.AsEnumerable().ToList();

                #region ghep bang

                // neu la 1 don vi
                var rows_new = new List<DataRow>();
                foreach (DataRow r in dt.Rows)
                {
                    // bo qua hang cha
                    if (r.Field<bool>(ColumnNameIsParent)) continue;


                    #region 1.donvi

                    if (donvis.Count == 1)
                    {
                        var row =
                           rows.FirstOrDefault(
                               x => x.Field<string>("sXauNoiMa").ToList("_").FirstOrDefault() == r.Field<string>("sXauNoiMa"));
                        if (row == null)
                        {
                            // gan don vi de nhap lieu
                            if (!r.Field<bool>(ColumnNameIsParent))
                            {
                                r["iID_MaDonVi"] = iID_MaDonVi;
                                r["sTenDonVi"] = donvis.First(x => x.iID_MaDonVi == iID_MaDonVi).sMoTa; //sTenDonVi
                            }
                        }
                        else
                        {
                            // gan gia tri chi tieu va cap phat
                            r["iID_MaDonVi"] = row["iID_MaDonVi"];
                            r["sTenDonVi"] = donvis.First(x => x.iID_MaDonVi == row["iID_MaDonVi"].ToString()).sMoTa;
                            //sTenDonVi
                            r["iID_MaCapPhatChiTiet"] = row["iID_MaCapPhatChiTiet"];
                            r["rTuChi_PhanBo"] = row["rTuChi_PhanBo"];
                            r["rTuChi_DaCap"] = row["rTuChi_DaCap"];
                            r["rTuChi_ConLai"] = Convert.ToDouble(row["rTuChi_PhanBo"]) -
                                                 Convert.ToDouble(row["rTuChi_DaCap"]) -
                                                 Convert.ToDouble(row["rTuChi"] == DBNull.Value ? 0 : row["rTuChi"]);
                            r["rTuChi"] = row["rTuChi"];


                            // bỏ dòng đã xử lý
                            rows.Remove(row);
                        }
                    }

                    #endregion

                    #region nhieu donvi

                    else
                    {
                        donvis.ForEach(dv =>
                        {
                            var id_donvi = dv.iID_MaDonVi;
                            var r_new = dt.NewRow();
                            r_new.ItemArray = r.ItemArray;

                            var row =
                               rows.FirstOrDefault(
                                   x => x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa") + "_" + id_donvi);

                            if (row == null)
                            {
                                // gan don vi de nhap lieu

                                r_new["iID_MaDonVi"] = id_donvi;
                                r_new["sTenDonVi"] = donvis.First(x => x.iID_MaDonVi == id_donvi).sMoTa; //sTenDonVi
                            }
                            else
                            {
                                // gan gia tri chi tieu va cap phat
                                r_new["iID_MaDonVi"] = row["iID_MaDonVi"];
                                r_new["sTenDonVi"] = donvis.First(x => x.iID_MaDonVi == row["iID_MaDonVi"].ToString()).sMoTa;
                                //sTenDonVi
                                r_new["iID_MaCapPhatChiTiet"] = row["iID_MaCapPhatChiTiet"];
                                r_new["rTuChi_PhanBo"] = row["rTuChi_PhanBo"];
                                r_new["rTuChi_DaCap"] = row["rTuChi_DaCap"];
                                r_new["rTuChi_ConLai"] = Convert.ToDouble(row["rTuChi_PhanBo"]) -
                                                     Convert.ToDouble(row["rTuChi_DaCap"]) -
                                                     Convert.ToDouble(row["rTuChi"] == DBNull.Value ? 0 : row["rTuChi"]);
                                r_new["rTuChi"] = row["rTuChi"];


                                // bỏ dòng đã xử lý
                                rows.Remove(row);
                            }

                            r_new["sXauNoiMa_Cha"] = r["sXauNoiMa"];
                            r_new["sXauNoiMa"] = r["sXauNoiMa"] + "_" + id_donvi;

                            rows_new.Add(r_new);
                        });
                        r[ColumnNameIsParent] = true;
                    }

                    #endregion
                }

                rows_new.ForEach(r => dt.Rows.Add(r));

                #endregion

                var dtView = dt.DefaultView;
                dtView.Sort = "sXauNoiMa,sXauNoiMa_Cha";
                dt = dtView.ToTable();

                #endregion

                #region loc so lieu

                rows_null.Clear();

                #region loc du lieu

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    if (!row.Field<bool>(ColumnNameIsParent))
                    {
                        if (filter_option == 1)
                        {
                            var is_null = row.ItemArray.Where(x => x is double).Sum(x => (double)x) == 0;
                            if (is_null)
                                rows_null.Add(row);
                        }
                    }
                }
                rows_null.ToList().ForEach(dt.Rows.Remove);
                rows_null.Clear();

                #endregion

                #region loc hang cha ko co hang con

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    if (row.Field<bool>(ColumnNameIsParent))
                    {
                        var has_child = dt.AsEnumerable().Skip(i)
                            .Any(x =>
                                x.Field<string>("sXauNoiMa").StartsWith(row.Field<string>("sXauNoiMa"))
                                && x.Field<string>("sNG").IsNotEmpty()
                                && !x.Field<bool>(ColumnNameIsParent));


                        if (!has_child)
                        {
                            rows_null.Add(row);
                        }
                    }
                }

                rows_null.ToList().ForEach(dt.Rows.Remove);

                #endregion

                #endregion

                return dt;
            }
        }


        private DataTable getData_CapPhat(string iNamLamViec, string lns, string iID_MaDonVi, string iID_MaPhongBan, string iID_MaNamNganSach, object date, string loai, string id_chungtu)
        {
            var sql = FileHelpers.GetSqlQuery("cp_sheet_ng.sql");
            using (var con = ConnectionFactory.Default.GetConnection())
            {
                sql = sql
                    .ToSqlLower()
                    .Replace("select * from f_split(@iID_MaDonVi)", iID_MaDonVi.ToParamList())
                    .Replace("select * from f_split(@sLNS)", lns.ToParamList());

                var dt = con.GetTable(sql, new
                {
                    iNamLamViec,
                    sLNS = lns.ToParamString(),
                    iID_MaDonVi,
                    iID_MaPhongBan,
                    iID_MaNamNganSach,
                    date,
                    loai = loai.ToParamString(),
                    id_chungtu
                });

                return dt;
            }
        }

        #endregion
    }
}
