declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='02' 

--#DECLARE#--/

select		NS_MucLucNganSach.sLNS
			, NS_MucLucNganSach.sLNS + ' - ' + NS_MucLucNganSach.sMoTa as TenLNS 
FROM		NS_MucLucNganSach 
			INNER JOIN NS_PhongBan_LoaiNganSach ON NS_PhongBan_LoaiNganSach.sLNS = NS_MucLucNganSach.sLNS 
			INNER JOIN NS_PhongBan ON NS_PhongBan.iID_MaPhongBan = NS_PhongBan_LoaiNganSach.iID_MaPhongBan
WHERE		iNamLamViec = @iNamLamViec AND sKyHieu = @iID_MaPhongBan
			AND NS_MucLucNganSach.iTrangThai=1 AND sL = '' 
ORDER BY	sLNS
