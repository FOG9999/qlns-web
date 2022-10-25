declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--

SELECT		sNG
			, sMoTa
			, iID_MaDonVi
            , sTenDonVi
			, SUM(rTuChi) AS rTuChi
                                                     
FROM 
			(SELECT		sNG=(CASE
									WHEN (sNG=57) THEN '55' 
									WHEN (sNG IN (30,32,35,36)) THEN '30'
									ELSE sNG END)
						, sMoTa=(CASE 
									WHEN (sNG=57) THEN N'Vận tải' 
									WHEN (sNG IN (30,32,35,36)) THEN N'Chính trị'
									ELSE sMoTa END)	
						, iID_MaDonVi
						, sTenDonVi
						, SUM(rTuChi) AS rTuChi					
			FROM		CP_CapPhatChiTiet
			WHERE		iTrangThai=1 
						AND iNamLamViec=@iNamLamViec 
						AND sLNS = '' 
						AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi))) 
						AND iID_MaPhongBan = @iID_MaPhongBan
						AND iID_MaCapPhat = @iID_MaCapPhat
			GROUP BY	iID_MaDonVi
						, sTenDonVi
						, sNG
						, sMoTa
			HAVING		SUM(rTuChi)<>0) a,
			(SELECT iID_MaNganhMLNS as MaNganh 
			FROM	NS_MucLucNganSach_Nganh 
			WHERE	iTrangThai = 1 
					AND iNamLamViec=@iNamLamViec 
					AND iID_MaNganh IN (SELECT * FROM F_Split(@Nganh))) b
WHERE		MaNganh LIKE '%' + a.sNG + '%'
GROUP BY	iID_MaDonVi
			, sTenDonVi
			, sNG
			, sMoTa
ORDER BY	iID_MaDonVi
			, sTenDonVi
			, sNG
			, sMoTa