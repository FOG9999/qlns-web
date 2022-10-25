
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07'
declare @iID_MaDonVi nvarchar(max)					set @iID_MaDonVi='10,11,12,29,79,80,81,82,83,84,87,88,89,90,91,92,93,94,95,96,97,98,99'

--###--

select  sLNS1,
		sLNS3, 
        sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi, 
		sTenDonVi,
		PhongBan=@iID_MaPhongBan,
		sMoTa_PhongBan,
        sum(CT_NamTruoc) as CT_NamTruoc,
        sum(CT_DauNam) as CT_DauNam,
        sum(CT_BoSung) as CT_BoSung
from
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
				sum(CT_NamTruoc) as CT_NamTruoc,
				sum(CT_DauNam) as CT_DauNam,
				sum(CT_BoSung) as CT_BoSung
		from
		(
				/*

				DU TOAN NAM NAY

				*/
				-- du toan
				SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
						CT.iID_MaDonVi, 
						sum(CT_NamTruoc)	as CT_NamTruoc,
						sum(rTuChi)			as CT_DauNam,
						sum(rBS)			as CT_BoSung
				FROM (


					-- NĂM TRƯỚC CHUYỂN SANG
					SELECT      
							sLNS,sL,sK,sM,sTM,sTTM,sNG,
							iID_MaDonVi,
							CT_NamTruoc=	sum(rTuChi),
							rTuChi=	0,
							0 as rBS
					FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi,@iID_MaPhongBan,4,null,GETDATE(),1,NULL)
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
					FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan,2,null,GETDATE(),1,1)
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
       
					FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan,2,null,GETDATE(),1,2)
					GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
					HAVING	SUM(rTuChi)<>0 
	 
				) as ct

				GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, CT.iID_MaDonVi 

		) as dt1
		GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

) as DT

-- lay ten phongban
inner join (select sKyHieu as id, sMoTa as sMoTa_PhongBan from NS_PhongBan where iTrangThai=1) as phongban
on phongban.id=@iID_MaPhongBan

-- lay ten donvi
inner join (select iID_MaDonVi as dv_id, sTen as sTenDonVi from NS_DonVi where iNamLamViec_DonVi=@iNamLamViec and iTrangThai=1) as dv
on dv.dv_id=dt.iID_MaDonVi

where  sLNS1 in (1,2,3,4)
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))

group by sLNS1, sLNS3, sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG, iID_MaDonVi,sTenDonVi,sMoTa_PhongBan
having	 sum(CT_NamTruoc)<>0
		or sum(CT_DauNam) <>0
		or sum(CT_BoSung) <>0
ORDER BY PhongBan,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
