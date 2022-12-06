
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='tranhnh'
declare @iID_MaPhongBan nvarchar(200)				set @iID_MaPhongBan='10' 
declare @iID_MaNganh nvarchar(20)					set @iID_MaNganh='33,43,40,30,31,32,33,35,36,37,38,39,45,46,47'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='23e2a77f-f920-4d83-a02b-4819c41a48a9,e869b682-03db-41c2-98c2-48bc5dfa827c,5984b5d5-fdd5-40a0-8f5d-adb2166fd406,3da6477f-120f-4ef0-9a31-c9758b790b05'
declare @sLNS nvarchar(200)							set @sLNS='2'



--#DECLARE#--

select	sLNS1
		,sLNS3	
		,sLNS5
		,sLNS,sL,sK,sM,sTM,sTTM,sNG,
		rTongSo			=sum(rTuChi+rPhanCap+rDuPhong)/@dvt,
        rTuChi			=SUM(rTuChi)/@dvt,
        rPhanCap		=SUM(rPhanCap)/@dvt,
        rGiamDuPhong    =SUM(rGiamDuPhong)/@dvt,
		rDuPhong		=SUM(rDuPhong)/@dvt
from 
		(
		SELECT  sLNS1=left(sLNS,1)
				,sLNS3=left(sLNS,3)		
				,sLNS5=left(sLNS,5)		
				,sLNS,sL,sK,sM,sTM,sTTM,sNG,
				rTuChi			=SUM(rChiTaiNganh),
				rPhanCap		=SUM(rPhanCap),
				rGiamDuPhong    =SUM(rDuPhong), 
				rDuPhong		=SUM(rDuPhong)
		FROM    DTBS_ChungTuChiTiet 
		WHERE   iTrangThai=1  
				AND iNamLamViec=@iNamLamViec
				AND (MaLoai='' OR MaLoai='2') 
				AND LEFT(sLNS,1) IN (select * from f_split(@slns))
				AND iID_MaPhongBanDich=@iID_MaPhongBan
				AND (@iID_MaNganh is null or sNG IN (select * from f_split(@iID_MaNganh)))
				AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))

		GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG
		HAVING  SUM(rChiTaiNganh) <>0
				OR SUM(rPhanCap) <>0
				OR SUM(rDuPhong) <>0

		union all

		SELECT  sLNS1
				,sLNS3	
				,sLNS5
				,sLNS,sL,sK,sM,sTM,sTTM,sNG,
				rTuChi			= 0,
				rPhanCap		= 0,
				rGiamDuPhong    = 0, 
				rDuPhong		= SUM(rDuPhong)
		FROM    [VIETTEL_NS1].[dbo].[f_ns_chitieu_full_decimal](@iNamLamViec,null,@iID_MaPhongBan,'2',null,@dot,1,null) 
		WHERE   LEFT(sLNS,1) IN (select * from f_split(@slns))
				AND (sXauNoiMa in (select sXauNoiMa from DTBS_ChungTuChiTiet 
													WHERE   iTrangThai=1  
															AND iNamLamViec=@iNamLamViec
															AND (MaLoai='' OR MaLoai='2') 
															AND LEFT(sLNS,1) IN (select * from f_split(@slns))
															AND iID_MaPhongBanDich=@iID_MaPhongBan
															AND (@iID_MaNganh is null or sNG IN (select * from f_split(@iID_MaNganh)))
															AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))))
		GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG
		HAVING  SUM(rDuPhong) <>0
		) as temp
group by sLNS1
		,sLNS3	
		,sLNS5
		,sLNS,sL,sK,sM,sTM,sTTM,sNG
having	SUM(rTuChi) <>0
		OR SUM(rPhanCap) <>0
		OR SUM(rGiamDuPhong) <>0
		OR SUM(rDuPhong) <>0
