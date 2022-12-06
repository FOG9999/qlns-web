
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='93399357-8d59-4510-a5be-410d1d3a8439,291ded86-deb6-48eb-a47f-5dab21580f22,50d47c74-fa66-4cf5-9a23-8c1b61169d59,9a32c1d6-fd51-4427-bcb5-8f8f51597a73,35047e92-7d57-4c61-9068-d9b2f18e5272,116f29fe-e549-4b47-9bd7-e0e9f79c9565,577d4467-c681-4577-8c1b-f21b0d44fbbd,0f8cf7a4-a157-47c1-ad6e-caceba3f1331'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null

--#DECLARE#--
declare @dn nvarchar(MAX)
set @dn = '30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94'

SELECT		iID_MaDonVi,sTenDonVi=[dbo].F_TenDonVi(@iNamLamviec, iID_MaDonVi),
			C1      =SUM(C1)/@dvt,
			C2      =SUM(C2)/@dvt
FROM
(

SELECT		iID_MaDonVi,
			C1      = case when iID_MaPhongBanDich <> '06' and iID_MaDonVi not in (select * from f_split(@dn)) then SUM(rTuChi+rHangMua+rHangNhap+rChiTaiNganh+rDuPhong) else 0 end,
			C2      = case when iID_MaPhongBanDich = '06' or iID_MaDonVi in (select * from f_split(@dn)) then SUM(rTuChi+rHangMua+rHangNhap) else 0 end
FROM		DTBS_ChungTuChiTiet 
WHERE		iTrangThai=1 
			AND iNamLamViec=@iNamLamViec
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) 

GROUP BY	iID_MaDonVi,iID_MaPhongBanDich
HAVING		SUM(rTuChi+rHangMua+rHangNhap+rChiTaiNganh+rDuPhong)<>0 

UNION ALL
                        
SELECT		iID_MaDonVi,
			C1      = case when iID_MaPhongBanDich <> '06' and iID_MaDonVi not in (select * from f_split(@dn)) then SUM(rTuChi) else 0 end,
			C2      = case when iID_MaPhongBanDich = '06' or iID_MaDonVi in (select * from f_split(@dn)) then SUM(rTuChi) else 0 end
FROM		DTBS_ChungTuChiTiet_PhanCap 
WHERE		iTrangThai=1 
			AND iNamLamViec=@iNamLamViec
			AND (MaLoai='' or MaLoai='2')
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND (
				iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
				-- phan cap cho cac bql
				OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

				---- phan cap cho b
				OR 	iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))


				-- phan cap lan 2
				OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
											from DTBS_ChungTuChiTiet 
											where iID_MaChungTu in (
														select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
														where iID_MaChungTu in (   
																				select iID_MaChungTu from DTBS_ChungTu
																				where iID_MaChungTu in (
																										select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																										where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))
			
				)

GROUP BY	iID_MaDonVi,iID_MaPhongBanDich
HAVING		SUM(rTuChi)<>0 

UNION ALL
                        
SELECT		iID_MaDonVi,
			C1      = case when iID_MaPhongBanDich <> '06' and iID_MaDonVi not in (select * from f_split(@dn)) then SUM(rHangNhap + rHangMua + rTuChi) else 0 end,
			C2      = case when iID_MaPhongBanDich = '06' or iID_MaDonVi in (select * from f_split(@dn)) then SUM(rHangNhap + rHangMua + rTuChi) else 0 end
FROM		DTBS_ChungTuChiTiet
WHERE		iTrangThai=1 
			AND iNamLamViec=@iNamLamViec
			AND MaLoai='2'
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND (
				iID_MaChungTu in (	select iID_MaChungTuChiTiet 
											from DTBS_ChungTuChiTiet 
											where iID_MaChungTu in (
														select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
														where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))
			
				)

GROUP BY	iID_MaDonVi,iID_MaPhongBanDich
HAVING		SUM(rHangNhap + rHangMua + rTuChi)<>0 
) as T1

left join 

(
select		iID_MaDonVi as id, iSTT
from		NS_DonVi
where		iTrangThai = 1 and iNamLamViec_DonVi = @iNamLamViec
) as dv

on			T1.iID_MaDonVi = dv.id
GROUP BY	iID_MaDonVi,iSTT
order by	iSTT