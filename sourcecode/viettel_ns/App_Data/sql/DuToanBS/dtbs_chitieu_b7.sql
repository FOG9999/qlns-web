
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
--declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='df49e1d6-4fe3-4e20-8560-1c1fcf2e2b36,793e6855-7ce3-4edb-83c9-2c6313c214be,b428fad1-014b-48bc-b507-4bd8f65d737f,e0e9875a-8445-4af4-8295-6ba2ffdc1296,518383d8-4e8b-468f-8961-6c68dfb0c6ac'
declare @iID_MaChungTu nvarchar(2000)				set @iID_MaChungTu='a8588d54-9e8b-4c60-9a4e-1582cd039878,80f6d308-d92c-48c4-b9ed-28600731ab93,f9500237-3bc5-43b0-be34-314477515bfb,7d4bcfb8-e74c-4c12-95e2-8263f53b986b,64baee13-bcef-4852-8e00-887292633781,ec1caa79-38b2-4451-b085-a24f60ffc122,bf5940d4-b43e-4553-b6e0-afbb59caa816,6cbefddd-959c-4aad-a35c-c6ca555c0a34,424074de-4c4e-4235-89f9-c795db5fc688,8f9b02fc-cda8-46df-8df0-dc2bf6524837'
--declare @iID_MaChungTu nvarchar(2000)				set @iID_MaChungTu='8f9b02fc-cda8-46df-8df0-dc2bf6524837'

declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @sLNS nvarchar(100)							set @sLNS='1,2'

--#DECLARE#--



SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sMota = dbo.f_mlns_mota_xau(@iNamLamviec,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG),
        iID_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
		rTongSo		=SUM(rTuChi)/@dvt,
        rTuChi      =SUM(rTuChi)/@dvt, 
        rHangNhap   =SUM(rHangNhap)/@dvt, 
        rHangMua    =SUM(rHangMua)/@dvt, 
        rHienVat    =SUM(rHienVat)/@dvt, 
        rPhanCap    =SUM(rPhanCap)/@dvt, 
        rDuPhong    =SUM(rDuPhong)/@dvt
FROM
(

SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        --rTuChi      =SUM(rTuChi+rHangNhap+rHangMua), 
        rTuChi		=SUM(rTuChi), 
        rHangNhap    =SUM(rHangNhap), 
        rHangMua    =SUM(rHangMua), 
        rHienVat    =SUM(rHienVat), 
        rPhanCap    =SUM(rPhanCap), 
        rDuPhong    =SUM(rDuPhong)
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND (MaLoai='' or MaLoai='2')
        AND iID_MaDonVi=@iID_MaDonVi
		--and iID_MaPhongBanDich = @iID_MaPhongBan
        --AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
		 AND (
			iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
			OR iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))))
		and iID_MaChungTuChiTiet not in ('EC789F2F-ED12-462D-B21A-59AC039E59C0','F3749BCF-67CF-4AF2-A600-010A17950A87','1A98C3BD-6AF1-49C9-9DF0-21622A752A7F','95FE3F82-3F13-4C3B-82A2-3E6DBDBCBC3C')
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
HAVING  SUM(rTuChi+rHangNhap+rHangMua)<>0 
        OR SUM(rHienVat)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 

UNION ALL
                        
SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        --rTuChi      =SUM(rTuChi+rHangNhap+rHangMua), 
		rTuChi		=SUM(rTuChi), 
        rHangNhap    =SUM(rHangNhap), 
        rHangMua    =SUM(rHangMua), 
        rHienVat    =SUM(rHienVat), 
        rPhanCap    =SUM(rPhanCap), 
        rDuPhong    =SUM(rDuPhong)
FROM	f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,@iID_MaChungTu) 
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
--HAVING  
--		SUM(rTuChi+rHangNhap+rHangMua)<>0 
--        SUM(rTuChi)<>0 
--        OR SUM(rHienVat)<>0 
--        OR SUM(rPhanCap)<>0 
--        OR SUM(rDuPhong)<>0 
) as a

 --lay mota lns
left join 
(select sXauNoiMa as mlns_id, sMoTa from dbo.f_mlns_mota2(@iNamLamViec)) as mlns
on mlns.mlns_id=a.sLNS+'-'+a.sL+'-'+a.sK+'-'+a.sM+'-'+a.sTM+'-'+a.sTTM+'-'+a.sNG

WHERE (@sLNS is null or LEFT(sLNS,1) in (select * from f_split(@sLNS))) 
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


HAVING  SUM(rTuChi)<>0         