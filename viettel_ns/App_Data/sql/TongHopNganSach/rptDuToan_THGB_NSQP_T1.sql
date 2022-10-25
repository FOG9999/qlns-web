
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
--declare @iID_MaDonVi nvarchar(max)					set @iID_MaDonVi='79,80,81,82,83,84,87,88,89,90,91,92,93,94,95,96,97,98,99,B17'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4'

--###--

declare @iID_MaChungTu nvarchar(MAX)
set @iID_MaChungTu = [dbo].[f_ns_dtbs_chungtu_gom_todate](@iNamLamViec,'1,2,4',@iID_MaPhongBan,GETDATE())


declare @ng nvarchar(max)
select @ng=iID_MaNganhMLNS from NS_MucLucNganSach_Nganh
where	iTrangThai=1 and iNamLamViec=@iNamLamViec 
		and iID_MaNganh=@iID_MaDonVi

select  sLNS1,
		sLNS3, 
        sLNS5,
        sLNS,
		--sMoTa = [dbo].F_MLNS_MoTa(@iNamLamViec,sLNS),
		--sMoTa = case sLNS when 1040400 then N'Ngân sách bảo đảm - Phân cấp' else mlns.sMoTa end,
		--sMoTa=mlns.sMoTa,
        iID_MaDonVi, 
        sTenDonVi = [dbo].F_GetTenDonVi(@iNamLamViec,iID_MaDonVi),

        sum(CT_NamTruoc)/@dvt as NamTruoc,
        sum(CT_NamTruoc2)/@dvt as NamTruoc2,
        sum(CT_NamTruoc3)/@dvt as NamTruoc3,

        sum(CT_DauNam)/@dvt as CT_DauNam,
        sum(CT_BoSung)/@dvt as CT_BoSung,

        sum(QT_rTuChi)/@dvt as QT_rTuChi from
