if(@type = 1)
begin
	select 
		distinct
		ctct.*,
		case
			when dahm.iID_LoaiCongTrinhID  is not null then dahm.iID_LoaiCongTrinhID else da.iID_LoaiCongTrinhID
		end iID_LoaiCongTrinhID,
		da.sTenDuAn
		into #tmpData
	from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet ctct 
	inner join VDT_DA_DuAn da on da.iID_DuAnID = ctct.iID_DuAnID
	left join VDT_DA_DuAn_HangMuc dahm on da.iID_DuAnID = dahm.iID_DuAnID
	where ctct.iID_KeHoachVonNam_DuocDuyetID in (select * from dbo.f_split(@lstId))
	and (((da.iID_LoaiCongTrinhID is not null) or (dahm.iID_LoaiCongTrinhID is not null)) and ctct.iID_KeHoachVonNam_DuocDuyetID in (select * from dbo.f_split(@lstId)))
	--and ctct.iLoaiDuAn = 1


	select tbl_sum.* into #tmp_tbl_kmc from (

		select
				'' as STT,
				lct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
				lct.iID_Parent as IdLoaiCongTrinhParent,
				lct.sTenLoaiCongTrinh as sTenDuAn,
				'' as sLNS,
				'' as sL,
				'' as sK,
				'' as sM,
				'' as sTM,
				'' as sTTM,
				'' as sNG,
				cast(0 as float) as FCapPhatTaiKhoBac,
				cast(0 as float) as FCapPhatBangLenhChi,
				cast(0 as float) as FGiaTriThuHoiNamTruocKhoBac,
				cast(0 as float) as FGiaTriThuHoiNamTruocLenhChi,
				cast(0 as float) as TongSo,
				null as IIdDuAn,

				3 as Loai,
				cast(1 as bit) as IsHangCha,
				case
					when lct.iID_Parent is null then 0 else 1
				end LoaiParent,
				1 as LoaiCongTrinh
			from f_loai_cong_trinh_get_list_childrent(@lct) lct

			union all

			select 
				'' as STT,
				ctct.iID_LoaiCongTrinhID as IdLoaiCongTrinh,
				null as IdLoaiCongTrinhParent,
				ctct.sTenDuAn as sTenDuAn,
				ctct.LNS as sLNS,
				ctct.L as sL,
				ctct.K as sK,
				ctct.M as sM,
				ctct.TM as sTM,
				ctct.TTM as sTTM,
				ctct.NG as sNG,
				ctct.FCapPhatTaiKhoBac/@MenhGiaTienTe as FCapPhatTaiKhoBac,
				ctct.FCapPhatBangLenhChi/@MenhGiaTienTe as FCapPhatBangLenhChi,
				ctct.FGiaTriThuHoiNamTruocKhoBac/@MenhGiaTienTe as FGiaTriThuHoiNamTruocKhoBac,
				ctct.FGiaTriThuHoiNamTruocLenhChi/@MenhGiaTienTe as FGiaTriThuHoiNamTruocLenhChi,
				(ctct.FCapPhatTaiKhoBac + ctct.FCapPhatBangLenhChi + ctct.FGiaTriThuHoiNamTruocKhoBac + ctct.FGiaTriThuHoiNamTruocLenhChi)/@MenhGiaTienTe as TongSo,
				ctct.iID_DuAnID as IIdDuAn,
				4 as Loai,
				cast(0 as bit) as IsHangCha,
				2 as LoaiParent,
				1 as LoaiCongTrinh
			from 
				#tmpData ctct
	) as tbl_sum

	insert into #tmp_tbl_kmc(
				STT,
				IdLoaiCongTrinh,
				IdLoaiCongTrinhParent,
				sTenDuAn,
				sLNS,
				sL,
				sK,
				sM,
				sTM,
				sTTM,
				sNG,
				FCapPhatTaiKhoBac,
				FCapPhatBangLenhChi,
				FGiaTriThuHoiNamTruocKhoBac,
				FGiaTriThuHoiNamTruocLenhChi,
				TongSo,
				IIdDuAn,
				Loai,
				IsHangCha,
				LoaiParent,
				LoaiCongTrinh
			)
			values(
				'A', null, null, N'CÔNG TRÌNH MỞ MỚI', '', '', '', '', '', '', '', 0, 0, 0, 0, 0, null, 1, 1, 0, 1
			)

			select * from #tmp_tbl_kmc order by IdLoaiCongTrinh, Loai

	drop table #tmp_tbl_kmc;
	drop table #tmpData;
