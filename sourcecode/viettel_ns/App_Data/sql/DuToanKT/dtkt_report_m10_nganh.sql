
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @id_phongban nvarchar(20)					set @id_phongban='10'
declare @lns		nvarchar(200)					set @lns = '104,109'
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47,10'
declare @id_nganh		nvarchar(200)				set @id_nganh = '23'


--###--


SELECT DISTINCT
        sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
FROM    DT_ChungTuChiTiet 
WHERE   
        iTrangThai=1
        --AND iBKhac=0  
        --AND iKyThuat=1 
        --AND MaLoai=1  
        AND iNamLamViec=@nam
        AND iID_MaNamNganSach=2
        AND iID_MaNguonNganSach=1
        AND (@lns is null or left(sLNS,3) in (select * from f_split(@lns)))
        --AND sNG IN (SELECT * FROM F_Split(@sNG))  
		AND sNG=@id_nganh
        AND iID_MaPhongBanDich=@id_phongban


UNION

-- phan cap
SELECT DISTINCT 
            sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG 
FROM    DT_ChungTuChiTiet_PhanCap 
WHERE   sLNS='1020100' 
        AND iTrangThai=1    
        --AND iBKhac=0 
        AND MaLoai<>'1' AND MaLoai <>'3'  
        AND iNamLamViec=@nam
        AND iID_MaNamNganSach=2
        AND iID_MaNguonNganSach=1
        --AND sNG IN (SELECT * FROM F_Split(@sNG))  
		AND sNG=@id_nganh
        AND iID_MaPhongBanDich=@id_phongban

ORDER BY sM,sTM,sTTM,sNG
