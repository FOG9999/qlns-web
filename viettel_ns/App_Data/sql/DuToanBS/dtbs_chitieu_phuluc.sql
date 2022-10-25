
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
--declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='b99eb11d-d5d2-404c-9e53-086908d23f0c,2d4c4bde-b477-433d-b747-65cd7cfe709e,c9491a39-907e-4b9e-b9d0-f6cc4ddc8323,33f41452-5079-4709-869a-569c52fd41de,c0547e70-5683-40eb-ad20-b24d2a007078'
declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='65ccb586-16d2-4052-91d2-4f10a756bd28,85b2711c-620a-47ae-8eda-5d97dc2b1b63,3b6ad507-a318-405d-bb94-6e52e08e3849,1b20b59c-5fbc-42fa-b118-6f1ba92a2b2e,905ae383-805d-4ea0-841c-92770a725bb5,e644bb6c-0483-4c00-bbe7-93707a97d750,dbd8c50d-807c-4baa-a46d-ae19764539b9,c0547e70-5683-40eb-ad20-b24d2a007078'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='52'
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='10'
declare @sLNS nvarchar(100)							set @sLNS=''

--#DECLARE#--



SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa
		,sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
		rTongSo		=SUM(rTuChi+rHangNhap+rHangMua+rTonKho+rPhanCap+rDuPhong)/@dvt,
        rTuChi      =SUM(rTuChi)/@dvt, 
        rHienVat    =SUM(rHienVat)/@dvt, 
        rHangNhap   =SUM(rHangNhap)/@dvt, 
        rHangMua    =SUM(rHangMua)/@dvt, 
        rTonKho     =SUM(rTonKho)/@dvt, 
        rPhanCap    =SUM(rPhanCap)/@dvt, 
        rDuPhong    =SUM(rDuPhong)/@dvt
FROM
(

SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
        iID_MaDonVi,
        rTuChi      =SUM(rTuChi), 
        rHienVat    =SUM(rHienVat), 
		rHangNhap	=SUM(rHangNhap),
		rHangMua	=SUM(rHangMua),
        rTonKho    =SUM(rTonKho), 
        rPhanCap    =SUM(rPhanCap), 
        rDuPhong    =SUM(rDuPhong)
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND (MaLoai='' or MaLoai='2')
		AND iID_MaPhongBanDich=@iID_MaPhongBan
        AND iID_MaDonVi=@iID_MaDonVi
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) 

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rHienVat)<>0 
        OR SUM(rTonKho)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 

UNION
                        
SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
        iID_MaDonVi,
        rTuChi      =SUM(rTuChi), 
        rHienVat    =SUM(rHienVat), 
		rHangNhap	=SUM(rHangNhap),
		rHangMua	=SUM(rHangMua),
        rTonKho    =SUM(rTonKho),
        rPhanCap    =SUM(rPhanCap),
        rDuPhong    =SUM(rDuPhong)
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND (MaLoai='' or MaLoai='2')
		AND iID_MaPhongBanDich=@iID_MaPhongBan
		AND iID_MaDonVi=@iID_MaDonVi
        AND (
			iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
			-- phan cap cho cac bql
			--OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
			--						where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

			---- phan cap cho b
			--OR 	iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
			--					 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))


			-- phan cap lan 2
			--OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
			--							from DTBS_ChungTuChiTiet 
			--							where iID_MaChungTu in (
			--										select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
			--										where iID_MaChungTu in (   
			--																select iID_MaChungTu from DTBS_ChungTu
			--																where iID_MaChungTu in (
			--																						select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
			--																						where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))
			
			)

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rTonKho)<>0 
		OR SUM(rHienVat)<>0
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 


) as T1
WHERE (@sLNS is null 
		or LEFT(sLNS,1) in (select * from f_split(@sLNS))
		or LEFT(sLNS,3) in (select * from f_split(@sLNS)))
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
