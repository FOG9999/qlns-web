declare @nam int									set @nam = 2020
declare @isreadonly int								set @isreadonly = 1
declare @id_phongban int							set @id_phongban = 07
declare @iloai nvarchar(10)							set @iloai = '2'


--#DECLARE#--
select	iLoai
		, sLoai = CASE WHEN iLoai = 1 THEN N'1 - Nhập dự toán kiểm tra - Đơn vị'
					   ELSE N' 2 - Nhập dự toán kiểm tra - Ngành'
					   END
		, Id_DonVi
		, TenDonVi					
from	SKT_ChungTu
where	NamLamViec=@nam
		and B_Locked <> @isreadonly
		and Id_PhongBan = @id_phongban
		and (@iloai is null or iLoai in (select * from f_split(@iloai)))
order by Id_DonVi

		