using System;
using System.Data;
using System.Data.SqlClient;
using DomainModel;

namespace VIETTEL.Models.ThuNop {
    public class ThuNop_DuLieuDaNhapModels
    {        

        public static DataTable Get_DanhSachDuLieu(String MaND, int Trang = 1, int SoBanGhi = 0)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            DataRow R = dtCauHinh.Rows[0];

            SqlDataAdapter da;            
            DataSet ds = new DataSet();
            //Khoi tao DataAdapter

            da = new SqlDataAdapter("[dbo].[sp_shared_dsdvidanhap]", Connection.ConnectionString);
            
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@iNamLamViec", SqlDbType.Int);
            da.SelectCommand.Parameters["@iNamLamViec"].Value = R["iNamLamViec"];
            da.SelectCommand.Parameters.Add("@iLoai", SqlDbType.Char, 10);
            da.SelectCommand.Parameters["@iLoai"].Value = "TN";            

            //Do du lieu vao dataset
            da.Fill(ds);            
            
            return ds.Tables[0];
        }

        public static int Get_DanhSachChungTu_Count( String MaND)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            DataRow R = dtCauHinh.Rows[0];

            int vR;
            SqlCommand cmd = new SqlCommand();            
            

            cmd.Parameters.AddWithValue("@iNamLamViec", R["iNamLamViec"]);        
            cmd.Parameters.AddWithValue("@iLoai", "TN");

            String SQL = String.Format("SELECT COUNT(*) FROM sp_shared_dsdvidanhap(@iNamLamViec,@iLoai)");
            cmd.CommandText = SQL;            
            vR = Convert.ToInt32(Connection.GetValue(cmd, 0));
            cmd.Dispose();
            return vR;
        }
    }
}