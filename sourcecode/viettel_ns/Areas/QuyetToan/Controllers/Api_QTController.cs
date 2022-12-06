using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Dapper;
using Microsoft.Ajax.Utilities;
using NUnit.Framework.Constraints;
using Viettel.Extensions;
using Viettel.Services;
using VIETTEL.Areas.QuyetToan.Models.Api;

namespace VIETTEL.Areas.QuyetToan.Controllers
{

    #region Models

    #endregion

    public class QTController : ApiController, IQTApi
    {
        /// <summary>
        ///     asdfasdfasdf
        /// </summary>
        /// <param name="namLamViec"></param>
        /// <param name="namNganSach"></param>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="iThang_Quy"></param>
        /// <param name="iLoai"></param>
        /// <returns></returns>
        public IEnumerable<QTA_ChungTu> GetAll(
            int namLamViec,
            string namNganSach = "2",
            string id_phongban = null,
            string id_donvi = null,
            string iThang_Quy = null,
            string lns = null,
            string iLoai = null)
        {
            #region sql

            var sql = @"
select * from QTA_ChungTu
where   iTrangThai=1
        and iNamLamViec=@NamLamViec
        and (@NamNganSach is null or iID_MaNamNganSach in (select * from f_split(@NamNganSach)))
        and (@iLoai is null or iLoai in (select * from f_split(@iLoai)))
        and (@id_phongban is null or iID_MaPhongBan in (select * from f_split(@id_phongban)))
        and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
        and (@iThang_Quy is null or iThang_Quy in (select * from f_split(@iThang_Quy)))
        --and (@lns is null or sDSLNS like @lns +'%')

order by dNgayChungTu,iID_MaPhongBan
";

            #endregion

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var query = conn.Query<QTA_ChungTu>(sql,
                    new
                    {
                        namLamViec,
                        namNganSach,
                        id_phongban,
                        id_donvi,
                        iThang_Quy,
                        //lns,
                        iLoai
                    }).ToList();


                // tach chung tu bao dam: 1040100,1040200,1040300 -> 1040100; 1040200

                var listBD = query.ToList().Where(x =>
                    x.sDSLNS.IsNotEmpty() &&
                    x.sDSLNS.Length > 7 &&
                    (x.sDSLNS.Contains("1040100") || x.sDSLNS.Contains("1040200") || x.sDSLNS.Contains("1040300")))
                    .ToList();

                listBD.ForEach(x =>
                {
                    query.Remove(x);

                    x.sDSLNS.ToList()
                    .ForEach(l =>
                    {
                        var ct = x.Clone();
                        ct.sDSLNS = l;
                        ct.sTienToChungTu = $"{x.iID_MaChungTu}_{l}";
                        query.Add(ct);
                    });
                });

                #region loc theo LNS



                #endregion

                #region lay LNS co so lieu

                query.ToList()
                    .ForEach(x =>
                    {
                        if (x.sDSLNS.IsEmpty() || !x.sDSLNS.StartsWith("104"))
                        {

                            #region lns

                            var s = @"
select distinct sLNS 
from    QTA_ChungTuChiTiet
where   iTrangThai=1 
        and iID_MaChungTu=@iID_MaChungTu
        and (sLNS not like '104%')
";
                            x.sDSLNS = conn.Query<string>(s, new { x.iID_MaChungTu }).Join();
                            x.sTienToChungTu = x.iID_MaChungTu.ToString();

                            #endregion
                        }


                        #region TongTien

                        var r = @"
select  sum(rTuChi) 
from    QTA_ChungTuChiTiet
where  iTrangThai=1 
        and iID_MaChungTu=@iID_MaChungTu
        and (sLNS not like '104%' or sLNS=@LNS)
";
                        x.rTuChi = conn.QueryFirstOrDefault<double?>(r, new { x.iID_MaChungTu, lns = x.sDSLNS }).GetValueOrDefault();

                        #endregion
                    });

                #endregion

                var list = new List<QTA_ChungTu>();
                var lnsList = lns.ToList();
                if (lnsList.Any())
                {
                    query.ToList()
                    .ForEach(r =>
                    {
                        var ok = r.sDSLNS.ToList().Intersect(lnsList).Any();
                        if (ok)
                        {
                            if (r.sTienToChungTu.Length < 10) r.sTienToChungTu = r.iID_MaChungTu.ToString();
                            list.Add(r);
                        }
                    });
                    query = list;
                }


                return query;
            }
        }

        public IEnumerable<QTA_ChungTuChiTiet> GetById(string id_chungtu)
        {
            #region sql

            var sql = @"
select 
    iID_MaPhongBan,
    sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,  
  
    rTuChi_ChiTieu      =0,
    rTuChi              =sum(rTuChi),
    rDonViDeNghi        =sum(rDonViDeNghi),
    rVuotChiTieu        =sum(rVuotChiTieu),
    rTonThatTonDong     =sum(rTonThatTonDong),
    rDaCapTien          =sum(rDaCapTien),
    rChuaCapTien        =sum(rChuaCapTien)

from QTA_ChungTuChiTiet
where   
    iTrangThai=1
    and iID_MaChungTu in (select * from f_split(@id_chungtu))
group by iID_MaPhongBan,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
";

            #endregion

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var query = conn.Query<QTA_ChungTuChiTiet>(sql,
                    new
                    {
                        id_chungtu
                    });

                return query ?? new List<QTA_ChungTuChiTiet>();
            }
        }

        public IEnumerable<QTA_ChungTuChiTiet> GetById(string id_chungtu, string lns)
        {
            var mlnsDic = new Dictionary<string, string>
            {
                {"lns", "sLNS"},
                {"lk", "sLNS,sL,sK"},
                {"m", "sLNS,sL,sK,sM"},
                {"tm", "sLNS,sL,sK,sM,sTM"},
                {"ng", "sLNS,sL,sK,sM,sTM,sTTM,sNG"}
            };

            #region sql

            var sql = @"
select 
    iID_MaPhongBan,
    iNamLamViec,
    @lns,
    rTuChi_ChiTieu      =0,
    rTuChi              =sum(rTuChi),
    rDonViDeNghi        =sum(rDonViDeNghi),
    rVuotChiTieu        =sum(rVuotChiTieu),
    rTonThatTonDong     =sum(rTonThatTonDong),
    rDaCapTien          =sum(rDaCapTien),
    rChuaCapTien        =sum(rChuaCapTien)
from QTA_ChungTuChiTiet
where   
    iTrangThai=1
    and iID_MaChungTu in (select * from f_split(@id_chungtu))
group by iID_MaPhongBan,iNamLamViec,@lns

";

            #endregion

            if (!mlnsDic.ContainsKey(lns))
            {
                lns = "ng";
            }

            lns = mlnsDic[lns];
            sql = sql.Replace("@lns", lns);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var query = conn.Query<QTA_ChungTuChiTiet>(sql,
                    new
                    {
                        id_chungtu
                    });

                #region lay mo ta

                if (query != null && query.Count() > 0)
                {
                    var namLamViec = query.First().iNamLamViec;
                    query.ToList()
                        .ForEach(e =>
                        {
                            var xauNoiMa = new List<string> { e.sLNS, e.sL, e.sK, e.sM, e.sTM, e.sTTM, e.sNG }
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Join("-");
                            e.sXauNoiMa = xauNoiMa;
                            e.sMoTa = NganSachService.Default.GetMLNS_MoTa_ByXauNoiMa(namLamViec.ToString(), xauNoiMa);
                        });
                }

                #endregion

                return query;
            }
        }

        public string Get(string id)
        {
            return id;
        }
    }
}