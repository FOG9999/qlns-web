using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using ImportData.Flexcel;
using System;
using System.Data;
using QLTCController;
using System.Collections.Generic;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace ImportData.Forms.Reports
{
    public class MNSReportController : FlexcelReportController
    {
        private string _filePath, _bql;
        private int _dvt = 1000000;

        public MNSReportController(string bql)
        {
            _filePath = GetFilePath("MNS.xls");
            _bql = "05,06,07,08,10,16".Contains(bql) ? bql : "-1";
        }

        #region getdata

        public override ExcelFile Print()
        {
            var xls = new XlsFile(_filePath, true);

            try
            {
                #region fill report

                var fr = new FlexCelReport();
                loadReport(fr);

                fr.UseCommonValue(_dvt)
                    .UseForm()
                    .Run(xls);

                #endregion
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return xls;
        }

        private void loadReport(FlexCelReport fr)
        {

            var ds = MakeData();

            fr.SetValue("header1", "SỐ LIỆU NGÂN SÁCH CÁC NĂM 2016, 2017, 2018, 2019");

            fr.AddTable(ds.Tables[0])
              .AddTable(ds.Tables[1])
              .AddTable(ds.Tables[2])
              .AddTable(ds.Tables[3])
              .AddTable(ds.Tables[4])
              .AddTable(ds.Tables[5])
              .AddTable(ds.Tables[6])
              .AddTable(ds.Tables[7])
              .AddTable(ds.Tables[8])
              .AddTable(ds.Tables[9])
              .AddTable(ds.Tables[10])
              .AddTable(ds.Tables[11])
              .AddTable(ds.Tables[12])
              .AddTable(ds.Tables[13])
              .AddTable(ds.Tables[14])
              .AddTable(ds.Tables[15])
              .AddTable(ds.Tables[16]);
            //.AddTable(ds.Tables[17])
            //.AddTable(ds.Tables[18]);
        }

        private DataSet MakeData()
        {
            #region sv205
            var sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                             from
                                    (select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                    from [NSach].[dbo].[QToan]
                                    where namlv in (2016,2017) and lns like '101%'
                                            and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
							 group by Lns, dvi
                             order by Dvi";

            var dt101 = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt101.TableName = "dt101";

            dt101.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt101.Rows.Count; i++)
            {
                var ro = dt101.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '102%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Dvi";

            var dt102 = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt102.TableName = "dt102";

            dt102.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt102.Rows.Count; i++)
            {
                var ro = dt102.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '103%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Dvi";

            var dt103 = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt103.TableName = "dt103";

            dt103.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt103.Rows.Count; i++)
            {
                var ro = dt103.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select ng, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,ng,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '10401%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by ng, dvi
                        order by ng, Dvi";

            var dt10401ct = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt10401ct.TableName = "dt10401ct";

            dt10401ct.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt10401ct.Rows.Count; i++)
            {
                var ro = dt10401ct.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select ng, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,ng,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '10402%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by ng, dvi
                        order by ng, Dvi";

            var dt10402ct = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt10402ct.TableName = "dt10402ct";

            dt10402ct.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt10402ct.Rows.Count; i++)
            {
                var ro = dt10402ct.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select ng, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,ng,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '10403%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by ng, dvi
                        order by ng, Dvi";

            var dt10403ct = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt10403ct.TableName = "dt10403ct";

            dt10403ct.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt10403ct.Rows.Count; i++)
            {
                var ro = dt10403ct.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '105%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Lns, Dvi";

            var dt105 = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt105.TableName = "dt105";

            dt105.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt105.Rows.Count; i++)
            {
                var ro = dt105.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '109%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Lns, Dvi";

            var dt109 = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt109.TableName = "dt109";

            dt109.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt109.Rows.Count; i++)
            {
                var ro = dt109.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '3%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Lns, Dvi";

            var dt3 = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt3.TableName = "dt3";

            dt3.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt3.Rows.Count; i++)
            {
                var ro = dt3.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns = LEFT(LNS,3),dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '2%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Lns, Dvi";

            var dt2ct = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt2ct.TableName = "dt2ct";

            dt2ct.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt2ct.Rows.Count; i++)
            {
                var ro = dt2ct.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '4%' and lns not like '444%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Lns, Dvi";

            var dt4ct = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt4ct.TableName = "dt4ct";

            dt4ct.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt4ct.Rows.Count; i++)
            {
                var ro = dt4ct.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
                         from
                                (
                                select namlv,Lns,dvi,n2016=case when namlv = 2016 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2017 then TuChi/@dvt else 0 end
                                from [NSach].[dbo].[QToan]
                                where namlv in (2016,2017) and lns like '444%'
                                        and dvi in (select dvi      
                                                        from [Dmuc2009].[dbo].[Donvi] where (@bql is null or bql = @bql))) as r
						group by Lns, dvi
                        order by Lns, Dvi";

            var dt444 = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
                {"dvt", _dvt.ToString()},
                {"bql", _bql.ToString()},
            });
            dt444.TableName = "dt444";

            dt444.Columns.Add(new DataColumn("donvi", typeof(string)));
            for (int i = 0; i < dt444.Rows.Count; i++)
            {
                var ro = dt444.Rows[i];
                ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            }

            //      sqlSv205 = @"select Lns, dvi, sum(n2016) as n2016, sum(n2017) as n2017
            //                   from
            //                          (
            //                          select namlv,Lns,dvi,n2016=case when namlv = 2017 then TuChi/@dvt else 0 end,  n2017=case when namlv = 2018 then TuChi/@dvt else 0 end
            //                          from CTieuBS
            //                          where namlv in (2017,2018) and nam in (1,2) and dvi not in ('','XX')) as r
            //group by Lns, dvi
            //                  order by Lns, Dvi";

            //      var dtnsct = Service.GetDataFromSql("NSach", sqlSv205, new Dictionary<string, string> {
            //          {"dvt", _dvt.ToString()},
            //      });
            //      dtnsct.TableName = "dtnsct";

            //      dtnsct.Columns.Add(new DataColumn("donvi", typeof(string)));
            //      for (int i = 0; i < dtnsct.Rows.Count; i++)
            //      {
            //          var ro = dtnsct.Rows[i];
            //          ro["donvi"] = Service.GetTenDonVi("NSach11", 2019, ro["Dvi"].ToString());
            //      }

            #endregion

            var sql = @"
                        select  sMoTa as MoTa, sNG as XauNoiMa                                
                        from    NS_MucLucNganSach
                        where   sLNS = '' and iTrangThai = 1 and iNamLamViec = 2019
					    order by sNG";

            var dtTenNganh = Service.GetDataFromSql("NSach11", sql);

            sql = @"
                        select  sMoTa as MoTa, sLNS as XauNoiMa                           
                        from    NS_MucLucNganSach
                        where   iNamLamViec = 2019 and iTrangThai = 1 and sM = '' and sNG = '' and LEN(sLNS) = 3 and sLNS like '2%'
					    order by sLNS";

            var dtLNSNN = Service.GetDataFromSql("NSach11", sql);

            sql = @"
                        select  sMoTa as MoTa, sLNS as XauNoiMa                          
                        from    NS_MucLucNganSach
                        where   iNamLamViec = 2019 and iTrangThai = 1 and sM = '' and sNG = '' and LEN(sLNS) = 7 and sLNS like '4%'
					    order by sLNS";

            var dtLNS4 = Service.GetDataFromSql("NSach11", sql);

            sql = @"
                        select  sMoTa as MoTa, sLNS as XauNoiMa                          
                        from    NS_MucLucNganSach
                        where   iNamLamViec = 2019 and iTrangThai = 1 and sM = '' and sNG = '' and LEN(sLNS) = 3
					    order by sLNS";

            var dtLNS = Service.GetDataFromSql("NSach11", sql);

            var ds = new DataSet();
            ds.Tables.Add(dt101.Copy());
            ds.Tables.Add(dt102.Copy());
            ds.Tables.Add(dt103.Copy());
            dt10401ct = Service.AddMoTa(dt10401ct, "dt10401ct", dtTenNganh, "Ng", "MoTa", "XauNoiMa");
            ds.Tables.Add(dt10401ct.Copy());
            ds.Tables.Add(dt10401ct.SelectDistinct("dt10401", "dvi,donvi"));
            dt10402ct = Service.AddMoTa(dt10402ct, "dt10402ct", dtTenNganh, "Ng", "MoTa", "XauNoiMa");
            ds.Tables.Add(dt10402ct.Copy());
            ds.Tables.Add(dt10402ct.SelectDistinct("dt10402", "dvi,donvi"));
            dt10403ct = Service.AddMoTa(dt10403ct, "dt10403ct", dtTenNganh, "Ng", "MoTa", "XauNoiMa");
            ds.Tables.Add(dt10403ct.Copy());
            ds.Tables.Add(dt10403ct.SelectDistinct("dt10403", "dvi,donvi"));
            ds.Tables.Add(dt105.Copy());
            ds.Tables.Add(dt109.Copy());
            ds.Tables.Add(Service.AddMoTa(dt2ct.SelectDistinct("dt2", "Lns"), "dt2", dtLNSNN, "Lns", "MoTa", "XauNoiMa"));
            ds.Tables.Add(dt2ct.Copy());
            ds.Tables.Add(dt3.Copy());
            ds.Tables.Add(Service.AddMoTa(dt4ct.SelectDistinct("dt4", "Lns"), "dt4", dtLNS4, "Lns", "MoTa", "XauNoiMa"));
            ds.Tables.Add(dt4ct.Copy());
            ds.Tables.Add(dt444.Copy());
            //ds.Tables.Add(Service.AddMoTa(dtnsct, "dtns", dtLNS, "Lns", "MoTa", "XauNoiMa"));
            //ds.Tables.Add(dtnsct);


            return ds;
        }

        #endregion

    }
}
