DECLARE @iID_DeNghiThanhToanID uniqueidentifier SET @iID_DeNghiThanhToanID = '77e092d2-68f6-47b6-b45b-ade501022cfb'
--#DECLARE#--
SELECT pdct.* ,
(select top 1 sSoQuyetDinh FROM VDT_TongHop_NguonNSDauTu WHERE iID_ChungTu = pdct.iID_KeHoachVonID) as sTenKHV,
	(select top 1 sSoQuyetDinh FROM VDT_TongHop_NguonNSDauTu WHERE iID_ChungTu = pdct.iId_DeNghiTamUng) as sTenDeNghiTU
FROM VDT_TT_PheDuyetThanhToan_ChiTiet pdct
WHERE pdct.iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID;