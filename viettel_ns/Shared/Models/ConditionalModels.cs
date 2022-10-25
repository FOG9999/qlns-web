using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DomainModel;

namespace VIETTEL.Models
{
    public class ConditionalModels
    {
        public static String DKDonVi(String MaND, SqlCommand cmd)
        {
            String DKDonVi = "";
            DataTable dtNĐonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
            for (int i = 0; i < dtNĐonVi.Rows.Count; i++)
            {
                DKDonVi += "iID_MaDonVi=@iID_MaDonVi" + i;
                if (i < dtNĐonVi.Rows.Count - 1)
                    DKDonVi += " OR ";
                cmd.Parameters.AddWithValue("@iID_MaDonVi" + i, dtNĐonVi.Rows[i]["iID_MaDonVi"]);
            }
            if (String.IsNullOrEmpty(DKDonVi)) DKDonVi = " AND 0=1";
            else
            {
                DKDonVi = "AND (" + DKDonVi + ")";
            }
            return DKDonVi;
        }

        public static String DKPhongBan(String MaND, SqlCommand cmd)
        {
            String DKPhongBan = "";
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            
            if (sTenPB == "02" || sTenPB == "2" || sTenPB == "11")
                DKPhongBan = " AND 1=1";
            else
            {
                DKPhongBan = " AND iID_MaPhongBan=@iID_MaPhongBan";
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
            }          
  
            return DKPhongBan;
        }

        public static String DKPhongBanDich(String MaND, SqlCommand cmd)
        {
            String DKPhongBan = "";
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);

            if (sTenPB == "02" || sTenPB == "2" || sTenPB == "11")
                DKPhongBan = " AND 1=1";
            else
            {
                DKPhongBan = " AND iID_MaPhongBanDich=@iID_MaPhongBan";
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
            }

            return DKPhongBan;
        }
    }
}