
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='29'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='23e2a77f-f920-4d83-a02b-4819c41a48a9'
declare @dMaDot datetime							set @dMaDot = '2018-04-18 00:00:00.000'

declare @sLNS		nvarchar(200)					set @sLNS = '104,109'
declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47,10'

--###--


SELECT DISTINCT 
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   sLNS='1020100' 
        AND iTrangThai=1    
        AND iBKhac=0 
        AND MaLoai<>'1' AND MaLoai <>'3'  
        AND iNamLamViec=@iNamLamViec
        AND iID_MaNamNganSach=@iID_MaNamNganSach
        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
        AND sNG IN (SELECT * FROM F_Split(@sNG))  
        AND iID_MaPhongBanDich=@iID_MaPhongBan
        
--UNION 
--SELECT DISTINCT
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
--        sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
--FROM    DTBS_ChungTuChiTiet 
--WHERE   
--        iTrangThai=1
--        AND iBKhac=0  
--        AND iKyThuat=1 
--        AND MaLoai=1  
--        AND iNamLamViec=@iNamLamViec
--        AND iID_MaNamNganSach=@iID_MaNamNganSach
--        AND iID_MaNguonNganSach=@iID_MaNguonNganSach
--        AND (@sLNS is null or left(sLNS,3) in (select * from f_split(@sLNS)))
--        AND sNG IN (SELECT * FROM F_Split(@sNG))  
--        AND iID_MaPhongBanDich=@iID_MaPhongBan
--ORDER BY sM,sTM,sTTM,sNG
