using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VIETTEL;

namespace Viettel.Domain.DomainModel
{
    public class zHelper
    {
        public static Dictionary<string, string> GetKeyList(int loai, int nam)
        {
            var dic = new Dictionary<string, string>();

            if (loai == (int)zType.DuToan)
            {
                dic = new Dictionary<string, string>()
                {
                    {$"DT_{nam}", $"Dự toán đầu năm {nam}" }
                };
            }
            else if (loai == (int)zType.QuyetToan)
            {
                dic = new Dictionary<string, string>()
                {
                    // theo quý
                    {$"QT{nam}_Quy1", $"Quyết toán Quý I/{nam}" },
                    {$"QT{nam}_Quy2", $"Quyết toán Quý II/{nam}" },
                    {$"QT{nam}_Quy3", $"Quyết toán Quý III/{nam}" },
                    {$"QT{nam}_Quy4", $"Quyết toán Quý IV/{nam}" },


                    // theo tháng
                    {$"QT{nam}_Thang1", $"QTTX Tháng 1/{nam}" },
                    {$"QT{nam}_Thang2", $"QTTX Tháng 2/{nam}" },
                    {$"QT{nam}_Thang3", $"QTTX Tháng 3/{nam}" },
                    {$"QT{nam}_Thang4", $"QTTX Tháng 4/{nam}" },
                    {$"QT{nam}_Thang5", $"QTTX Tháng 5/{nam}" },
                    {$"QT{nam}_Thang6", $"QTTX Tháng 6/{nam}" },
                    {$"QT{nam}_Thang7", $"QTTX Tháng 7/{nam}" },
                    {$"QT{nam}_Thang8", $"QTTX Tháng 8/{nam}" },
                    {$"QT{nam}_Thang9", $"QTTX Tháng 9/{nam}" },
                    {$"QT{nam}_Thang10", $"QTTX Tháng 10/{nam}" },
                    {$"QT{nam}_Thang11", $"QTTX Tháng 11/{nam}" },
                    {$"QT{nam}_Thang12", $"QTTX Tháng 12/{nam}" },
                };
            }
            if (loai == (int)zType.SoKiemTra)
            {
                dic = new Dictionary<string, string>()
                {
                    {$"SKT_{nam}", $"Số kiểm tra {nam}" }
                };
            }
            if (loai == (int)zType.SoKiemTra_DuToan)
            {
                dic = new Dictionary<string, string>()
                {
                    {$"SKT_DT_{nam}", $"Dự toán số kiểm tra {nam}" }
                };
            }
            return dic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nam"></param>
        /// <param name="iThangQuy"></param>
        /// <param name="iThangQuyLoai">0: thang, 1: quy</param>
        /// <returns></returns>
        public static string GetKey(int nam, int iLoai, int iThangQuy, int iThangQuyLoai = 0)
        {
            var keys_list = GetKeyList(iLoai, nam);
            string r = null;

            if (keys_list.Count == 1) return keys_list.First().Key;
            if (iLoai == (int)zType.QuyetToan)
            {
                r = iThangQuyLoai == 0 ? $"QT{nam}_Thang{iThangQuy}" : $"QT{nam}_Quy{iThangQuy}";
            }


            return r;
        }



        public static Z_ChungTu GetChungTu_FromFileName(string filename)
        {
            var entity = new Z_ChungTu()
            {
                MoTa = filename,
            };

            //kiem tra neu la duong dan day du
            if (File.Exists(filename))
            {
                filename = new FileInfo(filename).Name;
            }

            try
            {
                var items = filename.ToList("___");

                var donvi = items.ElementAtOrDefault(0).ToStringEmpty().ToList("_");
                entity.Id_DonVi = donvi.ElementAtOrDefault(0);
                entity.Id_DonVi_Ten = donvi.ElementAtOrDefault(1).ToStringEmpty();

                entity.Key = items.ElementAtOrDefault(1).ToStringEmpty();
                entity.Id_PhongBan = items.ElementAtOrDefault(2).ToStringEmpty();
                entity.NoiDung = items.ElementAtOrDefault(3).ToStringEmpty();
                entity.Id = Guid.Parse(items.ElementAtOrDefault(4).ToStringEmpty());

                //entity.MoTa = filename;
            }
            catch (Exception)
            {

            }

            return entity;
        }

        public static string GetFolderSync(int nam)
        {
            var root = HttpContext.Current.Server.MapPath("~/app_data");
            var folder = Path.Combine(root, $@"ns_sync\{nam}");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            return folder;
        }
    }
}