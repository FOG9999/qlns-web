/*Làm chức năng điều chỉnh kế hoạch vốn ứng--
 --* Kế hoạch vốn ứng đề xuất --
 Bảng: VDT_KHV_KeHoachVonUng_DX thêm các cột:
	bActive: bit
	bIsGoc: bit
 Bảng: VDT_KHV_KeHoachVonUng_DX_ChiTiet thêm các cột:
	fGiaTriDeNghiDC float

	* Kế hoạch vốn ứng được duyệt:
Bảng: VDT_KHV_KeHoachVonUng thêm các cột:
	bActive: bit
	bIsGoc: bit,
	iID_ParentID: uniqueidentifier;
Bảng: VDT_KHV_KeHoachVonUng_ChiTiet thêm các cột:
	fCapPhatTaiKhoBacDC float,
	fCapPhatBangLenhChiDC float;

	*/
--script SQL:
ALTER TABLE VDT_KHV_KeHoachVonUng_DX
ADD 
	bActive  bit,
    bIsGoc  bit,
	iID_ParentID uniqueidentifier;
	
ALTER TABLE VDT_KHV_KeHoachVonUng_DX_ChiTiet
ADD fGiaTriDeNghiDC float


alter table VDT_KHV_KeHoachVonUng
drop column IID_ParentID ;

ALTER TABLE VDT_KHV_KeHoachVonUng
ADD 
	bActive  bit,
    bIsGoc  bit,
	iID_ParentID uniqueidentifier;
	
ALTER TABLE VDT_KHV_KeHoachVonUng_ChiTiet
ADD 
	fCapPhatTaiKhoBacDC float,
	fCapPhatBangLenhChiDC float;