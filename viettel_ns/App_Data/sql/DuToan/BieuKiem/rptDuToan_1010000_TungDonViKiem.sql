declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='08'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='11'
declare @id_donvi nvarchar(20)						set @id_donvi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @lns nvarchar(20)							set @lns='1020700'

--###--/

			SELECT		sLNS
						,sL
						,sK
						,sM
						,sTM
						,sTTM
						,sNG
						,sMoTa = dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG)
						,rTuChi=SUM(rTuChi/@dvt)
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai=1  
						AND sLNS='1010000'
						@@1
						@@2
						AND iID_MaDonVi =@iID_MaDonVi 
						AND iNamLamViec=@iNamLamViec 
						AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
			GROUP BY	sLNS
						,sL
						,sK
						,sM
						,sTM
						,sTTM
						,sNG
			HAVING		SUM(rTuChi/@dvt)<>0 