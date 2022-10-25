using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Viettel.Data;

namespace Viettel.Services
{
    public interface IDuToanReportService
    {
        DataTable ChonTo(
          string username,
          string lns,
          int maTo = 1,
          int dvt = 1000);

        DataTable TungDonVi(
            string username,
            string donvi,
            string lns,
            string m = "",
            string loai = "",
            //bool isNhaNuoc = false, 
            int dvt = 1000);

        DataTable TungDonVi_NhaNuoc(
          string username,
          string donvi,
          string lns,
          string m = "",
          //string loai = "",
          int dvt = 1000);



        /// <summary>
        /// Ngân sách nghiệp vụ ngành kỹ thuật
        /// </summary>
        /// <param name="username"></param>
        /// <param name="donvi"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        DataTable TungDonVi_1020100_KT(string username, string donvi, int dvt = 1000);


        /// <summary>
        /// Tùy viên quốc phòng
        /// </summary>
        /// <param name="username"></param>
        /// <param name="donvi"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        DataTable TungDonVi_1020200(string username, string donvi, int dvt = 1000);

        //DataTable DT_rptDuToan_1010000_ChonTo(string username, string lns, int maTo = 1, int dvt = 1000);

        DataTable TongHop(
            string username,
            string phongban,
            string lns,
            int dvt = 1000);


        DataTable GetDonviListByLNS(string username, string lns);
        string GetDonvisByLNS(string username, string lns, int namLamViec = 0);

    }

    public class DuToanReportService : IDuToanReportService
    {
        private readonly INganSachService _nganSachService;
        private readonly IConnectionFactory _connectionFactory;
        public DuToanReportService()
        {
            _nganSachService = NganSachService.Default;
            _connectionFactory = new ConnectionFactory();
        }

        public DuToanReportService(
            IConnectionFactory connectionFactory,
            INganSachService nganSachService)
        {
            _connectionFactory = connectionFactory;
            _nganSachService = nganSachService;
        }

        public DataTable TungDonVi_1020200(string username, string iID_MaDonVi, int dvt = 1000)
        {
            var config = _nganSachService.GetCauHinh(username);

            #region sql

            var sql =
                @"SELECT DISTINCT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
                    ,rTuChi=SUM(rTuChi/{0})
                    ,rHienVat=SUM(rHienVat/{0})
                    ,rHangNhap=SUM(rHangNhap/{0})
                 FROM DT_ChungTuChiTiet
                 WHERE iTrangThai=1  AND 
                    sLNS='1020200' AND 
                    iID_MaDonVi = @iID_MaDonVi  AND 
                    iNamLamViec=@iNamLamViec 
                 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi";

            #endregion

            sql = string.Format(sql, dvt);

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         username,
                         iID_MaDonVi,
                         iNamLamViec = config.iNamLamViec,
                     },
                     commandType: CommandType.Text
                 );

