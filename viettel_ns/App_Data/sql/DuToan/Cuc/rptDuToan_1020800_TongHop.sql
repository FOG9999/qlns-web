declare @dvt int									set @dvt = 1000
declare @nam int								set @nam = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @lns int									set @lns=1020700


/*

author:		hiep
date:		09/12/2018
desc:		Chi khoa học nền

*/

--###--

SELECT		sTen = CASE iID_MaDonVi WHEN 65 THEN 'TTNĐV-N' 
									WHEN 75 THEN 'VKHCN-QS'
									WHEN 83 THEN 'HVCT'
									ELSE '' END
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, STTM
			, sNG
			, sTNG
			, sMoTa = dbo.F_MLNS_MoTa_LNS(@nam,'1020800',sL,sK,sM,sTM,sTTM,sNG)	
			, SUM(rTuChi)/@dvt AS rTuChi
			, SUM(rHienVat/@dvt) AS rHienVat 
FROM		DT_ChungTuChiTiet 
WHERE		iTrangThai in (1,2)        
			and sLNS = '1020800'        
			and iNamLamViec=@nam
GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaDonVi
HAVING		SUM(rTuChi)>0 OR 
			SUM(rHienVat)>0