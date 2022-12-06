
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='3b6ad507-a318-405d-bb94-6e52e08e3849'

declare @sNG		nvarchar(200)					set @sNG = null
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47'

--###-- 


SELECT  iID_MaDonVi, TenDonVi,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,CT.sMoTa,
        sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
        rTuChi,
        rHienVat 
FROM 
(
    SELECT  iID_MaDonVi,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
            SUM(rTuChi/@dvt) AS rTuChi,
            SUM(rHienVat/@dvt) AS rHienVat 
    FROM    DTBS_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1  
            AND MaLoai<>'1' AND MaLoai <>'3'  
            AND sLNS LIKE '2%' 
            AND iNamLamViec=@iNamLamViec
            AND iID_MaNamNganSach=2
            AND iID_MaNguonNganSach=1
			AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
            --AND sNG IN (SELECT * FROM F_SplitString2(@sNG))  
            AND iID_MaPhongBanDich=@iID_MaPhongBan
            AND iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa 
    HAVING SUM(rTuChi)>0 OR SUM(rHienVat)>0 
    
   -- UNION 

   -- SELECT  iID_MaDonVi,
   --         sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
   --         sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
   --         SUM(rTuChi/@dvt) AS rTuChi,
   --         SUM(rHienVat/@dvt) AS rHienVat 
   -- FROM    DTBS_ChungTuChiTiet 
   -- WHERE   
   --         iTrangThai=1
   --         --AND iKyThuat=1 
   --         --AND MaLoai=1  
   --         AND iNamLamViec=@iNamLamViec
   --         AND iID_MaNamNganSach=@iID_MaNamNganSach
   --         AND iID_MaNguonNganSach=@iID_MaNguonNganSach
   --         AND sLNS LIKE '2%'
			--AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
   --         AND iID_MaPhongBanDich=@iID_MaPhongBan
			----AND iID_MaChungTu in (select * from f_split(@iID_MaChungTu))
   --         AND (  iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu)))
			--	or iID_MaChungTu in (select * from f_split(@iID_MaChungTu))) 
					
   -- GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa 
   -- HAVING  SUM(rTuChi)>0 OR SUM(rHienVat)>0

) AS CT  

INNER JOIN (
    SELECT iID_MaDonVi as dv_id, (iID_MaDonVi + ' - ' + sTen)  as TenDonVi
    FROM NS_DonVi 
    WHERE   iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec) AS NS_DonVi 

ON NS_DonVi.dv_id=CT.iID_MaDonVi

--where iID_MaDonVi=51
--order by NG
