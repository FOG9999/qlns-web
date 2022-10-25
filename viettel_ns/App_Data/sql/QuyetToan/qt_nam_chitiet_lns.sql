
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='07'
declare @username nvarchar(20)						set @username='tranhnh'
declare @dNgayChungTu datetime						set @dNgayChungTu = '2018-04-25 00:00:00.000'

--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47'
declare @sLNS		nvarchar(200)					set @sLNS='2%'
declare @sL			nvarchar(3)						set @sL=NULL
declare @sK			nvarchar(3)						set @sK=NULL
declare @loai		nvarchar(3)						set @loai=1

--###--  


SELECT  sM,sTM
        ,rTuChi = case @loai when 0 then SUM(rTuChi) else sum(rTuChi+rVuotChiTieu) end
FROM    QTA_ChungTuChiTiet as t
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND left(sLNS,1) in (1,2)
		AND (@sLNS is null or sLNS like @sLNS)
		AND (@sL is null or sL=@sL)
		AND (@sK is null or sK=@sK)
		AND (rTuChi<>0 or rVuotChiTieu<>0)
		--and iID_MaNamNganSach in (1,2
GROUP BY sM,sTM
order by sM,sTM