(

SELECT  sLNS1	=LEFT(sLNS,1),
		sLNS3	=CASE    
                WHEN sLNS like '105%' then '105'
                WHEN sLNS LIKE '102%' then '102'
                else  LEFT(sLNS,3) END, 
        sLNS5	=CASE    
                WHEN sLNS like '2%' then LEFT(sLNS,3)+'00'
                WHEN sLNS like '109%' then '10900'
                WHEN sLNS like '105%' then '10500'
                WHEN sLNS LIKE '102%' then '10200'
                else  LEFT(sLNS,5) END, 
        sLNS	=CASE    
                WHEN sLNS like '3%' then LEFT(sLNS,3)+'000X'
                WHEN sLNS like '2%' then LEFT(sLNS,3)+'000X'
                WHEN sLNS like '109%' then '109000X'
                WHEN sLNS like '105%' then '105000X'
                WHEN sLNS LIKE '103%' then '103010X'
                WHEN sLNS LIKE '102%' then '102010X'
                WHEN sLNS LIKE '101%' then '101000X'
                else sLNS END, 
     
        DT1.iID_MaDonVi, 
        sum(CT_NamTruoc) as CT_NamTruoc,
        sum(CT_DauNam) as CT_DauNam,
        sum(CT_BoSung) as CT_BoSung,
        sum(CT_NamTruoc2) as CT_NamTruoc2,
        sum(CT_NamTruoc3) as CT_NamTruoc3,
        sum(QT_rTuChi) as QT_rTuChi
from
(

/*

DU TOAN NAM NAY

*/
-- du toan
SELECT  sLNS,
        CT.iID_MaDonVi, 
        0 as CT_NamTruoc,
        sum(rTuChi) as CT_DauNam,
        sum(rBS) as CT_BoSung,
        0 as CT_NamTruoc2,
        0 as CT_NamTruoc3,
        0 as QT_rTuChi
FROM (

    -- DU TOAN
	SELECT  sLNS, iId_MaDonVi,
            rTuChi=SUM(rTuChi) 
			,rBS=0 
    FROM f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,2,null,GETDATE(),1,1)
    GROUP BY sLNS, iId_MaDonVi

	-- DU TOAN BỔ SUNG
	UNION ALL
	SELECT  sLNS, iId_MaDonVi,
            rTuChi=0
			,rBS=SUM(rTuChi) 
    FROM f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,2,null,GETDATE(),1,2)
	WHERE	sLNS not like '3%'
    GROUP BY sLNS, iId_MaDonVi


	-- DU TOAN - phan cap toàn quân
	UNION ALL
	SELECT  sLNS='1040400'
			,iId_MaDonVi=@iID_MaDonVi
            ,rTuChi=SUM(rTuChi) 
			,rBS=0 
    FROM	f_ns_chitieu_full_tuchi(@iNamLamViec,NULL,NULL,2,null,GETDATE(),1,1)
	WHERE	
			iID_MaDonVi<>@iID_MaDonVi
			AND sLNS='1020100'
			AND sNG IN (select * from f_split(@ng))
    GROUP BY sLNS, iId_MaDonVi



	-- DU TOAN - phan cap bo sung
	UNION ALL
	SELECT  sLNS='1040400'
			,iId_MaDonVi=@iID_MaDonVi
            ,rTuChi=0
			,rBS=SUM(rTuChi) 
    FROM	f_ns_chitieu_full_tuchi(@iNamLamViec,NULL,NULL,2,null,GETDATE(),1,2)
	WHERE	
			iID_MaDonVi<>@iID_MaDonVi
			AND sLNS='1020100'
			AND sNG IN (select * from f_split(@ng))
    GROUP BY sLNS, iId_MaDonVi


	-- DU TOAN - phan cap bản thân
	UNION ALL
	SELECT  sLNS='1040500'
			,iId_MaDonVi=@iID_MaDonVi
            ,rTuChi=SUM(rTuChi) 
			,rBS=0 
    FROM	f_ns_chitieu_full_tuchi(@iNamLamViec,NULL,NULL,2,null,GETDATE(),1,1)
	WHERE	
			iID_MaDonVi=@iID_MaDonVi
			AND sLNS='1020100'
			AND sNG IN (select * from f_split(@ng))
    GROUP BY sLNS, iId_MaDonVi



	-- DU TOAN - phan cap bo sung
	UNION ALL
	SELECT  sLNS='1040500'
			,iId_MaDonVi=@iID_MaDonVi
            ,rTuChi=0
			,rBS=SUM(rTuChi) 
    FROM	f_ns_chitieu_full_tuchi(@iNamLamViec,NULL,NULL,2,null,GETDATE(),1,2)
	WHERE	
			iID_MaDonVi=@iID_MaDonVi
			AND sLNS='1020100'
			AND sNG IN (select * from f_split(@ng))
    GROUP BY sLNS, iId_MaDonVi

) as ct
GROUP BY sLNS, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi

UNION ALL

-- QUYET TOAN                   
SELECT  sLNS,
		iID_MaDonVi,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CT_NamTruoc2,
        0 as CT_NamTruoc3,
        sum(rTuChi) as QT_rTuChi

FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
        AND (@iID_MaDonVi is NULL OR iID_MaDonVi=@iID_MaDonVi)  
        AND iNamLamViec=@iNamLamViec-1
		AND iID_MaNamNganSach = 2
		--AND (sLNS<>'1020100' or	sNG not IN (select * from f_split(@ng)))
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 
--order by iID_MaDonVi
-- END 101


-- QUYET TOAN 2018 - PHAN CAP
UNION ALL
SELECT  sLNS='1040400',
		iID_MaDonVi=@iID_MaDonVi,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CT_NamTruoc2,
        0 as CT_NamTruoc3,
        sum(rTuChi) as QT_rTuChi

FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec-1
		AND iID_MaNamNganSach = 2
		AND sLNS='1020100'
		and iID_MaDonVi<>@iID_MaDonVi
		AND sNG IN (select * from f_split(@ng))
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