                return dt;
            }
        }

        #region chon to

        //public DataTable DT_rptDuToan_1010000_ChonTo(string username, int maTo = 1, int dvt = 1000)
        //{
        //    return DT_rptDuToan_ChonTo(username, "1010000", maTo);
        //}

        //public DataTable DT_rptDuToan_1020000_ChonTo(string username, int maTo = 1, int dvt = 1000)
        //{
        //    return DT_rptDuToan_ChonTo(username, "1020000", maTo);
        //}

        public DataTable ChonTo(string username, string lns, int maTo = 1, int dvt = 1000)
        {
            var config = _nganSachService.GetCauHinh(username);
            var phongban = _nganSachService.GetPhongBan(username);
            //var donVis = _nganSachService.GetDonviListByUser(username, config.iNamLamViec)
            //    .Select(x => x.iID_MaDonVi)
            //    .ToList();
            var donVis = GetDonviListByLNS(username, lns)
                .AsEnumerable()
                .Select(x => x.Field<string>("iID_MaDonVi"))
                .ToList();

            var dkPhongban = getDkPhongBan(phongban.sKyHieu);
            var dkLns = getDkLns(lns);

            var sql = string.Empty;

            if (maTo == 1)
            {
                #region sql

                sql =
@"SELECT    SUBSTRING(sLNS,1,1) as sLNS1,
            SUBSTRING(sLNS,1,3) as sLNS3,
            SUBSTRING(sLNS,1,5) as sLNS5,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            SUM(CongTrongKy/{0}) as rTuChi,
            SUM(DonVi1/{0}) AS DonVi1,
            SUM(DonVi2/{0}) AS DonVi2,
            SUM(DonVi3/{0}) AS DonVi3,
            SUM(DonVi4/{0}) AS DonVi4,
            SUM(DonVi5/{0}) AS DonVi5
FROM
(
        SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
                CongTrongKy =SUM(CASE WHEN iNamLamViec=@iNamLamViec THEN rTuChi ELSE 0 END),
                DonVi1      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi1 ) THEN rTuChi ELSE 0 END),
                DonVi2      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi2 ) THEN rTuChi ELSE 0 END),
                DonVi3      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi3 ) THEN rTuChi ELSE 0 END),
                DonVi4      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi4 ) THEN rTuChi ELSE 0 END),
                DonVi5      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi5 ) THEN rTuChi ELSE 0 END)
         FROM   DT_ChungTuChiTiet
         WHERE  
                {1} -- phongban
                {2} -- lns
                {3} -- donvi
                iTrangThai  = 1 AND iNamLamViec =@iNamLamViec
        GROUP BY   sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi) a
 GROUP BY  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
 HAVING SUM(CongTrongKy)<>0  OR 
        SUM(DonVi1)<>0  OR 
        SUM(DonVi2)<>0  OR 
        SUM(DonVi3)<>0  OR 
        SUM(DonVi4)<>0  OR 
        SUM(DonVi5)<>0";

                #endregion


                var dkDonvi = getDkDonVi(donVis);

                sql = string.Format(sql, dvt, dkPhongban, dkLns, dkDonvi);

                using (var conn = _connectionFactory.GetConnection())
                {
                    var dt = conn.ExecuteDataTable(
                         commandText: sql,
                         parameters: new
                         {
                             MaDonVi1 = donVis.Count() >= 1 ? donVis[0] : "",
                             MaDonVi2 = donVis.Count() >= 2 ? donVis[1] : "",
                             MaDonVi3 = donVis.Count() >= 3 ? donVis[2] : "",
                             MaDonVi4 = donVis.Count() >= 4 ? donVis[3] : "",
                             MaDonVi5 = donVis.Count() >= 5 ? donVis[4] : "",
                             iNamLamViec = config.iNamLamViec,
                         },
                         commandType: CommandType.Text
                     );

                    return dt;
                }
            }
            else
            {
                #region sql

                sql =
@"SELECT    SUBSTRING(sLNS,1,1) as sLNS1,
            SUBSTRING(sLNS,1,3) as sLNS3,
            SUBSTRING(sLNS,1,5) as sLNS5,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            SUM(DonVi1/{0}) AS DonVi1,
            SUM(DonVi2/{0}) AS DonVi2,
            SUM(DonVi3/{0}) AS DonVi3,
            SUM(DonVi4/{0}) AS DonVi4,
            SUM(DonVi5/{0}) AS DonVi5,
            SUM(DonVi6/{0}) AS DonVi6
FROM
(
        SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
                DonVi1      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi1 ) THEN rTuChi ELSE 0 END),
                DonVi2      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi2 ) THEN rTuChi ELSE 0 END),
                DonVi3      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi3 ) THEN rTuChi ELSE 0 END),
                DonVi4      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi4 ) THEN rTuChi ELSE 0 END),
                DonVi5      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi5 ) THEN rTuChi ELSE 0 END),
                DonVi6      =SUM(CASE WHEN (iID_MaDonVi=@MaDonVi6 ) THEN rTuChi ELSE 0 END)
         FROM   DT_ChungTuChiTiet
         WHERE  
                {1} -- phongban
                {2} -- lns
                {3} -- donvi
                iTrangThai  = 1 AND iNamLamViec =@iNamLamViec
        GROUP BY   sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi) a
 GROUP BY  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
 HAVING 
        SUM(DonVi1)<>0  OR 
        SUM(DonVi2)<>0  OR 
        SUM(DonVi3)<>0  OR 
        SUM(DonVi4)<>0  OR 
        SUM(DonVi5)<>0  OR 
        SUM(DonVi6)<>0";

                #endregion

                // phan trang
                donVis = donVis.Skip((maTo - 1) * 6 - 1).Take(6).ToList();
                var dkDonvi = getDkDonVi(donVis);

                sql = string.Format(sql, dvt, dkPhongban, dkLns, dkDonvi);

                using (var conn = _connectionFactory.GetConnection())
                {
                    var dt = conn.ExecuteDataTable(
                         commandText: sql,
                         parameters: new
                         {
                             MaDonVi1 = donVis.Count() >= 1 ? donVis[0] : "",
                             MaDonVi2 = donVis.Count() >= 2 ? donVis[1] : "",
                             MaDonVi3 = donVis.Count() >= 3 ? donVis[2] : "",
                             MaDonVi4 = donVis.Count() >= 4 ? donVis[3] : "",
                             MaDonVi5 = donVis.Count() >= 5 ? donVis[4] : "",
                             MaDonVi6 = donVis.Count() >= 6 ? donVis[5] : "",
                             iNamLamViec = config.iNamLamViec,
                         },
                         commandType: CommandType.Text
                     );

                    return dt;
                }
            }
        }

        #endregion

        #region tung don vi


        public DataTable TungDonVi(
            string username,
            string donvi,
            string lns,
            string m = "",
            string loai = "",
            int dvt = 1000)
        {
            return dt_rptDuToan_TungDonVi(username, donvi, lns, m, loai, false, dvt);
        }

        public DataTable TungDonVi_NhaNuoc(
            string username,
            string donvi,
            string lns,
            string m = "",
            //string loai = "",
            int dvt = 1000)
        {
            return dt_rptDuToan_TungDonVi(username, donvi, lns, m, "", true, dvt);
        }

        private DataTable dt_rptDuToan_TungDonVi(
            string username,
            string donvi,
            string lns,
            string m = "",
            string loai = "",
            bool isNhaNuoc = false,
            int dvt = 1000)
        {
            #region sql

            var sql =
@"SELECT    sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu,
            rTuChi      =SUM(rTuChi/{0}),
            rHienVat     =SUM(rHienVat/{0}),
            rTonKho     =SUM(rTonKho/{0}),
            rHangNhap   =SUM(rHangNhap/{0}),
            rHangMua    =SUM(rHangMua/{0}),
            rPhanCap    =SUM(rPhanCap/{0}),
            rDuPhong    =SUM(rDuPhong/{0})
    FROM DT_ChungTuChiTiet
    WHERE   
            -- theo nganh
            {1}
            -- theo donvi
            {2}
            -- theo lns
            {3} 
            -- theo m
            {4}
            -- theo ma loai
            {5}
            iTrangThai=1  AND 
            iNamLamViec=@iNamLamViec
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu
    HAVING  SUM(rTuChi)<>0 OR 
            SUM(rHienVat)<>0 OR 
            SUM(rHangNhap)<>0 OR 
            SUM(rTonKho)<>0  OR 
            SUM(rHangMua)<>0  OR 
            SUM(rPhanCap)<>0 OR 
            SUM(rDuPhong)<>0";

            #endregion

            #region dieu kien

            var dkNganh = "";
            var dkDonvi = "";
            var dkLns = getDkLns(lns);
            var dkM = string.IsNullOrWhiteSpace(m) ? string.Empty : string.Format("sM IN ('{0}')  AND", m);
            var dkLoai = string.IsNullOrWhiteSpace(loai) ? string.Empty : string.Format("MaLoai IN ('{0}')  AND", loai);
            var config = _nganSachService.GetCauHinh(username);

            var maNganh = isNhaNuoc ?
                _nganSachService.MLNS_MaNganh_NhaNuoc(donvi) :
                _nganSachService.MLNS_MaNganh( config.iNamLamViec.ToString(),donvi);

            // khong truy can neu ko co ma nganh
            if (string.IsNullOrWhiteSpace(maNganh))
            {
                maNganh = "-1";
            }
            else
            {
                dkDonvi = getDkDonViByUser(username);
            }

            dkNganh = string.Format("sNG IN ({0}) AND", maNganh);
            sql = string.Format(sql, dvt, dkNganh, dkDonvi, dkLns, dkM, dkLoai);

            #endregion

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iNamLamViec = config.iNamLamViec,
                     },
                     commandType: CommandType.Text
                 );

                return dt;
            }
        }

        public DataTable TungDonVi_1020100_KT(string username, string donvi, int dvt = 1000)
        {
            #region sql

            var sql =
@"SELECT    sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu,
            rTuChi      =SUM(rTuChi/{0}),
            rHienVat     =SUM(rHienVat/{0}),
            rTonKho     =SUM(rTonKho/{0})
    FROM DT_ChungTuChiTiet
    WHERE   
            -- theo phongban
            {1}
            -- theo donvi
            --{2}

            iID_MaDonVi =@iID_MaDonVi AND
            iNamLamViec=@iNamLamViec AND

            sLNS='1020100' AND 
            sM IN (6900,7000,7750,9050) AND
            iTrangThai=1 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu
    HAVING  SUM(rTuChi)<>0 OR 
            SUM(rHienVat)<>0 OR 
            SUM(rTonKho)<>0";

            #endregion

            #region dieu kien

            var phongban = _nganSachService.GetPhongBan(username);
            var dkPhongban = getDkPhongBan(phongban.sKyHieu);
            //var dkDonvi = getDkDonViByUser(username);

            sql = string.Format(sql, dvt, dkPhongban);

            #endregion

            #region result

            var config = _nganSachService.GetCauHinh(username);
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iID_MaDonVi = donvi,
                         iNamLamViec = config.iNamLamViec,
                     },
                     commandType: CommandType.Text
                 );

                return dt;
            }

            #endregion
        }


        #endregion

        #region tong hop

        public DataTable TongHop(
            string username,
            string phongban,
            string lns,
            int dvt = 1000)
        {
            #region sql

            var sql =
@"SELECT    SUBSTRING(sLNS,1,3) as sLNS3,SUBSTRING(sLNS,1,5) as sLNS5,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            rTuChi      =0,
            rChiTapTrung=SUM(rTuChi/{0}),
            rPhanCap    =SUM(rPhanCap/{0}),
            rDuPhong    =SUM(rDuPhong/{0})
    FROM DT_ChungTuChiTiet
    WHERE   
            -- theo phongban
            {1}
            -- theo donvi
            {2}
            -- theo lns
            {3}
            iTrangThai=1  AND 
            iNamLamViec=@iNamLamViec
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
    HAVING  SUM(rTuChi)<>0 OR 
            SUM(rChiTapTrung)<>0 OR 
            SUM(rPhanCap)<>0 OR 
            SUM(rDuPhong)<>0";

            #endregion

            #region dieu kien

            var dkPhongban = getDkPhongBan(phongban);
            var dkDonvi = getDkDonViByUser(username);
            var dkLns = getDkLns(lns);

            sql = string.Format(sql, dvt, dkPhongban, dkDonvi, dkLns);

            #endregion

            #region result

            var config = _nganSachService.GetCauHinh(username);
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iNamLamViec = config.iNamLamViec,
                     },
                     commandType: CommandType.Text
                 );

                return dt;
            }

            #endregion
        }

        #endregion

        public DataTable GetDonviListByLNS(string username, string lns)
        {
            var config = _nganSachService.GetCauHinh(username);
            var phongban = _nganSachService.GetPhongBan(username);

            var dkPhongBan = getDkPhongBan(phongban.sKyHieu);
            var dkDonVi = getDkDonVi(_nganSachService.GetDonviListByUser(username, config.iNamLamViec).Select(x => x.iID_MaDonVi));
            var dkLns = getDkLNS(phongban.sKyHieu, config.iNamLamViec.ToString(), lns);

            var sql =
            #region sql
@"SELECT a.iID_MaDonVi,a.iID_MaDonVi+' - '+sTen as TenHT 
FROM (
    SELECT DISTINCT iID_MaDonVi 
        FROM    DT_ChungTuChiTiet 
        WHERE   
                {0} -- phongban
                {1} -- donvi
                {2} -- lns 
                iTrangThai=1  AND 
                iID_MaDonVi <>'' AND 
                iNamLamViec=@iNamLamViec AND
                (rTuChi<>0 OR rHienVat<>0 OR rDuPhong<>0 OR rPhanCap<>0 OR rHangNhap<>0 OR rHangMua<>0)
    UNION                           
    SELECT DISTINCT iID_MaDonVi 
        FROM    DT_ChungTuChiTiet_PhanCap 
        WHERE   {0} -- phongban
                {1} -- donvi
                {2} -- lns
                iTrangThai=1  AND 
                iID_MaDonVi <>'' AND 
                iNamLamViec=@iNamLamViec AND
                (rTuChi<>0 OR rHienVat<>0 OR rDuPhong<>0 OR rPhanCap<>0 OR rHangNhap<>0 OR rHangMua<>0)
                ) a
  
INNER JOIN (SELECT DISTINCT iID_MaDonVi,sTen 
                FROM NS_DonVi 
                WHERE   iTrangThai=1 AND 
                        iNamLamViec_DonVi=@iNamLamViec) as b
ON a.iID_MaDonVi=b.iID_MaDonVi ORDER BY a.iID_MaDonVi";

            #endregion

            sql = string.Format(sql, dkPhongBan, dkDonVi, dkLns);

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iNamLamViec = config.iNamLamViec,
                     }
                 );
                return dt;
            }
        }

        public string GetDonvisByLNS(string username, string lns, int namLamViec = 0)
        {
            var items = GetDonviListByLNS(username, lns);
            //var result = string.Join(",", items.Select(x => x.iID_MaDonVi));
            return "";
        }


        #region private methods

        private string getDkPhongBan(string iID_MaPhongBan)
        {
            return iID_MaPhongBan == "02" ?
                "" :
                string.Format("iID_MaPhongBan = '{0}' AND", iID_MaPhongBan);
        }

        private string getDkDonViByUser(string username)
        {
            var donvis = _nganSachService.GetDonviListByUser(username)
                .Select(x => x.iID_MaDonVi);
            return getDkDonVi(donvis);
        }
        private string getDkDonVi(IEnumerable<string> maDonVis)
        {
            return string.Format("(iID_MaDonVi IN ({0})) AND",
                maDonVis.Select(x => string.Format("'{0}'", x))
                    .Join());
        }

        private string getDkLNS(string iID_MaPhongBan, string iNamLamViec, string lns)
        {
            var lnsPhongban = _nganSachService.GetLNS(iID_MaPhongBan, iNamLamViec)
                .AsEnumerable()
                .Select(x => string.Format("'{0}'", x.Field<string>("sLNS")))
                .ToList()
                .Join();

            var lnsLike = string.IsNullOrWhiteSpace(lns) ?
                "" :
                string.Format("({0}) AND", lns.Split(',').Select(x => string.Format("sLNS LIKE '{0}%'", x)).Join(" OR "));

            return string.Format("(sLNS IN ({0})) AND {1}",
                lnsPhongban,
                lnsLike);
        }

        private string getDkLns(string lns)
        {
            if (string.IsNullOrWhiteSpace(lns))
                return string.Empty;

            var values = lns.Split(',').ToList();
            if (values.Count == 1)
            {
                var dkLns = lns.Length <= 3 ?
                    string.Format("(sLNS LIKE '{0}%') AND ", lns) :
                    string.Format("(sLNS IN ('{0}')) AND ", lns);

                return dkLns;
            }
            else
            {
                var dkLns = new List<string>();
                values.ForEach(x =>
                {
                    var s = x.Length <= 3 ?
                       string.Format("sLNS LIKE '{0}%'", x) :
                       string.Format("sLNS = '{0}'", x);
                    dkLns.Add(s);
                });
                return string.Format("({0}) AND ", dkLns.Join(" OR "));
            }


        }





        #endregion
    }
}
