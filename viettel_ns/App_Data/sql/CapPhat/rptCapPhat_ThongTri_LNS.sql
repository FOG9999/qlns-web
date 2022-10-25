declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @@sLNS nvarchar(20)							set @@sLNS = '101%'
declare @iID_MaCapPhat nvarchar(100)				set @iID_MaCapPhat = 'b0522861-1daf-4b32-a671-95013e3146dd'

--#DECLARE#--

SELECT	a.sLNS
		, a.sLNS+' - '+sMoTa as TenHT 
FROM 
        (SELECT DISTINCT sLNS 
		 FROM	CP_CapPhatChiTiet
         WHERE	iTrangThai=1  
				AND iID_MaCapPhat = @iID_MaCapPhat  
				AND iID_MaPhongBan = @iID_MaPhongBan  
				AND ( sLNS LIKE @@sLNS) 
				AND iNamLamViec = @iNamLamViec) as a

        INNER JOIN

        (SELECT sLNS,sMoTa 
		 FROM	NS_MucLucNganSach 
		 WHERE	iTrangThai=1 
				AND LEN(sLNS)=7 
				AND sL='' 
				AND iNamLamViec=@iNamLamViec) as b
        ON a.sLNS = b.sLNS