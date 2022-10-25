DECLARE  @sodenghi nvarchar(200) 		set @sodenghi = ''
DECLARE @type_action int		set @type_action = 0
DECLARE @imadenghi uniqueidentifier set @imadenghi = '00000000-0000-0000-0000-000000000000'
--#DECLARE#--

/* */

DECLARE @number int;
If @type_action = 0
	If (select count(ID) from NH_TT_ThanhToan where sSoDeNghi = @sodenghi) >= 1
		select 1;
	Else
		select 0;
Else
	If (select 1 from NH_TT_ThanhToan where sSoDeNghi = @sodenghi and ID = @imadenghi) = 1
		select 0;
	Else
		If (select 1 from NH_TT_ThanhToan where sSoDeNghi = @sodenghi) = 1
			select 1;
		Else
			select 0;

	

