
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='2'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='08'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='7766267b-b46e-4cf1-a190-332213a70ee3'

declare @sNG		nvarchar(200)					set @sNG = NULL
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47'

--###-- 

select	iID_MaDonVi, TenDonVi, sMoTa,
		sLNS,sL,sK, NG,
		rTuChi		=SUM(rTuChi),		
		rHienVat	=SUM(rHienVat), 
		id = NG +'-'+sLNS

from
(

SELECT  iID_MaDonVi, TenDonVi,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
		--CT.sMoTa,
        --sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,

		sMoTa = case when (sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG in (select * from f_split('7150.7151.00.38,7150.7164.00.38,7150.7166.00.38'))) then N'Chi Pháp lệnh Người có công'
				else ct.sMoTa end,

		NG = case when (sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG in (select * from f_split('7150.7151.00.38,7150.7164.00.38,7150.7166.00.38'))) then '8000.0000.00.00'
				else sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG end,

        rTuChi,
        rHienVat

FROM 
(
   -- SELECT  iID_MaDonVi,
   --         sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
   --         sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
   --         SUM(rTuChi/@dvt) AS rTuChi,
   --         SUM(rHienVat/@dvt) AS rHienVat 
   -- FROM    DTBS_ChungTuChiTiet_PhanCap 
   -- WHERE   iTrangThai=1  
   --         AND MaLoai<>'1' AND MaLoai <>'3'  
   --         AND sLNS LIKE '2%' 
   --         AND iNamLamViec=@iNamLamViec
   --         AND iID_MaNamNganSach=2
   --         AND iID_MaNguonNganSach=1
			--AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
   --         --AND sNG IN (SELECT * FROM F_SplitString2(@sNG))  
   --         AND iID_MaPhongBanDich=@iID_MaPhongBan
   --         AND iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu)))
   -- GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa 
   -- HAVING SUM(rTuChi)>0 OR SUM(rHienVat)>0 
    
   -- UNION 

    SELECT  iID_MaDonVi,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
            SUM(rTuChi/@dvt) AS rTuChi,
            SUM(rHienVat/@dvt) AS rHienVat 
    FROM    DTBS_ChungTuChiTiet 
    WHERE   
            iTrangThai=1
            --AND iKyThuat=1 
            --AND MaLoai=1  
            AND iNamLamViec=@iNamLamViec
            AND iID_MaNamNganSach=@iID_MaNamNganSach
            AND iID_MaNguonNganSach=@iID_MaNguonNganSach
            AND sLNS LIKE '2%'
			AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
            AND iID_MaPhongBanDich=@iID_MaPhongBan
			--AND iID_MaChungTu in (select * from f_split(@iID_MaChungTu))
            AND (  iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu)))
				or iID_MaChungTu in (select * from f_split(@iID_MaChungTu))) 
					
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa
    HAVING  SUM(rTuChi)>0 OR SUM(rHienVat)>0

) AS CT  

INNER JOIN (
    SELECT iID_MaDonVi as MaDonVi, TenDonVi = (iID_MaDonVi + ' - ' + sTen)
    FROM NS_DonVi 
    WHERE   iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec) AS NS_DonVi 
	ON NS_DonVi.MaDonVi=CT.iID_MaDonVi

) as t2

GROUP by iID_MaDonVi, TenDonVi, sMoTa,NG,sLNS,sL,sK,sM,sTM,sTTM
