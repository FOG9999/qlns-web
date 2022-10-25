declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='40,51'

--###--

declare @dssLNS nvarchar(max)
set @dssLNS = '2040100,2040206,2040207,2040208,2040210,2040212,2040213,2040214,2040215,2040216,2040401,2040402,2130100,2130200,2130201,2050100,2060100,2060101,2060102,2060300,2180100,2180200,2020201,2020202,2020300,2020302,2020303,2070100,2070200,2070300,2070400,2070500,2070600,2070800'

SELECT	
		iID_MaDonVi
		,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi)
	FROM			
			(
				SELECT		distinct
							iID_MaDonVi=left(iID_MaDonVi,2)
				FROM		DT_CTNT
				where		Checked = 1 
							and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
							--and (@Id_PhongBan is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
							and sLNS in (select * from f_split(@dssLNS)) 
							and NamNS=(@iNamLamViec-1)
							and (rTien+rTaiNganh)<>0


				union 

				SELECT		distinct
							iID_MaDonVi=left(iID_MaDonVi,2)
				FROM		DT_CTNT
				where		Checked = 1 
							and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
							--and (@Id_PhongBan is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
							and sLNS in (select * from f_split(@dssLNS)) 
							and NamNS=(@iNamLamViec)
							and (rTien+rTaiNganh)<>0
	
			) as dt_chitieu		 
