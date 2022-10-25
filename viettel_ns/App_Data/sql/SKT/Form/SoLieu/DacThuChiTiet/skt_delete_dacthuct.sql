declare @id_chungtu uniqueidentifier		set @id_chungtu = '4F60C12E-D3FE-4077-9D35-324ABE6A257C'

--#DECLARE#--
delete	SKT_DacThuChiTiet
where	Id_ChungTu = @id_chungtu		