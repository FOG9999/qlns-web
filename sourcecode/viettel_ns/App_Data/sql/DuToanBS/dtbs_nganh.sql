
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(200)				set @iID_MaPhongBan='10' 
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @sLNS nvarchar(200)							set @sLNS='104%'
declare @sNG nvarchar(200)							set @sNG='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu=''

--#DECLARE#--

SELECT  CT.iID_MaDonVi, CT.iID_MaDonVi+' - '+ NS_DonVi.sTen AS TenDonVi,
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
            AND sLNS LIKE @sLNS_pc
            AND iNamLamViec=@iNamLamViec
            AND iID_MaNamNganSach=2
            AND iID_MaNguonNganSach=1
            AND sNG IN (SELECT * FROM F_Split(@sNG))  
            AND iID_MaPhongBanDich=@iID_MaPhongBan
            AND iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa 
    HAVING SUM(rTuChi)>0 OR SUM(rHienVat)>0 
    
    UNION 
    SELECT  iID_MaDonVi,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
            SUM(rTuChi/@dvt) AS rTuChi,
            SUM(rHienVat/@dvt) AS rHienVat 
    FROM    DTBS_ChungTuChiTiet 
    WHERE   
            iTrangThai=1
            AND iKyThuat=1 
            AND MaLoai=1  
            AND iNamLamViec=@iNamLamViec
            AND iID_MaNamNganSach=2
            AND iID_MaNguonNganSach=1
            AND sLNS LIKE @sLNS
            AND sNG IN (SELECT * FROM F_Split(@sNG))  
            AND iID_MaPhongBanDich=@iID_MaPhongBan
            AND iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa 
    HAVING  SUM(rTuChi)>0 OR SUM(rHienVat)>0
) AS CT  

INNER JOIN (
    SELECT iID_MaDonVi as MaDonVi, sTen 
    FROM NS_DonVi 
    WHERE   iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec) AS NS_DonVi 
ON NS_DonVi.MaDonVi=CT.iID_MaDonVi
