
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(max)					set @iID_MaDonVi='79,80,81,82,83,84,87,88,89,90,91,92,93,94,95,96,97,98,99,B17'
--declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4'

--###--

declare @iID_MaChungTu nvarchar(MAX)
set @iID_MaChungTu = [dbo].[f_ns_dtbs_chungtu_gom_todate](@iNamLamViec,'1,2,4,5',@iID_MaPhongBan,GETDATE())

select  sLNS1,
		sLNS3, 
        sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi, 
		sTenDonVi,
		PhongBan=@iID_MaPhongBan,
		sMoTa_PhongBan,
        sum(HT_Cap) as HT_Cap,
        sum(HT_QuyetToan) as HT_QuyetToan,
        sum(CT_NamTruoc) as CT_NamTruoc,
        sum(CT_DauNam) as CT_DauNam,
        sum(CT_BoSung) as CT_BoSung,
        sum(CP_Cap) as CP_Cap,
        sum(CP_Thu) as CP_Thu,
        sum(QT_rTuChi) as QT_rTuChi from
(

SELECT  sLNS1	=LEFT(sLNS,1),
		sLNS3	=CASE    
                WHEN sLNS=1050100 then 101
                WHEN sLNS LIKE '102%' then 102
                else  LEFT(sLNS,3) END, 
        sLNS5	=CASE    
                WHEN sLNS=1050100 then 10100
                WHEN sLNS LIKE '102%' then 10200
                else  LEFT(sLNS,5) END, 
        sLNS	=CASE    
                WHEN sLNS=1050100 then 1010000
                WHEN sLNS LIKE '102%' AND sLNS <> '1020200' then 1020100
                else sLNS END, 
        sL,sK,sM,sTM,sTTM,sNG,
        DT1.iID_MaDonVi, 
        sum(HT_Cap) as HT_Cap,
        sum(HT_QuyetToan) as HT_QuyetToan,
        sum(CT_NamTruoc) as CT_NamTruoc,
        sum(CT_DauNam) as CT_DauNam,
        sum(CT_BoSung) as CT_BoSung,
        sum(CP_Cap) as CP_Cap,
        sum(CP_Thu) as CP_Thu,
        sum(QT_rTuChi) as QT_rTuChi
from
(

/*

HOP THUC CAP NAM TRUOC

*/

--1. Hợp thức cấp
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        HT_CAP.iID_MaDonVi, 
        sum(rTuChi) as HT_Cap,
        0 as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM (
  
	 -- nam sau cap ve
	SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
			iId_MaDonVi,
            rTuChi=	sum(rTuChi)
    FROM f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'1,5',null,GETDATE(),1,null)
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi


) as HT_CAP
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, HT_CAP.iID_MaDonVi

UNION ALL

--2. Hợp thức quyết toán

SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        HT_QuyetToan.iID_MaDonVi, 
        0 as HT_Cap,
        sum(rTuChi) as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM (
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        SUM(rTuChi) as rTuChi
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND iID_MaNamNganSach in (select * from f_split('1,5'))
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
		AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,sTenDonVi

) as HT_QuyetToan
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, HT_QuyetToan.iID_MaDonVi

/*

CHI TIEU NAM TRUOC

*/

--UNION ALL

--SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
--        CT_NAMTRUOC.iID_MaDonVi, 
--        0 as HT_Cap,
--        0 as HT_QuyetToan,
--        sum(rTuChi) as CT_NamTruoc,
--        0 as CT_DauNam,
--        0 as CT_BoSung,
--        0 as CP_Cap,
--        0 as CP_Thu,
--        0 as QT_rTuChi
--FROM (

---- du toan dau nam
--    SELECT
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,
--		rTuChi=	sum(rTuChi)
--    FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4,null,GETDATE(),1,null)
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


--) as CT_NAMTRUOC
--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, CT_NAMTRUOC.iID_MaDonVi

UNION ALL


/*

DU TOAN NAM NAY

*/
-- du toan
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        CT.iID_MaDonVi, 
        0 as HT_Cap,
        0 as HT_QuyetToan,
        sum(CT_NamTruoc)	as CT_NamTruoc,
        sum(rTuChi)			as CT_DauNam,
        sum(rBS)			as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM (


	-- NĂM TRƯỚC CHUYỂN SANG
    SELECT      
			sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            CT_NamTruoc=	sum(rTuChi),
            rTuChi=	0,
            0 as rBS
    FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, '4',null,GETDATE(),1,NULL)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0

    -- DU TOAN DAU NAM

	UNION ALL
    SELECT  
			sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			CT_NamTruoc = 0,
            rTuChi=	sum(rTuChi),
            0 as rBS
    FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,GETDATE(),1,1)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0
 
    
    --//////////////////////////////////////////////////////////////////
    -- DU TOAN BO SUNG

    UNION ALL
    SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			CT_NamTruoc = 0,
            rTuChi=0,
            rBS=sum(rTuChi)
       
    FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,GETDATE(),1,2)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0 
	 
) as ct


GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, CT.iID_MaDonVi 

UNION ALL

-- QUYET TOAN                   
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,

		iID_MaDonVi,
        0 as HT_Cap,
        0 as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        sum(rTuChi) as QT_rTuChi

FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
        --AND iThang_Quy IN (1,2)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
HAVING SUM(rTuChi)<>0 
--order by iID_MaDonVi
-- END 101


UNION ALL
-- CAP PHAT              
SELECT  
		sLNS,sL,sK,sM,sTM,sTTM,sNG,

        iID_MaDonVi,
        0 as HT_Cap,
        0 as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        sum(rTuChi) as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM    CP_CapPhatChiTiet
WHERE   iTrangThai=1 
        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
		AND sLNS <> ''
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

) as dt1
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


) as DT

--left join (select distinct sLNS as id,sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and sL='' and sLNS=sXauNoiMa) as mlns on dt.sLNS=mlns.id



-- lay ten phongban
inner join (select sKyHieu as id, sMoTa as sMoTa_PhongBan from NS_PhongBan where iTrangThai=1) as phongban
on phongban.id=@iID_MaPhongBan


-- lay ten donvi
inner join (select iID_MaDonVi as dv_id, sTen as sTenDonVi from NS_DonVi where iNamLamViec_DonVi=@iNamLamViec and iTrangThai=1) as dv
on dv.dv_id=dt.iID_MaDonVi

where  
        sLNS1 in (1,2,3,4)
        --sLNS in(1010000)
        --AND sLNS not in(1050000)
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))

		--and slns like '102%'
group by sLNS1, sLNS3, sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG, iID_MaDonVi,sTenDonVi,sMoTa_PhongBan
having	SUM(HT_Cap)<>0 
		or sum(HT_QuyetToan)<>0
		or sum(CT_NamTruoc)<>0
		or sum(CT_DauNam) <>0
		or sum(CT_BoSung) <>0
		or sum(CP_Cap) <>0
		or sum(CP_Thu) <>0
		or sum(QT_rTuChi) <>0
ORDER BY PhongBan, sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


--select * from F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan)
