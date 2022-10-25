
declare @idonvi uniqueidentifier       set @idonvi = '00000000-0000-0000-0000-000000000000'
declare @inhiemvuchi uniqueidentifier       set @inhiemvuchi = '00000000-0000-0000-0000-000000000000'
declare @ichudautu	 uniqueidentifier      set @ichudautu = '00000000-0000-0000-0000-000000000000'
declare @ihopdong uniqueidentifier        set @ihopdong = '00000000-0000-0000-0000-000000000000'
declare @iduan uniqueidentifier          set @iduan = '00000000-0000-0000-0000-000000000000'
declare @dngaytao datetime          set @dngaytao = '00000000-0000-0000-0000-000000000000'
--#DECLARE#--

/* */

	Select  * from NH_TT_ThanhToan as tt  
	where ((tt.iLoaiDeNghi = 1 and iCoQuanThanhToan = 2) or (tt.iLoaiDeNghi = 2 and iCoQuanThanhToan = 1) or (tt.iLoaiDeNghi  = 2 and iCoQuanThanhToan = 1))
	and tt.iID_DonVi = @idonvi
	and tt.iID_KHCTBQP_NhiemVuChiID = @inhiemvuchi
	and tt.iID_ChuDauTuID  = @ichudautu
	and tt.dNgayTao < @dngaytao  order by dNgayTao desc


	

