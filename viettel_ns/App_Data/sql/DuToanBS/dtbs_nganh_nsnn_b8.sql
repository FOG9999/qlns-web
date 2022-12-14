
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='2'
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='08'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='29'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='7766267b-b46e-4cf1-a190-332213a70ee3'
declare @dMaDot datetime							set @dMaDot = '2018-04-25 00:00:00.000'

declare @col		nvarchar(200)					set @col = 'SUM(rNgay)'
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47,10'
declare @sNG		nvarchar(200)					set @sNG = NULL
declare @loai		int								set @loai=0		--0: tat ca		1: phancap

--###--


select distinct
	sLNS, sL, sK,
	Ng, sMoTa,
	sNG = Ng + '(' + sL + '-' + sK + ')'
	,
	id = NG + '-' + sLNS
from
(


select  sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,
		sMoTa = case when (NG='7150.7151.00.38' 
						or NG='7150.7164.00.38' 
						or NG='7150.7166.00.38') then N'Chi Pháp lệnh Người có công'
				else dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG) end,

		NG = case when (NG='7150.7151.00.38' 
						or NG='7150.7164.00.38' 
						or NG='7150.7166.00.38') then '8000.0000.00.00'
				else NG end

--NG,sMoTa = dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG)

from
(

SELECT  
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG  AS NG 
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   
        iTrangThai=1    
        AND iBKhac=0 
        AND MaLoai<>'1' AND MaLoai <>'3'  
        AND iNamLamViec=@iNamLamViec
		AND (sLNS LIKE '2%' or sLNS like '4%')
        AND iID_MaNamNganSach=@iID_MaNamNganSach
        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
        AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
        AND iID_MaPhongBanDich=@iID_MaPhongBan
        AND iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
        
UNION 

SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,
        sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG  AS NG 
FROM    DTBS_ChungTuChiTiet 
WHERE   
        iTrangThai=1
        --AND iBKhac=0  
        --AND iKyThuat=1 
        --AND MaLoai=1  
        AND iNamLamViec=@iNamLamViec
        AND iID_MaNamNganSach=@iID_MaNamNganSach
        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
        AND iID_MaPhongBanDich=@iID_MaPhongBan
		AND (sLNS LIKE '2%' or sLNS like '4%')
        AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
		AND iID_MaChungTu in (select * from f_split(@iID_MaChungTu))
		AND (@loai is null or @loai=0)
) as t

)as T2

order by ng


--select distinct
--	Ng, sMoTa
	
--from
--(


--select  sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,
--		sMoTa = case when (NG='7150.7151.00.38' 
--						or NG='7150.7164.00.38' 
--						or NG='7150.7166.00.38') then N'Chi Pháp lệnh Người có công'
--				else dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG) end,

--		NG = case when (NG='7150.7151.00.38' 
--						or NG='7150.7164.00.38' 
--						or NG='7150.7166.00.38') then '8000.0000.00.00'
--				else NG end

----NG,sMoTa = dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG)

--from
--(

--SELECT  
--            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,
--            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
--FROM    DTBS_ChungTuChiTiet_PhanCap 
--WHERE   sLNS LIKE '2%' 
--        AND iTrangThai=1    
--        AND iBKhac=0 
--        AND MaLoai<>'1' AND MaLoai <>'3'  
--        AND iNamLamViec=@iNamLamViec
--        AND iID_MaNamNganSach=@iID_MaNamNganSach
--        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
--        AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
--        AND iID_MaPhongBanDich=@iID_MaPhongBan
        
--UNION 

--SELECT 
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,
--        sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
--FROM    DTBS_ChungTuChiTiet 
--WHERE   
--        iTrangThai=1
--        --AND iBKhac=0  
--        --AND iKyThuat=1 
--        --AND MaLoai=1  
--        AND iNamLamViec=@iNamLamViec
--        AND iID_MaNamNganSach=@iID_MaNamNganSach
--        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
--        AND iID_MaPhongBanDich=@iID_MaPhongBan
--        AND sLNS LIKE '2%' 
--        AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
--		AND iID_MaChungTu in (select * from f_split(@iID_MaChungTu))

--) as t

--)as T2
