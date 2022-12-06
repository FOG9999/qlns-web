
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='c165b25e-e340-41ed-86cd-04caa0bc1b7f,7e10789c-90bd-4a4e-8532-08aff64ba2de,74fd04b2-4fb6-42f2-b46f-0ac60aa3d1e3,7d6b48da-bcf1-4c70-ad9c-0c63127f534e,a909a30e-7ef6-4f20-a6f3-0e1c3dc509a6,c699f6d8-b58d-495c-b407-13df0d5a6973,45800f8d-2a4d-4815-a4d3-22c75ffcab81,f13ba2f6-7504-4fb7-bc38-2a85d7597501,9e297582-c39d-48e4-ad52-302ca496a432,52ad6ec5-093b-40c0-b48e-369f03496484,3f267b1d-eb70-4949-946b-4d6f8d87d83f,89553264-b4e8-4e19-a66d-4d81f94469aa,50436d1f-ffe4-437a-91e7-591a3ade7352,e4e35aa7-b0fc-4685-a881-5b129a351dce,b3d8a5b7-aa7f-4356-a80d-5c1e374d47b5,b06c860d-135e-4fa9-b7ba-6bc5aa0af71c,22ec41b5-2235-4a22-bbc0-9978025cf56b,76d1de82-02e9-4326-980d-a63eb96ea5f3,4b1fa5b2-4352-43be-8bce-a71fdb874025,e7d49f5b-57ce-4a36-97f8-ae37381badc2,e9fb8a2d-746f-4722-b15a-b0cfc647a409,7ae573ab-82c0-487b-836f-b854ec5f5aac,f1228511-2a9e-4a73-8ed4-ba58cb607764,22d6bb04-f49d-4ed1-b09b-c656d82a881b,11f4b601-fa56-4be0-b4ea-c7554eddd88a,105ca7b2-2f31-41f9-98c8-c7be908d04f4,b7678fd1-5f8e-4691-97d8-d1dd6eec8724,cf38718f-648f-4867-9dcb-d7336adba04d,a5c069bd-7fe6-4fa3-8153-d9309e90ebfc,2a047fca-b184-4286-8d04-df5c41dd6bdf,8bed96cd-48be-45b6-9c52-e3941dab1357,975b881a-22ed-4994-bb58-e4cce2ba0c3f,6cc9e7f4-23c6-4233-b096-ea951cd4bcf0,05fe3875-5b6e-4c77-af24-eb465b5f445b,c7b229ec-ab8a-4023-b3cd-ee935ae855ff,7aaa700b-8264-40ad-9a6b-eea5ebb9309a,e98e2690-3b03-4c01-a7da-f50478965850,cf05d0d2-b410-4ef3-af33-b1b90bbb4f65'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null

--#DECLARE#--


declare @dn nvarchar(MAX)
set @dn = '30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94'

SELECT		iID_MaDonVi = Ma_Dv ,sTenDonVi=[dbo].F_TenDonVi(@iNamLamviec, Ma_Dv), iID_MaPhongBan = iID_MaPhongBanDich, SX=iID_MaPhongBanDich + '-'+Ma_Dv,
			C1      =SUM(C1)/@dvt,
			C2      =SUM(C2)/@dvt,
			C3      =SUM(C3)/@dvt,
			C4      =SUM(C4)/@dvt,
			C5      =SUM(C5)/@dvt,
			C6      =SUM(C6)/@dvt,
			C7      =SUM(C7)/@dvt,
			C8      =SUM(C8)/@dvt,
			C9      =SUM(C9)/@dvt,
			C10     =SUM(C10)/@dvt,
			C11     =SUM(C11)/@dvt,
			C12     =SUM(C12)/@dvt,
			C13     =SUM(C13)/@dvt,
			C14     =SUM(C14)/@dvt,
			C15     =SUM(C15)/@dvt,
			C16     =SUM(C16)/@dvt
