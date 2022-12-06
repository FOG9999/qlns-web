using System;
using System.Collections.Generic;

namespace VIETTEL.Models
{
    public class SheetViewModel
    {

        public SheetViewModel()
        {
            Id = "BangDuLieu";
            Height = 470;
            FixedRowHeight = 50;
        }

        public SheetViewModel(SheetTable bang = null,
            string message = null,
            string id = null,
            int option = 0,
            Dictionary<string, string> filters = null,
            string urlPost = null,
            string urlGet = null,
            bool isReadonly = false,
            int dvt = 0) : this()
        {
            Message = message;
            Table = bang;
            UrlPost = urlPost;
            UrlGet = urlGet;
            Id = id;
            Option = option;
            Filters = filters ?? new Dictionary<string, string>();
            Dvt = dvt;


            if (bang != null)
            {
                IsReadonly = bang.IsReadonly;
            }
            else
            {
                IsReadonly = isReadonly;
            }

            AvaiableKeys = new Dictionary<string, string>()
            {
                {"F2" , "Thêm dòng" },
                {"F10" , "Lưu" },
                {"Del" , "Xóa" },
            };
        }


        public SheetTable Table { get; set; }

        public bool IsReadonly { get; private set; }

        public Dictionary<string, string> AvaiableKeys { get; set; }

        public string UrlPost { get; set; }
        public string UrlGet { get; set; }
        public string Message { get; set; }

        public string Id { get; set; }
        public int Option { get; set; }

        public Dictionary<string, string> Filters { get; set; }
        public string FiltersString { get { return Newtonsoft.Json.JsonConvert.SerializeObject(Filters); } }



        public int Height { get; set; }
        public int FixedRowHeight { get; set; }

        public int ChiTietHeight { get { return Height - FixedRowHeight; } }

        public int Dvt { get; set; }
    }

    public class SheetEditViewModel
    {
        public string Id { get; set; }
        public string Option { get; set; }

        public string FiltersString { get; set; }

        public string idXauMaCacHang { get; set; }
        public string idXauLaHangCha { get; set; }
        public string idXauMaCacCot { get; set; }
        public string idXauGiaTriChiTiet { get; set; }
        public string idXauDuLieuThayDoi { get; set; }
        public string idXauCacHangDaXoa { get; set; }
        public IEnumerable<SheetRow> Rows
        {
            get
            {
                return getRows();
            }
        }

        private IEnumerable<SheetRow> getRows()
        {
            var items = new List<SheetRow>();
            
            var arrLaHangCha = idXauLaHangCha.Split(',');
            //var arrMaHang = idXauMaCacHang.Split(',');
            var arrMaHang = idXauMaCacHang != null ? idXauMaCacHang.Split(',') : new string[1];//NinhNV 7/7/21
            var arrMaCot = idXauMaCacCot.Split(',');
            var arrHangDaXoa = idXauCacHangDaXoa.Split(',');
            var arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] { SheetTable.DauCachHang }, StringSplitOptions.None);
            var arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] { SheetTable.DauCachHang }, StringSplitOptions.None);

            for (int i = 0; i < arrMaHang.Length; i++)
            {
                //if (string.IsNullOrWhiteSpace(arrMaHang[i])) continue;

                //var iID_MaChungTu = arrMaHang[i].Split('_')[0];
                //var iID_MaMucLucNganSach = arrMaHang[i].Split('_')[1];

                var row = new SheetRow()
                {
                    Id = arrMaHang[i],
                    IsParent = arrLaHangCha[i] == "1",
                };

                // neu xoa hang
                if (arrHangDaXoa[i] == "1")
                {
                    row.IsDeleted = true;
                    items.Add(row);
                }
                else
                {
                    var arrThayDoi = arrHangThayDoi[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);

                    var hasChanges = false;
                    for (int j = 0; j < arrMaCot.Length; j++)
                    {
                        if (arrThayDoi[j] == "1")
                        {
                            hasChanges = true;
                            break;
                        }
                    }

                    if (!hasChanges) continue;

                    var arrGiaTri = arrHangGiaTri[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                    for (int j = 0; j < arrMaCot.Length; j++)
                    {
                        if (arrThayDoi[j] == "0" && arrMaCot[j] != "iLevel") continue;
                        row.Columns.Add(arrMaCot[j], arrGiaTri[j]);
                    }

                    items.Add(row);
                }

            }

            return items;
        }
    }

    public class SheetRow
    {

        public SheetRow()
        {
            Columns = new Dictionary<string, string>();
        }

        public string Id { get; set; }

        public bool IsParent { get; set; }

        public bool IsDeleted { get; set; }

        public string ColumnsName { get; set; }

        public string ColumnsValue { get; set; }

        public Dictionary<string, string> Columns { get; set; }

    }
}
