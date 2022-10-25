
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='07'
declare @iID_MaChungTu nvarchar(1000)				set @iID_MaChungTu='56f2d846-c52c-447d-ae93-004b071b5bb2,eacfe458-a102-4dd8-8cb9-29737e61ff46,23e2a77f-f920-4d83-a02b-4819c41a48a9,e869b682-03db-41c2-98c2-48bc5dfa827c,5f7dc13c-5e8f-40e3-928e-58d232b12a89,5984b5d5-fdd5-40a0-8f5d-adb2166fd406,3cfcacb8-c111-40c0-b3da-c693ad537c9e'
declare @username nvarchar(20)						set @username='tranhnh'
declare @dNgayChungTu datetime						set @dNgayChungTu = '2018-04-25 00:00:00.000'

--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47'
declare @sLNS		nvarchar(200)					set @sLNS=null
declare @sL			nvarchar(3)						set @sL=NULL
declare @sK			nvarchar(3)						set @sK=NULL

--###--  


SELECT  sLNS3=left(sLNS,3),
        sLNS,sL,sK,sM,sTM
		--sXauNoiMa = (sLNS + '-' + sL+'-' + sK +'-'+sM+'-'+sTM),
        --SUM(rTuChi) as rTuChi 
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND left(sLNS,1) in (1,2)
		AND (@sLNS is null or sLNS like @sLNS)
		AND (@sL is null or sL=@sL)
		AND (@sK is null or sK=@sK)
		AND rTuChi<>0
		--and iID_MaNamNganSach=2
GROUP BY left(sLNS,3),sLNS,sL,sK,sM,sTM
