insert into VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet(
		 [Id]
		,[iID_PhanBoVon_DonVi_PheDuyet_ID]
		,[iID_DuAnID]
		,[fGiaTriPhanBo]
		,[fGiaTriThuHoi]
		,[iID_DonViTienTeID]
		,[iID_TienTeID]
		,[fTiGiaDonVi]
		,[fTiGia]
		,[sGhiChu]
		,[iID_LoaiCongTrinh]
		,[iId_Parent]
		,[bActive]
		,[ILoaiDuAn]
	)
	select
		 NEWID()
		,@iID_KePhanBoVonDonViL
		,pbvct.[iID_DuAnID]
		,pbvct.[fGiaTriPhanBo]
		,pbvct.[fGiaTriThuHoi]
		,pbvct.[iID_DonViTienTeID]
		,pbvct.[iID_TienTeID]
		,pbvct.[fTiGiaDonVi]
		,pbvct.[fTiGia]
		,pbvct.[sGhiChu]
		,pbvct.[iID_LoaiCongTrinh]
		,pbvct.[iId_Parent]
		,pbvct.[bActive]
		,pbvct.[ILoaiDuAn]
	from
		VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet pbvct
	where pbvct.iID_PhanBoVon_DonVi_PheDuyet_ID = @iID_KePhanBoVonDonViF