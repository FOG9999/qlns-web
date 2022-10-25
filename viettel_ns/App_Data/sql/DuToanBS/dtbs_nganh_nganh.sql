
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(200)				set @iID_MaPhongBan='10' 
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @sLNS nvarchar(200)							set @sLNS='104%'
declare @sNG nvarchar(200)							set @sNG='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--#DECLARE#--


SELECT DISTINCT 
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   sLNS LIKE @sLNS 
        AND iTrangThai=1    
        AND iBKhac=0 
        AND MaLoai<>'1' AND MaLoai <>'3'  
        AND iNamLamViec=@iNamLamViec
        AND iID_MaNamNganSach=@iID_MaNamNganSach
        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
        AND sNG IN (SELECT * FROM F_Split(@sNG))  
        AND iID_MaPhongBanDich=@iID_MaPhongBan
        
UNION 
SELECT DISTINCT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
        sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
FROM    DTBS_ChungTuChiTiet 
WHERE   
        iTrangThai=1
        AND iBKhac=0  
        AND iKyThuat=1 
        AND MaLoai=1  
        AND iNamLamViec=@iNamLamViec
        AND iID_MaNamNganSach=@iID_MaNamNganSach
        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
        AND sLNS LIKE @sLNS
        AND sNG IN (SELECT * FROM F_Split(@sNG))  
        AND iID_MaPhongBanDich=@iID_MaPhongBan
ORDER BY sM,sTM,sTTM,sNG
