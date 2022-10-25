declare @nam int									set @nam = 2018
declare @isreadonly int								set @isreadonly = 1
declare @irequest nvarchar(MAX)						set @irequest = '1'
declare @iloai int									set @iloai = 1
declare @ilan nvarchar(MAX)							set @ilan = '1,2'
declare @id_phongban int							set @id_phongban = 10
declare @id_user nvarchar(MAX)						set @id_user = null
declare @id_donvi nvarchar(MAX)						set @id_donvi = '57,75,79,99'

--#DECLARE#--
select	iLoai
		, sLoai
		, Id_PhongBan
		, TenPB
		, id_User 
		, sID_User 
		, Id_DonVi
		, TenDonVi
		, iLan
		, sLan
from 
		(select	iLoai
				, sLoai = CASE WHEN iLoai = 1 THEN N'1 - Nhập dự toán kiểm tra - Đơn vị'
							   WHEN iLoai = 2 THEN N'2 - Nhập dự toán kiểm tra (tăng/giảm nhiệm vụ)- Đơn vị'
							   WHEN iLoai = 3 THEN N'3 - Nhập dự toán kiểm tra (giảm bệnh viện tự chủ)- Đơn vị'
							   ELSE N' 4 - Nhập dự toán kiểm tra - Ngành'
							   END
				, Id_PhongBan
				, id_User = UserCreator
				, sID_User = UserCreator
				, Id_DonVi
				, TenDonVi	
				, iLan
				, sLan = N'Lần thứ - ' + LTRIM(str(iLan))
		from	SKT_ChungTu
		where	NamLamViec=@nam
				and Locked <> @isreadonly
				and (@iloai is null or iLoai in (select * from f_split(@iloai)))
				and (@id_phongban is null or Id_PhongBan in (select * from f_split(@id_phongban)))
				and (@id_user is null or UserCreator in (select * from f_split(@id_user)))
				and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))) as r1

		inner join 

		(select	Id = sKyHieu
				, TenPB = sTen + ' - ' + sMoTa 
		from	NS_PhongBan where iTrangThai = 1) as pb
		on r1.Id_PhongBan = pb.Id