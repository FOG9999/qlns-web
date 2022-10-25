USE [viettel_ns]

--GO
--DROP FUNCTION [dbo].[F_NS_LNS_PhongBan]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[F_NS_LNS_PhongBan]
(    
      @iNamLamViec int,
	  @iID_MaPhongBan nvarchar(2)
)

RETURNS @Output TABLE  
(
	sLNS	nvarchar(7),
	sMoTa	nvarchar(200),
	TenHT	nvarchar(200)
)
AS

BEGIN

declare @id_phongban uniqueidentifier;

select top(1) @id_phongban=iID_MaPhongBan from NS_PhongBan
where sKyHieu=@iID_MaPhongBan;
 
with r as
(

SELECT sLNS,sMoTa,TenHT
FROM   (

	SELECT	sLNS,
			sMoTa,
			sLNS +' - '+ sMoTa AS TenHT ,
			Row_number() OVER(PARTITION BY sLNS ORDER BY sLNS) rn
        
	FROM    NS_MucLucNganSach as A 
	INNER JOIN (select sLNS as id, iID_MaPhongBan from NS_PhongBan_LoaiNganSach where iTrangThai=1) AS B ON A.sLNS = B.id 
	WHERE   B.iID_MaPhongBan=@id_phongban 
			AND LEN(sLNS)=7  
			AND sL = '' 
			AND iNamLamViec=@iNamLamViec 
			AND iTrangThai=1
		) t
WHERE  rn = 1

)

INSERT @Output
select * from r

RETURN
END


/*

select * from F_NS_LNS_PhongBan(2017,'07')


*/
