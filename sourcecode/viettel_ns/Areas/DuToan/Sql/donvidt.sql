declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2020
declare @donvis nvarchar(500)						set @donvis = '00,03,10,11,12,13,14,15,17,18,19,20,21,22,23,24,29,30,31,33,34,35,40,41,42,43,44,45,46,47,48,50,51,52,53,54,55,56,57,61,65,66,67,68,69,70,71,72,73,75,76,77,78,79,80,81,82,83,84,87,88,89,90,91,92,93,94,95,96,97,98,99,A7,A8,B16,B5,B6,B8,C1,C2,C3,C4,HK,L1'
declare @Id_PhongBanDich nvarchar(20)				set @Id_PhongBanDich=null
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07,07,08,10'
declare @username nvarchar(20)						set @username=null


--#DECLARE#--/

SELECT Distinct	Id, Ten = iID_MaDonVi + ' - ' + dv.sTen 
FROM			(
				select	iID_MaDonVi 
				from	DT_ChungTuChiTiet 
				where	iNamLamViec = @nam						
						and (@Id_PhongBanDich is null or iID_MaPhongBanDich = @Id_PhongBanDich)
						and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
						and iTrangThai <> 0
						and LEN(iID_MaDonVi) = 2
						and (rTuChi + rHangMua + rHangNhap) <> 0
						and MaLoai <> 1
						and sLNS not like '8%'

				union all

				select	iID_MaDonVi 
				from	DT_ChungTuChiTiet_PhanCap 
				where	iNamLamViec = @nam						
						and (@Id_PhongBanDich is null or iID_MaPhongBanDich = @Id_PhongBanDich)
						and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
						and iTrangThai <> 0
						and LEN(iID_MaDonVi) = 2
						and MaLoai <> 1
						and (rTuChi) <> 0) as ct

				LEFT JOIN 
				(select iID_MaDonVi as Id, sTen from NS_DonVi where iNamLamViec_DonVi=@nam and 
					iID_MaDonVi in (select * from f_split(@donvis))) as dv ON dv.Id = ct.iID_MaDonVi
WHERE			Id is not null
				