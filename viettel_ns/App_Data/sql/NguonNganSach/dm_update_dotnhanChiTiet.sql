--CREATE TYPE DotNhanChiTietTableType AS TABLE  
--    ( iID_DotNhanChiTiet guid, SoTien decimal,ghiChu nvarchar(max) )  

--Declare @DotNhanChiTietTableType DotNhanChiTietTableType 

--BEGIN
--		UPDATE NNS_DotNhanChiTiet
--		SET
--			SoTien = tb.SoTien,
--			GhiChu = tb.GhiChu
			
--		FROM 
--			(SELECT * FROM @DotNhanChiTietTableType) as tb
--		WHERE iID_DotNhanChiTiet = tb.iID_DotNhanChiTiet
--END


BEGIN
		UPDATE NNS_DotNhanChiTiet
		SET
			SoTien = @SoTien,
			GhiChu = @GhiChu,
			iSoLanSua = ISNULL(iSoLanSua,0) + 1,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_DotNhanChiTiet = @iID_DotNhanChiTiet
	END

