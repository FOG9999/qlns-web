declare @nam int									set @nam = 2020
declare @id_chungtu uniqueidentifier				set @id_chungtu = '58652eef-1bcd-4f16-903a-b78abd13cf65'



--#DECLARE#--

update	SKT_ChungTu
set		B_Locked = Case B_Locked WHEN 0 THEN 1 ELSE 0 END
where	Id = @id_chungtu