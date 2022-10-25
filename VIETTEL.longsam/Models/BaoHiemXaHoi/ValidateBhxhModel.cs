using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Extensions;

namespace Viettel.Models.BaoHiemXaHoi
{
    public class ValidateBhxhModel
    {
        public string sLabelName { get; set; }
        public string sPropertyName { get; set; }
        public int iType { get; set; }
        public int? iMaxLength { get; set; }
        public bool bIsNotNull { get; set; }

        public static List<ValidateBhxhModel> GetValidateBhxh()
        {
            List<ValidateBhxhModel> lstData = new List<ValidateBhxhModel>();
            lstData.Add(new ValidateBhxhModel() { sLabelName = "STT", sPropertyName = "sSTT", iType = (int)Constants.BaoHiemXHType.ITypes.Int });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "ho_ten", sPropertyName = "sHoTen", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 100, bIsNotNull = true });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "NGAYSINH", sPropertyName = "sNgaySinh", iType = (int)Constants.BaoHiemXHType.ITypes.Date, iMaxLength = 10 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "GIOITINH", sPropertyName = "sGioiTinh", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 10 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "CAPBAC", sPropertyName = "sCapBac", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 20 });
            //lstData.Add(new ValidateBhxhModel() { sLabelName = "CHUCVU", sPropertyName = "sChucVu", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 10 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "MATHE", sPropertyName = "sMaThe", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 15 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "NGAY_VAO_VIEN", sPropertyName = "sNgayVaoVien", iType = (int)Constants.BaoHiemXHType.ITypes.Date, iMaxLength = 10 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "NGAY_RA_VIEN", sPropertyName = "sNgayRaVien", iType = (int)Constants.BaoHiemXHType.ITypes.Date, iMaxLength = 10 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "SO_NGAY_DTRI", sPropertyName = "sSoNgayDieuTri", iType = (int)Constants.BaoHiemXHType.ITypes.Int});
            lstData.Add(new ValidateBhxhModel() { sLabelName = "MABV", sPropertyName = "sMaBV", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 5 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "TENBV", sPropertyName = "sTenBV", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 100 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "MADV", sPropertyName = "sMaDV", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 10 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "TENDV", sPropertyName = "sTenDonVi", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 200 });
            lstData.Add(new ValidateBhxhModel() { sLabelName = "GHICHU", sPropertyName = "sGhiChu", iType = (int)Constants.BaoHiemXHType.ITypes.String, iMaxLength = 100 });
            return lstData;
        }
    }
}