-- QUYET TOAN 2018 - PHAN CAP ban than
UNION ALL
SELECT  sLNS='1040500',
		iID_MaDonVi=@iID_MaDonVi,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CT_NamTruoc2,
        0 as CT_NamTruoc3,
        sum(rTuChi) as QT_rTuChi

FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec-1
		AND iID_MaNamNganSach = 2
		AND sLNS='1020100'
		and iID_MaDonVi=@iID_MaDonVi
		AND sNG IN (select * from f_split(@ng))
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

UNION ALL
-- CAP PHAT              
SELECT  
		sLNS,
        iID_MaDonVi,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        sum(rTuChi) as CT_BoSung,
        0 as CT_NamTruoc2,
        0 as CT_NamTruoc3,
        0 as QT_rTuChi
FROM    CP_CapPhatChiTiet
WHERE   iTrangThai=1 
        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
        AND (@iID_MaDonVi is NULL OR iID_MaDonVi=@iID_MaDonVi)  
        AND iNamLamViec=@iNamLamViec 
		AND iID_MaNamNganSach = 2
		and sLNS like '3%'
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 




/*

	lấy quyết toán từ csdl 205

*/



--- nam 2017
union all
SELECT  sLNS,
		iID_MaDonVi,
        CT_NamTruoc	=0,
        CT_DauNam	=0,
        CT_BoSung	=0,
        CT_NamTruoc2 =sum(rTuChi),
        CT_NamTruoc3	=0,
        QT_rTuChi	=0
FROM    o_qt(@iNamLamViec-2,@iID_MaDonVi,@iID_MaPhongBan,2,null,1)
GROUP BY sLNS,iID_MaDonVi
--order by iID_MaDonVi
-- END 101


-- QUYET TOAN 2017 - PHAN CAP
UNION ALL
SELECT  sLNS='1040400',
		iID_MaDonVi=@iID_MaDonVi,
         CT_NamTruoc	=0,
        CT_DauNam	=0,
        CT_BoSung	=0,
        CT_NamTruoc2 =sum(rTuChi),
        CT_NamTruoc3	=0,
        QT_rTuChi	=0
FROM    o_qt(@iNamLamViec-2,null,@iID_MaPhongBan,2,null,1)
WHERE    
		sLNS='1020100'
		and iID_MaDonVi<>@iID_MaDonVi
		AND sNG IN (select * from f_split(@ng))
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

---- QUYET TOAN 2018 - PHAN CAP ban than
UNION ALL
SELECT  sLNS='1040500',
		iID_MaDonVi=@iID_MaDonVi,
         CT_NamTruoc	=0,
        CT_DauNam	=0,
        CT_BoSung	=0,
        CT_NamTruoc2 =sum(rTuChi),
        CT_NamTruoc3	=0,
        QT_rTuChi	=0
FROM    o_qt(@iNamLamViec-2,@iID_MaDonVi,@iID_MaPhongBan,2,null,1)
WHERE    
		sLNS='1020100'
		and iID_MaDonVi=@iID_MaDonVi
		AND sNG IN (select * from f_split(@ng))
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 










--------------------------- END 205 --------------------- 



) as dt1
GROUP BY sLNS,iID_MaDonVi


) as DT

left join (select distinct sLNS as id,sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and sL='' and sLNS=sXauNoiMa) as mlns on dt.sLNS=mlns.id

where  
        sLNS1 in ('x','1','2','3','4')
        --AND sLNS not in(1050000)
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
		--and slns like 'x1040500%'
		--and iID_MaDonVi='51'
group by sLNS1, sLNS3, sLNS5,sLNS, iID_MaDonVi,sMoTa
having	sum(CT_NamTruoc)<>0
		or sum(CT_DauNam) <>0
		or sum(CT_BoSung) <>0
		or sum(CT_NamTruoc2) <>0
		or sum(CT_NamTruoc3) <>0
		or sum(QT_rTuChi) <>0
ORDER BY sLNS, iID_MaDonVi


--select * from F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan)
