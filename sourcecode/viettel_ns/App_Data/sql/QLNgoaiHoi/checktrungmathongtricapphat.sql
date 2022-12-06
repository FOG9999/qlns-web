DECLARE  @mathongtri nvarchar(200) 		set @mathongtri = ''
DECLARE @type_action int		set @type_action = 0
DECLARE @imathongtri uniqueidentifier set @imathongtri = '00000000-0000-0000-0000-000000000000'
--#DECLARE#--

/* */

DECLARE @number int;
If @type_action = 0
	If (select 1 from NH_TT_ThongTriCapPhat where sSoThongTri = @mathongtri) = 1
		select 1;
	Else
		select 0;
Else
	If (select 1 from NH_TT_ThongTriCapPhat where sSoThongTri = @mathongtri) = 1 and (select ID from NH_TT_ThongTriCapPhat where sSoThongTri = @mathongtri) = @imathongtri
		select 0;
	Else
		If (select 1 from NH_TT_ThongTriCapPhat where sSoThongTri = @mathongtri) = 1
			select 1;
		Else
			select 0;

	

