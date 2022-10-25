declare @nam int									set @nam = 2020
declare @id_lock int								set @id_lock = 0
declare @phongbans nvarchar(Max)					set @phongbans = '07'
declare @donvis nvarchar(MAX)						set @donvis = null


--#DECLARE#--

update	SKT_ChungTu
set		B_Locked = @id_lock
where	NamLamViec=@nam
		and B_Locked <> @id_lock
		and Id_PhongBan = @phongban
		and (@iloais is null or iLoai in (select * from f_split(@iloais)))
		and (@donvis is null or Id_DonVi in (select * from f_split(@donvis)))
