declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='51'

--###--



SELECT	
		sLNS1 = LEFT(sLNS,1),
		sLNS3 = LEFT(sLNS,3),
		sLNS5 = LEFT(sLNS,5),
		sLNS,sL,sK,sM,sTM,sTTM,sNG
		,sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG
		,iID_MaDonVi
		,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi)
		,ChiTaiNganh	=SUM(ChiTaiNganh)		 
		,TuChi	=SUM(TuChi)	 
	FROM			
			(
				SELECT		sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi=left(iID_MaDonVi,2)
							,ChiTaiNganh =SUM(rTaiNganh)
							,TuChi	= sum(rTien)
				FROM		DT_CTNT
				where		Checked = 1 
							and NamLamViec = @iNamLamViec
							and iID_MaPhongBan in (select * from f_split(@Id_PhongBan))
				GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi					
			) as dt_chitieu
GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG
			,iID_MaDonVi
HAVING		SUM(ChiTaiNganh) <> 0 or SUM(TuChi) <> 0       
order by	iID_MaDonVi