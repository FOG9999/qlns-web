declare @dvt int									set @dvt = @dvt
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='08'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @lns nvarchar(20)							set @lns='1020700'

--###--/


SELECT		iID_MaNganh
			,sTenNganh
			,sLNS
			,sL
			,sK
			,sM
			,sTM
			,sTTM
			,sNG
			,sMoTa = dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG)
			,iID_MaDonVi
			,rDonVi=SUM(rDonVi)
			,rBoSung=SUM(rBoSung)
			,rNganh=SUM(rNganh)
			,rPhanCap=SUM(rPhanCap)
			,Loai = CASE WHEN sNG = '00' THEN 0
						 ELSE 1 END
			,sLoai = CASE WHEN sNG = '00' THEN 'Ngành 00'
					     ELSE 'Ngành NV #' END
FROM
			(
			SELECT		sLNS = '1020100'
						,sL
						,sK
						,sM
						,sTM
						,sTTM
						,sNG
						,iID_MaDonVi
						,rDonVi=SUM(rTuChi/@dvt)
						,rBoSung=0
						,rNganh=0
						,rPhanCap=0
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai=1  
						AND (sLNS='1020100' OR sLNS='1020000')  
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
						,iID_MaDonVi
			HAVING		SUM(rTuChi/@dvt)<>0
 
			UNION ALL

			SELECT		sLNS = '1020100'
						,sL
						,sK
						,sM
						,sTM
						,sTTM
						,sNG
						,iID_MaDonVi
						,rDonVi=0
						,rBoSung=SUM(rTuChi/@dvt)
						,rNganh=0
						,rPhanCap=0
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai=2						
						AND (sLNS='1020100' OR sLNS='1020000')  
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
						,iID_MaDonVi
			HAVING		SUM(rTuChi/@dvt)<>0

			UNION ALL

			SELECT		sLNS = '1020100'
						,sL
						,sK
						,sM
						,sTM
						,sTTM
						,sNG
						,iID_MaDonVi
						,rDonVi=0
						,rBoSung=0
						,rNganh=SUM(rTuChi/@dvt)
						,rPhanCap=0
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai=3						
						AND (sLNS='1020100' OR sLNS='1020000')  
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
						,iID_MaDonVi
			HAVING		SUM(rTuChi/@dvt)<>0
 
			UNION ALL
		 
			SELECT		sLNS = '1020100'
						,sL
						,sK
						,sM
						,sTM
						,sTTM
						,sNG
						,iID_MaDonVi
						,rDonVi=0
						,rBoSung=0
						,rNganh=0
						,rPhanCap=SUM(rTuChi/@dvt)
			FROM		DT_ChungTuChiTiet_PhanCap
			WHERE		iTrangThai=1  
						AND MaLoai in ('','2')
						AND (sLNS='1020100' OR sLNS='1020000')  
						@@1
						@@2
						AND iID_MaDonVi = @iID_MaDonVi AND iNamLamViec=@iNamLamViec
						AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
			GROUP BY	sLNS
						,sL
						,sK
						,sM
						,sTM
						,sTTM
						,sNG
						,sMoTa
						,iID_MaDonVi
			HAVING		SUM(rTuChi/@dvt)<>0) a,
			(select sNG as iID_MaNganh, sMoTa as sTenNganh from NS_MucLucNganSach where iTrangThai = 1 and iNamLamViec=@iNamLamViec and sLNS = '') as b
WHERE		b.iID_MaNganh = a.sNG	
GROUP BY	iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
ORDER BY	iID_MaNganh
			,sM
			,sTM
			,sTTM
			,sNG
