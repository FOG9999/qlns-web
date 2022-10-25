declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=0
declare @sXauNoiMa int								set @sXauNoiMa=null

--###--

SELECT		sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, rTuChi=SUM(rTuChi/@dvt)
			, rHangNhap=SUM(rHangNhap/@dvt)
			, rDuPhong=SUM(rDuPhong/@dvt)
FROM		DT_ChungTuChiTiet
WHERE		iTrangThai in (1,2) 
			AND sLNS='1050000' 
			AND iNamLamViec=@iNamLamViec 
			@@DKDV @@DKPB 
GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING		SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rDuPhong)<>0
