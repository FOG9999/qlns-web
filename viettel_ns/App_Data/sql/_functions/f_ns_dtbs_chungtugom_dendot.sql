

/*

author:			longsam
date:			22/05/2018
description:	lấy danh sách id các chứng từ đã gom
params:			- @iNamLamViec
				- @iID_MaNamNganSach
				- @iID_MaPhongBan
				- @dNgay

*/

USE [viettel_ns]
GO
/****** Object:  UserDefinedFunction [dbo].[F_NS_DTBS_ChungTuDaGom_DenDot]    Script Date: 27/03/2018 2:51:51 CH ******/
--DROP FUNCTION [dbo].[f_ns_dtbs_chungtugom_dendot]
GO

/****** Object:  UserDefinedFunction [dbo].[F_NS_DTBS_ChungTuDaGom_DenDot]    Script Date: 27/03/2018 2:51:51 CH ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create FUNCTION [dbo].[f_ns_dtbs_chungtugom_dendot]
(    
      @iNamLamViec int,
	  @iID_MaNamNganSach nvarchar(10),
	  @iID_MaPhongBan nvarchar(2),
	  @dNgay datetime
)

RETURNS @Output TABLE  
(
	id NVARCHAR(36) 
)
AS

BEGIN


declare @results varchar(2000);

if (@dNgay IS NULL) 
set @dNgay=GETDATE()


--select @results = coalesce(@results + ',', '') +  iID_MaChungTu
--from DTBS_ChungTu_TLTH
--where	iTrangThai=1
--		and iNamLamViec=@iNamLamViec
--		-- neu nam truoc chuyen sang thi ko can gom
--		and (iID_MaNamNganSach=4 or iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach)))
--		and iID_MaPhongBan=@iID_MaPhongBan
--		and dNgayChungTu<=@dNgay



select @results = coalesce(@results + ',', '') +  iID_MaChungTu from
(


select iID_MaChungTu
from DTBS_ChungTu_TLTH
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and (iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach)))
		and iID_MaPhongBan=@iID_MaPhongBan
		and dNgayChungTu<=@dNgay

-- neu nam truoc chuyen sang thi ko can gom
union
select CONVERT(nvarchar(36),iID_MaChungTu) as iID_MaChungTu
from DTBS_ChungTu
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaNamNganSach=4
		and iID_MaPhongBan=@iID_MaPhongBan
		and dNgayChungTu<=@dNgay
) as t


INSERT @Output (id)
select distinct * from F_Split(@results)

RETURN

END

/*

SELECT * FROM F_NS_DTBS_ChungTuDaGom(2018,'07')

*/

GO
