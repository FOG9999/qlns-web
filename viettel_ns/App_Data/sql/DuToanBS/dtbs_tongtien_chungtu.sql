
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaChungTu nvarchar(2000)				set @iID_MaChungTu='a8588d54-9e8b-4c60-9a4e-1582cd039878,80f6d308-d92c-48c4-b9ed-28600731ab93,f9500237-3bc5-43b0-be34-314477515bfb,7d4bcfb8-e74c-4c12-95e2-8263f53b986b,64baee13-bcef-4852-8e00-887292633781,ec1caa79-38b2-4451-b085-a24f60ffc122,bf5940d4-b43e-4553-b6e0-afbb59caa816,6cbefddd-959c-4aad-a35c-c6ca555c0a34,424064de-4c4e-4235-89f9-c795db5fc688,8f9b02fc-cda8-46df-8df0-dc2bf6524837,d3e073db-2535-4faa-8810-12d064255408'

declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 

--#DECLARE#--


SELECT  SUM(TuChi) AS TuChi
FROM
(

SELECT (SUM(rTuChi) + SUM(rHienVat) + SUM(rHangNhap) + SUM(rHangMua) + SUM(rChiTaiNganh)) as TuChi
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
		AND (@iID_MaPhongban = '02' or (@iID_MaPhongban <> '02' and iID_MaPhongBan = iID_MaPhongBanDich))
        AND (MaLoai='' or MaLoai='2')        
		AND (iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
			OR iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))))

union all

SELECT  SUM(rTuChi) as TuChi
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
		AND (@iID_MaPhongban = '02' or (@iID_MaPhongban <> '02' and iID_MaPhongBanDich = @iID_MaPhongban))
        AND (MaLoai='' or MaLoai='2')        
		AND iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))

union all

SELECT  SUM(rTuChi) as TuChi
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
		AND (@iID_MaPhongban = '02' or (@iID_MaPhongban <> '02' and iID_MaPhongBanDich = @iID_MaPhongban))
        AND MaLoai='2'      
		and iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where  iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where  iTrangThai=1 and iID_MaChungTu  in (select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu where iTrangThai=1 and iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))))

union all

SELECT  (SUM(rTuChi) + SUM(rHienVat) + SUM(rHangNhap) + SUM(rHangMua)) as TuChi
FROM    DTBS_ChungTuChiTiet
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
		AND (@iID_MaPhongban = '02' or (@iID_MaPhongban <> '02' and iID_MaPhongBanDich = @iID_MaPhongban))
        AND MaLoai='2'      
		and iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where  iTrangThai=1 and iID_MaChungTu  in (select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu where iTrangThai=1 and iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))))
) as T1
