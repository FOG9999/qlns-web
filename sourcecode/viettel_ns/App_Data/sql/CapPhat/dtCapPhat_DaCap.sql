declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = GetDate()
declare @sLNS nvarchar(MAX)							set @sLNS = '1010000'
declare @iSoCapPhat nvarchar(100)					set @iSoCapPhat = '1260'

--#DECLARE#--/

SELECT		iID_MaDonVi
			,@@select
			,SUM(rTuChi) as rTuChi
from		(
			select	iID_MaDonVi
					, sLNS
					, sL
					, sK
					, sM
					, sTM
					, sTTM
					, sNG
					, rTuChi = Case iID_MaTinhChatCapThu WHEN 2 THEN -1 * rTuChi ELSE rTuChi END
			FROM		CP_CapPhatChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec = @iNamLamViec 
						AND @@Tien_HienVat = 1 
						AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND (@iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND (@iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach))) 
						AND iID_MaCapPhat IN (SELECT iID_MaCapPhat 
											  FROM	 CP_CapPhat 
											  WHERE  ((dNgayCapPhat < @dNgayCapPhat) 
													 OR (dNgayCapPhat=@dNgayCapPhat AND (@iSoCapPhat IS NULL OR iSoCapPhat<@iSoCapPhat))))  
			) as r	  
GROUP BY	iID_MaDonVi
			, @@MucCap
ORDER BY	@@MucCap
			, iID_MaDonVi




                          
                     

