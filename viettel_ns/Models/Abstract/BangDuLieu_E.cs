using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web.Configuration;
using System.Linq;
using System;

namespace VIETTEL.Models
{
    public abstract class BangDuLieu_E : BangDuLieu
    {
        private IEnumerable<Cot> _arrDSCot;
        public IEnumerable<Cot> DSCots { get { return _arrDSCot ?? (_arrDSCot = LayDSCot()); } }
        protected abstract IEnumerable<Cot> LayDSCot();
        public Dictionary<string, string> _arrGiaTriTimKiem = null;
        public string CotLaHangCha { get; set; }
        public string CotHangChaId { get; set; }
        public string CotId { get; set; }
        protected virtual void CapNhap_arrLaHangCha()
        {
            //Xác định hàng là hàng cha, con
            _arrCSCha = new List<int>();
            _arrLaHangCha = new List<bool>();

            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                arrLaHangCha.Add(Convert.ToBoolean(r[CotLaHangCha]));

                int parent = -1;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Convert.ToString(r[CotHangChaId]) ==
                        Convert.ToString(dtChiTiet.Rows[j][CotId]))
                    {
                        parent = j;
                        break;
                    }
                }
                arrCSCha.Add(parent);
            }
        }
        protected virtual void CapNhap_arrEdit()
        {
            _arrEdit = new List<List<string>>();

            dtChiTiet.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var hangChiDoc = false;

                    if (!string.IsNullOrWhiteSpace(CotLaHangCha) && Convert.ToBoolean(r[CotLaHangCha])) hangChiDoc = true;

                    var items = new List<string>();
                    DSCots.ToList()
                        .ForEach(c =>
                        {
                            if (_DuocSuaChiTiet && !_ChiDoc && !hangChiDoc)
                            {
                                items.Add(c.ChiDoc ? "" : "1");
                            }
                            else
                            {
                                items.Add("");
                            }
                        });

                    _arrEdit.Add(items);
                });
        }
        protected virtual void CapNhap_arrDuLieu()
        {
            _arrDuLieu = new List<List<string>>();

            var dsCots = dtChiTiet.GetColumnNames();
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                var items = new List<string>();

                _arrDSMaCot.ToList().ForEach(c =>
                {
                    items.Add(LayDuLieuO(r, i, c, dsCots));
                });
                _arrDuLieu.Add(items);

            }
        }
        protected virtual string LayDuLieuO(DataRow r, int i, string c, IEnumerable<string> dsCots)
        {
            try
            {
                if (dsCots.Contains(c))
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

        protected virtual void CapNhap_CotId(string columns)
        {
            _arrDSMaHang = new List<string>();

            var fields = columns.Split(',').ToList();
            dtChiTiet.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var id = !string.IsNullOrWhiteSpace(CotLaHangCha) && r.Field<bool>(CotLaHangCha) ? "" : fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    _arrDSMaHang.Add(id);
                });
        }

        protected virtual void CapNhapDSCots()
        {
            _arrDSMaCot = new List<string>();
            _arrFormat = new List<string>();
            _arrType = new List<string>();
            _arrAlign = new List<string>();            
            _arrTieuDe = new List<string>();
            _arrWidth = new List<int>();
            _arrHienThiCot = new List<bool>();
            _arrSoCotCungNhom = new List<int>();
            _arrTieuDeNhomCot = new List<string>();

            DSCots.ToList().ForEach(ThemCot);
            // fixed
            _nCotFixed = DSCots.Where(x => x.CoDinh).ToList().Count;

            // slide
            _nCotSlide = DSCots.Where(x => !x.CoDinh).ToList().Count;
        }

        private void ThemCot(Cot c)
        {
            _arrDSMaCot.Add(c.Ten);
            _arrTieuDe.Add(c.TieuDe);
            _arrWidth.Add(c.DoRongCot);
            _arrType.Add(c.KieuNhap);
            _arrAlign.Add(c.KieuCanLe);
            _arrHienThiCot.Add(c.Hien);

            _arrSoCotCungNhom.Add(c.SoCotCungNhom);
            _arrTieuDeNhomCot.Add(c.TieuDeNhom);
        }
        #endregion       

        /// <summary>
        /// Tính tổng cho các hàng cha tại các cột là number
        /// </summary>
        protected virtual void CapNhap_HangTongCong()
        {
            var cots = _arrDSCot.Where(x => x.KieuNhap == "1").ToList();

            //Tinh lai cac hang cha
            for (int i = dtChiTiet.Rows.Count - 1; i >= 0; i--)
            {
                var r = dtChiTiet.Rows[i];
                var isParent = r.Field<bool>(CotLaHangCha);

                if (isParent)
                {
                    var id = r[CotId].ToString();

                    cots.ToList()
                               .ForEach(c =>
                               {
                                   var sum = 0d;

                                   for (int j = i + 1; j < dtChiTiet.Rows.Count; j++)
                                   {
                                       var r1 = dtChiTiet.Rows[j];
                                       if (id == r1[CotHangChaId].ToString() && !string.IsNullOrWhiteSpace(r1[c.Ten].ToString()))
                                       {
                                           sum += double.Parse(r1[c.Ten].ToString());
                                       }
                                   }

                                   r[c.Ten] = sum;

                               });
                }
            }
        }

        protected virtual void ThemHang_TongCong(string tenCot)
        {
            var text = "TỔNG CỘNG";
            var soHang = dtChiTiet.Rows.Count;

            var check = soHang <= 0 || (soHang > 0 && dtChiTiet.Rows[0][tenCot].ToString() == text && dtChiTiet.Rows[soHang-1][tenCot].ToString() == text);

            if (check) return;

            var cots = _arrDSCot.Where(x => x.KieuNhap == "1").ToList();

            // add summary rows
            var hangMot = dtChiTiet.NewRow();
            var hangCuoi = dtChiTiet.NewRow();
            var dsHangCon = dtChiTiet.AsEnumerable()
                    .ToList()
                    .Where(x => !x.Field<bool>(CotLaHangCha));
            cots.ToList()
                .ForEach(c =>
                {
                    var sum = dsHangCon.Sum(x => x[c.Ten].ToString() == "" ? 0 : double.Parse(x[c.Ten].ToString()));
                    hangMot[c.Ten] = sum;
                    hangCuoi[c.Ten] = sum;

                });

            hangMot[CotLaHangCha] = true;
            hangMot[tenCot] = text;

            hangCuoi[CotLaHangCha] = true;
            hangCuoi[tenCot] = text;

            dtChiTiet.Rows.InsertAt(hangMot, 0);
            dtChiTiet.Rows.Add(hangCuoi);
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
                var isParent = row.Field<bool>(CotLaHangCha);
                if (i == dt.Rows.Count - 1 && isParent)
                {
                    items.Add(row);
                }
                else
                {
                    for (int j = i; j < dt.Rows.Count; j++)
                    {
                        var rowChild = dt.Rows[j];
                        if (!rowChild.Field<bool>(CotLaHangCha))
                        {
                            rowChild[checkedColumn] = true;
                        }

                        if (row[CotId].ToString() == rowChild[CotHangChaId].ToString() && rowChild.Field<bool>(checkedColumn))
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
    }

    public class Cot
    {
        public const string CanGiua = "center";
        public const string CanPhai = "right";
        public const string CanTrai = "left";

        public string Ten { get; set; }
        public string TieuDe { get; set; }
        public string TieuDeNhom { get; set; }
        public int SoCotCungNhom { get; set; }
        public int DoRongCot { get; set; }

        /// <summary>
        /// Xác định kiểu dữ liệu của cột nhập, trong đó:
        /// 0:  String
        /// 1:  Number
        /// 2:  Bool
        /// 3:  Autocomplete
        /// 4:  Datetime
        /// </summary>
        public string KieuNhap { get; set; }
        public string KieuCanLe { get; set; }
        public string DinhDang { get; set; }
        public bool Hien { get; set; }
        public bool ChiDoc { get; set; }
        public bool CoDinh { get; set; }
        public Cot()
        {

        }
        public Cot(
            string ten,
            string tieuDe = "",
            string tieuDeNhom = "",
            int soCotCungNhom = 1,
            int doRongCot = 0,
            string kieuNhap = "0",
            string kieuCanLe = CanTrai,
            string dinhDang = "",
            bool hien = true,
            bool chiDoc = false,
            bool coDinh = false)
        {
            Ten = ten;
            TieuDe = tieuDe;
            TieuDeNhom = tieuDeNhom;
            SoCotCungNhom = soCotCungNhom;
            DoRongCot = doRongCot;
            KieuNhap = kieuNhap;
            KieuCanLe = kieuCanLe;
            Hien = hien;
            ChiDoc = chiDoc;
            CoDinh = coDinh;
            DinhDang = dinhDang;

            if (string.IsNullOrWhiteSpace(DinhDang))
            {
                if (KieuNhap == "4")
                {
                    DinhDang = "dd/MM/yyyy";
                }
            }

            if (KieuNhap == "1")
            {
                KieuCanLe = CanPhai;
            }
        }

    }
}