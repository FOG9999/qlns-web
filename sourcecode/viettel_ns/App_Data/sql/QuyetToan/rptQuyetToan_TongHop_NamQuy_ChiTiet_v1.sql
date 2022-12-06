
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @iThang_Quy int								set @iThang_Quy='1'
declare @sLNS nvarchar(20)							set @sLNS='1010000'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--


SELECT 
    LEFT(a.sLNS,1) as sLNS1,
    LEFT(a.sLNS,3) as sLNS3,
    LEFT(a.sLNS,5) as sLNS5,
    a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
    iID_MaDonVi,
    sTenDonVi,
    SUM(rTuChi) as rTuChi,
    SUM(Quy1) as Quy1,
    SUM(Quy2) as Quy2,
    SUM(Quy3) as Quy3,
    SUM(Quy4) as Quy4
FROM
(
    -- quyet toan
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        iID_MaDonVi,
        (iID_MaDonVi + ' - ' + sTenDonVi) as sTenDonVi,
        rTuChi=0,
        Quy1=SUM(CASE WHEN (iThang_Quy=1) THEN rTuChi ELSE 0 END),
        Quy2=SUM(CASE WHEN (iThang_Quy=2) THEN rTuChi ELSE 0 END),
        Quy3=SUM(CASE WHEN (iThang_Quy=3) THEN rTuChi ELSE 0 END),
        Quy4=SUM(CASE WHEN (iThang_Quy=4) THEN rTuChi ELSE 0 END)
    FROM    QTA_ChungTuChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
            AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM F_SplitString2(@iID_MaNamNganSach))
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi = @iID_MaDonVi)
            
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi

    UNION ALL

    -- lay chi tieu
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        iID_MaDonVi,
        sTenDonVi,
        CT as rTuChi,
        0 as Quy1,
        0 as Quy2,
        0 as Quy3,
        0 as Quy4
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM F_SplitString2(@sLNS))) 
) a
WHERE LEFT(sLNS,1) IN (1,2,3,4)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
HAVING  SUM(rTuChi)<>0 OR
        SUM(Quy1)<>0 OR
        SUM(Quy2)<>0 OR
        SUM(Quy3)<>0 OR
        SUM(Quy4)<> 0
