qt_thongtri_lns.sql
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iThang_Quy int								set @iThang_Quy = '1'
declare @username nvarchar(20)						set @username='chiendv'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @sLNS nvarchar(20)							set @sLNS='4'
DECLARE @iID_MaChungTu NVARCHAR(100)				SET @iID_MaChungTu='b1fec22e-04f7-4d43-a1b9-07167afd8bb2'
 
--###--

SELECT	a.sLNS
		, a.sLNS+' - '+sMoTa as TenHT 
FROM 
        (SELECT DISTINCT sLNS 
		 FROM	QTA_ChungTuChiTiet
         WHERE	iTrangThai=1  
				AND iID_MaChungTu = @iID_MaChungTu  
				AND (@sLNS is null or (@sLNS <> '102' and sLNS like + @sLNS +'%') or (@sLNS = '102' and (sLNS like '102%' or sLNS like '107%')))) as a

        INNER JOIN

        (SELECT sLNS,sMoTa 
		 FROM	NS_MucLucNganSach 
		 WHERE	iTrangThai=1 
				AND LEN(sLNS)=7 
				AND sL='' 
				AND iNamLamViec=@iNamLamViec) as b
        ON a.sLNS = b.sLNS
		 