FROM
(

SELECT		Ma_Dv = LEFT(iID_MaDonVi, 2), iID_MaPhongBanDich, sLNS, sL, sK, sM, sTM, sTTM, sNG,
			C1      = case when (sLNS = '1010000' or sLNS = '1020700') and sum(rTuChi) < 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C2      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) < 0 and sNG = '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C3      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) < 0 and sNG <> '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C4      = case when sLNS like '104%' and sum(rTuChi + rHangNhap + rHangMua) < 0 then sum(rTuChi + rHangNhap + rHangMua) else 0 end,
			C5      = case when ((LEN(iID_MaDonVi) > 2 and sLNS like '1%') or sLNS = '1020900') and sum(rTuChi) < 0 then sum(rTuChi) else 0 end,
			C6      = case when (sLNS like '109%' or sLNS like '105%' or sLNS = '1020200') and sum(rTuChi) < 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C7      = case when (sLNS = '1010000' or sLNS = '1020700') and sum(rTuChi) > 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C8      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) > 0 and sNG = '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C9      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) > 0 and sNG <> '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C10     = case when sLNS like '104%' and sum(rTuChi + rHangNhap + rHangMua) > 0 then sum(rTuChi + rHangNhap + rHangMua) else 0 end,
			C11     = case when ((LEN(iID_MaDonVi) > 2 and sLNS like '1%') or sLNS = '1020900') and sum(rTuChi) > 0 then sum(rTuChi) else 0 end,
			C12      = case when (sLNS like '109%' or sLNS like '105%' or sLNS = '1020200') and sum(rTuChi) > 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C13      = case when sLNS like '2%' and sum(rTuChi) < 0 then sum(rTuChi) else 0 end,
			C14      = case when sLNS like '2%' and sum(rTuChi) > 0 then sum(rTuChi) else 0 end,
			C15      = case when sLNS like '4%' and sum(rTuChi) < 0 then sum(rTuChi) else 0 end,
			C16      = case when sLNS like '4%' and sum(rTuChi) > 0 then sum(rTuChi) else 0 end
FROM		DTBS_ChungTuChiTiet 
WHERE		iTrangThai=1 
			AND iNamLamViec=@iNamLamViec
			AND (MaLoai='' or MaLoai='2')
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) 

GROUP BY	iID_MaDonVi, iID_MaPhongBanDich, sLNS, sL, sK, sM, sTM, sTTM, sNG
HAVING		SUM(rTuChi)<>0 
			or SUM(rHangNhap) <> 0
			or SUM(rHangMua) <> 0

UNION ALL
                        
SELECT		Ma_Dv = LEFT(iID_MaDonVi, 2), iID_MaPhongBanDich, sLNS, sL, sK, sM, sTM, sTTM, sNG,
			C1      = case when (sLNS = '1010000' or sLNS = '1020700') and sum(rTuChi) < 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C2      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) < 0 and sNG = '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C3      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) < 0 and sNG <> '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C4      = case when sLNS like '104%' and sum(rTuChi + rHangNhap + rHangMua) < 0 then sum(rTuChi + rHangNhap + rHangMua) else 0 end,
			C5      = case when ((LEN(iID_MaDonVi) > 2 and sLNS like '1%') or sLNS = '1020900') and sum(rTuChi) < 0 then sum(rTuChi) else 0 end,
			C6      = case when (sLNS like '109%' or sLNS like '105%' or sLNS = '1020200') and sum(rTuChi) < 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C7      = case when (sLNS = '1010000' or sLNS = '1020700') and sum(rTuChi) > 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C8      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) > 0 and sNG = '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C9      = case when sLNS like '1020%' and sLNS not in ('1020200','1020700','1020900') and sum(rTuChi) > 0 and sNG <> '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C10     = case when sLNS like '104%' and sum(rTuChi + rHangNhap + rHangMua) > 0 then sum(rTuChi + rHangNhap + rHangMua) else 0 end,
			C11     = case when ((LEN(iID_MaDonVi) > 2 and sLNS like '1%') or sLNS = '1020900') and sum(rTuChi) > 0 then sum(rTuChi) else 0 end,
			C12      = case when (sLNS like '109%' or sLNS like '105%' or sLNS = '1020200') and sum(rTuChi) > 0 and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C13      = case when sLNS like '2%' and sum(rTuChi) < 0 then sum(rTuChi) else 0 end,
			C14      = case when sLNS like '2%' and sum(rTuChi) > 0 then sum(rTuChi) else 0 end,
			C15      = case when sLNS like '4%' and sum(rTuChi) < 0 then sum(rTuChi) else 0 end,
			C16      = case when sLNS like '4%' and sum(rTuChi) > 0 then sum(rTuChi) else 0 end
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

GROUP BY	iID_MaDonVi,iID_MaPhongBanDich, sLNS, sL, sK, sM, sTM, sTTM, sNG
HAVING		SUM(rTuChi)<>0 

) as T1

left join 

(
select		iID_MaDonVi as id, iSTT
from		NS_DonVi
where		iTrangThai = 1 and iNamLamViec_DonVi = @iNamLamViec
) as dv

on			T1.Ma_Dv = dv.id
GROUP BY	Ma_Dv,iSTT, iID_MaPhongBanDich
order by	iID_MaPhongBanDich,iSTT