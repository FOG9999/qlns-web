declare @nam int									set @nam = 2018
declare @id_lock int								set @id_lock = 1
declare @iloais nvarchar(MAX)						set @iloais = '1'
declare @ireqs nvarchar(MAX)						set @ireqs = '0'
declare @phongbans nvarchar(Max)					set @phongbans = '10'
declare @users nvarchar(MAX)						set @users = null
declare @donvis nvarchar(MAX)						set @donvis = null
declare @ilans nvarchar(MAX)						set @ilans = null


--#DECLARE#--

update	SKT_ChungTu
set		Locked = @id_lock
where	NamLamViec=@nam
		and Locked <> @id_lock
		and (@iloais is null or iLoai in (select * from f_split(@iloais)))
		and (@phongbans is null or Id_PhongBan in (select * from f_split(@phongbans)))
		and (@users is null or UserCreator in (select * from f_split(@users)))
		and (@donvis is null or Id_DonVi in (select * from f_split(@donvis)))
		and (@ilans is null or iLan in (select * from f_split(@ilans)))