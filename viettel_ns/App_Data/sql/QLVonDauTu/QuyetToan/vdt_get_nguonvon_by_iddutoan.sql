  declare @DuAnId nvarchar(300)
  set @DuAnId = (select iID_DuAnID FROM VDT_DA_DuToan where iID_DuToanID = cast(@duToanId AS nvarchar(300)))

  select SUM(cast(ISNULL(fCapPhatTaiKhoBac,0) as float)) AS fCapPhatTaiKhoBac,
  SUM(cast(isnull(fCapPhatBangLenhChi,0) as float)) AS fCapPhatBangLenhChi into #tmp from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet where iID_DuAnID = @DuAnId

  select 
  nv.iID_NguonVonID as IIdNguonVonId,
  nv.fTienPheDuyetQDDT as GiaTriPheDuyet,
  ns.sTen as TenNguonVon,
  cast(0 as float) as TienDeNghi,
  nguonvonchitiet.fCapPhatTaiKhoBac,
  nguonvonchitiet.fCapPhatBangLenhChi,
    SUM(cast(isnull(pdttct.fGiaTriThanhToanTN, 0) as float)) AS fGiaTriThanhToanTN,
  SUM(cast(isnull(pdttct.fGiaTriThanhToanNN, 0) as float)) AS fGiaTriThanhToanNN
  from #tmp nguonvonchitiet, VDT_DA_DuToan_Nguonvon nv
  inner join NS_NguonNganSach ns ON ns.iID_MaNguonNganSach = nv.iID_NguonVonID
  left join VDT_TT_DeNghiThanhToan dntt on dntt.iID_DuAnId = @DuAnId
  left join VDT_TT_PheDuyetThanhToan_ChiTiet pdttct on pdttct.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID 
  where iID_DuToanID in (select * FROM f_split(@duToanId))
  group by nv.iID_NguonVonID,ns.sTen,nv.fTienPheDuyetQDDT, nguonvonchitiet.fCapPhatTaiKhoBac, nguonvonchitiet.fCapPhatBangLenhChi

   drop table #tmp