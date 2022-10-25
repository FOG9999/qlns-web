declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='06'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='03,10,12,14,15,17,19,29,30,33,34,341,35,41,44,47,50,501,51,52,53,61,69,70,71,72,73,78,80,88,89,90,91,92,93,94,99,B6,HK'

--###--

declare @dssLNS nvarchar(max)
set @dssLNS = '2040100,2040206,2040207,2040208,2040210,2040212,2040213,2040214,2040215,2040216,2040401,2040402,2130100,2130200,2130201,2050100,2060100,2060101,2060102,2060300,2180100,2180200,2020201,2020202,2020300,2020302,2020303,2070100,2070200,2070300,2070400,2070500,2070600,2070800'

SELECT	
		sLNS1 = LEFT(sLNS,1),
		sLNS3 = LEFT(sLNS,3),
		sLNS5 = LEFT(sLNS,5),
		sLNS,sL,sK,sM,sTM,sTTM,sNG
		,sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG
		,iID_MaDonVi
		,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi)
		,DuToan	=SUM(DuToan)		 
		,TuChi	=SUM(TuChi)	 
	FROM			
			(
				SELECT		sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi=left(iID_MaDonVi,2)
							,DuToan =SUM(rTien+rTaiNganh)
							,TuChi	=0
				FROM		DT_CTNT
				where		Checked = 1 
							and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
							--and (@Id_PhongBan is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
							and iID_MaDonVi in (select * from f_split(@Id_DonVi))
							and sLNS in (select * from f_split(@dssLNS))
							and NamNS=(@iNamLamViec-1)
				GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,iID_MaPhongBanDich


				union all
				SELECT		sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi=left(iID_MaDonVi,2)
							,DuToan =0
							,TuChi	=SUM(rTien+rTaiNganh)
				FROM		DT_CTNT
				where		Checked = 1 
							and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
							--and (@Id_PhongBan is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
							and iID_MaDonVi in (select * from f_split(@Id_DonVi))
							and sLNS in (select * from f_split(@dssLNS))
							and NamNS=(@iNamLamViec)
				GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,iID_MaPhongBanDich
	
			) as dt_chitieu
GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG
			,iID_MaDonVi
HAVING		SUM(DuToan) <> 0 or SUM(TuChi) <> 0       
order by	iID_MaDonVi