end
else 
begin
	select 
	distinct
	ctct.*,
	case
		when dahm.iID_LoaiCongTrinhID  is not null then dahm.iID_LoaiCongTrinhID else da.iID_LoaiCongTrinhID
	end iID_LoaiCongTrinhID,
	da.sTenDuAn
	into #tmpDataCt
from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet ctct 
inner join VDT_DA_DuAn da on da.iID_DuAnID = ctct.iID_DuAnID
left join VDT_DA_DuAn_HangMuc dahm on da.iID_DuAnID = dahm.iID_DuAnID
where ctct.iID_KeHoachVonNam_DuocDuyetID in (select * from dbo.f_split(@lstId))
and (((da.iID_LoaiCongTrinhID is not null) or (dahm.iID_LoaiCongTrinhID is not null)) and ctct.iID_KeHoachVonNam_DuocDuyetID in (select * from dbo.f_split(@lstId)))
--and ctct.iLoaiDuAn = 2

select tbl_sum.* into #tmp_tbl_kct from (

			select
					'' as STT,
					lct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
					lct.iID_Parent as IdLoaiCongTrinhParent,
					lct.sTenLoaiCongTrinh as sTenDuAn,
					'' as sLNS,
					'' as sL,
					'' as sK,
					'' as sM,
					'' as sTM,
					'' as sTTM,
					'' as sNG,
					cast(0 as float) as FCapPhatTaiKhoBac,
					cast(0 as float) as FCapPhatBangLenhChi,
					cast(0 as float) as FGiaTriThuHoiNamTruocKhoBac,
					cast(0 as float) as FGiaTriThuHoiNamTruocLenhChi,
					cast(0 as float) as TongSo,
					null as IIdDuAn,

					3 as Loai,
					cast(1 as bit) as IsHangCha,
					case
						when lct.iID_Parent is null then 0 else 1
					end LoaiParent,
					2 as LoaiCongTrinh
				from f_loai_cong_trinh_get_list_childrent(@lct) lct

				union all

				select 
					'' as STT,
					ctct.iID_LoaiCongTrinhID as IdLoaiCongTrinh,
					null as IdLoaiCongTrinhParent,
					ctct.sTenDuAn as sTenDuAn,
					ctct.LNS as sLNS,
					ctct.L as sL,
					ctct.K as sK,
					ctct.M as sM,
					ctct.TM as sTM,
					ctct.TTM as sTTM,
					ctct.NG as sNG,
					ctct.FCapPhatTaiKhoBac/@MenhGiaTienTe as FCapPhatTaiKhoBac,
					ctct.FCapPhatBangLenhChi/@MenhGiaTienTe as FCapPhatBangLenhChi,
					ctct.FGiaTriThuHoiNamTruocKhoBac/@MenhGiaTienTe as FGiaTriThuHoiNamTruocKhoBac,
					ctct.FGiaTriThuHoiNamTruocLenhChi/@MenhGiaTienTe as FGiaTriThuHoiNamTruocLenhChi,
					(ctct.FCapPhatTaiKhoBac + ctct.FCapPhatBangLenhChi + ctct.FGiaTriThuHoiNamTruocKhoBac + ctct.FGiaTriThuHoiNamTruocLenhChi)/@MenhGiaTienTe as TongSo,
					ctct.iID_DuAnID as IIdDuAn,
					4 as Loai,
					cast(0 as bit) as IsHangCha,
					2 as LoaiParent,
					2 as LoaiCongTrinh
				from 
					#tmpDataCt ctct
		) as tbl_sum


	insert into #tmp_tbl_kct(
			STT,
			IdLoaiCongTrinh,
			IdLoaiCongTrinhParent,
			sTenDuAn,
			sLNS,
			sL,
			sK,
			sM,
			sTM,
			sTTM,
			sNG,
			FCapPhatTaiKhoBac,
			FCapPhatBangLenhChi,
			FGiaTriThuHoiNamTruocKhoBac,
			FGiaTriThuHoiNamTruocLenhChi,
			TongSo,
			IIdDuAn,
			Loai,
			IsHangCha,
			LoaiParent,
			LoaiCongTrinh
		)
		values(
			'B', null, null, N'CÔNG TRÌNH CHUYỂN TIẾP', '', '', '', '', '', '', '', 0, 0, 0, 0, 0, null, 1, 1, 0, 2
		)

		select * from #tmp_tbl_kct order by IdLoaiCongTrinh, Loai

		drop table #tmp_tbl_kct;
		drop table #tmpDataCt;
end