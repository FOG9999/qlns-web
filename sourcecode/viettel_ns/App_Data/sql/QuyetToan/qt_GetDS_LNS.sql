
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @LNS nvarchar(20)							set @LNS='1'

--#DECLARE#--

SELECT	DISTINCT	A.sLNS, TenHT = A.sLNS +' - '+ A.sMoTa
					--sLNS = CASE WHEN A.sLNS LIKE '104%' THEN '104'
					--			ELSE A.sLNS END
					--,  TenHT = CASE WHEN A.sLNS LIKE '104%' THEN N'104 - Ngân sách bảo đảm'
					--		   ELSE A.sLNS +' - '+ A.sMoTa END
FROM				NS_MucLucNganSach as A 
					INNER JOIN NS_PhongBan_LoaiNganSach AS B 
					ON A.sLNS = B.sLNS 
WHERE				B.iID_MaPhongBan= @iID_MaPhongBan
					AND B.iTrangThai=1 
					AND LEN(A.sLNS)=7 
					-- ls: 27/8/2019
					AND sL=''
					AND A.sLNS LIKE @LNS + '%' 
					AND A.iNamLamViec=@iNamLamViec					
ORDER BY			sLNS
