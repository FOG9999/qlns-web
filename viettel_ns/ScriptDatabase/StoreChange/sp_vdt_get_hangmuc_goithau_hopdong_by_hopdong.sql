USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_get_hangmuc_goithau_hopdong_by_hopdong]    Script Date: 28/12/2022 9:01:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_vdt_get_hangmuc_goithau_hopdong_by_hopdong]
@iIdHopDong uniqueidentifier
AS
BEGIN
	
	select 
		gthm.Id as Id,
		gtnt.iID_GoiThauID  as IIDGoiThauID,
		gthm.iID_ChiPhiID as IIDChiPhiID,
		gtnt.Id as IdGoiThauNhaThau,
		gtnt.iID_HopDongID as IIDHopDongID,
		gthm.iID_HangMucID as IIDHangMucID,
		null as IIDNhaThauID,
		cast(1 as bit) as IsChecked,
		dacp.sTenChiPhi as STenChiPhi,
		gthm.fGiaTri as FTienGoiThau,
		dacp.iThuTu as IThuTu,	
		dacp.iID_ChiPhi_Parent as IdChiPhiDuAnParent,
		dthm.iID_ParentID as HangMucParentId,
		dthm.smaOrder AS MaOrDer,
		dthm.sTenHangMuc as STenHangMuc,
		gthm.fGiaTri as FGiaTriDuocDuyet,
		null as FGiaTriConLai,
		gthm.fGiaTri as FTienGoiThauTruocDC,
		isnull(cast(case when parentId.iID_ParentID is not null or dthm.iID_ParentID is null then 1 else 0 end as bit),0)  as IsHangCha
	from VDT_DA_HopDong_GoiThau_HangMuc gthm

	inner join VDT_DA_QDDauTu_DM_HangMuc dthm ON dthm.iID_QDDauTu_DM_HangMucID = gthm.iID_HangMucID
	inner join VDT_DA_HopDong_GoiThau_NhaThau gtnt ON gtnt.Id = gthm.iID_HopDongGoiThauNhaThauID
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = gthm.iID_ChiPhiID

	left join
			(
				select distinct tb2.iID_ParentID from VDT_DA_HopDong_GoiThau_HangMuc tb1
				inner join VDT_DA_QDDauTu_DM_HangMuc tb2 ON tb1.iID_HangMucID = tb2.iID_QDDauTu_DM_HangMucID  and tb2.iID_ParentID is not null
				inner join VDT_DA_HopDong_GoiThau_NhaThau tb3 ON tb3.Id = tb1.iID_HopDongGoiThauNhaThauID AND tb3.iID_HopDongID = @iIdHopDong
			
			) as parentId ON parentId.iID_ParentID = gthm.iID_HangMucID

	--where gtnt.iID_HopDongID = @iIdHopDong
	--order by MaOrDer

	-- add them VDT_DA_DuToan_DM_HangMuc, VDT_DA_ChuTruongDauTu_DM_HangMuc
	union all 
		
	select 
		gthm.Id as Id,
		gtnt.iID_GoiThauID  as IIDGoiThauID,
		gthm.iID_ChiPhiID as IIDChiPhiID,
		gtnt.Id as IdGoiThauNhaThau,
		gtnt.iID_HopDongID as IIDHopDongID,
		gthm.iID_HangMucID as IIDHangMucID,
		null as IIDNhaThauID,
		cast(1 as bit) as IsChecked,
		dacp.sTenChiPhi as STenChiPhi,
		gthm.fGiaTri as FTienGoiThau,
		dacp.iThuTu as IThuTu,	
		dacp.iID_ChiPhi_Parent as IdChiPhiDuAnParent,
		dthm.iID_ParentID as HangMucParentId,
		dthm.maOrder AS MaOrDer,
		dthm.sTenHangMuc as STenHangMuc,
		gthm.fGiaTri as FGiaTriDuocDuyet,
		null as FGiaTriConLai,
		gthm.fGiaTri as FTienGoiThauTruocDC,
		isnull(cast(case when parentId.iID_ParentID is not null or dthm.iID_ParentID is null then 1 else 0 end as bit),0)  as IsHangCha
	from VDT_DA_HopDong_GoiThau_HangMuc gthm

	inner join VDT_DA_DuToan_DM_HangMuc dthm ON dthm.Id = gthm.iID_HangMucID
	inner join VDT_DA_HopDong_GoiThau_NhaThau gtnt ON gtnt.Id = gthm.iID_HopDongGoiThauNhaThauID
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = gthm.iID_ChiPhiID

	left join
			(
				select distinct tb2.iID_ParentID from VDT_DA_HopDong_GoiThau_HangMuc tb1
				inner join VDT_DA_DuToan_DM_HangMuc tb2 ON tb1.iID_HangMucID = tb2.Id  and tb2.iID_ParentID is not null
				inner join VDT_DA_HopDong_GoiThau_NhaThau tb3 ON tb3.Id = tb1.iID_HopDongGoiThauNhaThauID AND tb3.iID_HopDongID = @iIdHopDong
			
			) as parentId ON parentId.iID_ParentID = gthm.iID_HangMucID

    union all 
		
	select 
		gthm.Id as Id,
		gtnt.iID_GoiThauID  as IIDGoiThauID,
		gthm.iID_ChiPhiID as IIDChiPhiID,
		gtnt.Id as IdGoiThauNhaThau,
		gtnt.iID_HopDongID as IIDHopDongID,
		gthm.iID_HangMucID as IIDHangMucID,
		null as IIDNhaThauID,
		cast(1 as bit) as IsChecked,
		dacp.sTenChiPhi as STenChiPhi,
		gthm.fGiaTri as FTienGoiThau,
		dacp.iThuTu as IThuTu,	
		dacp.iID_ChiPhi_Parent as IdChiPhiDuAnParent,
		dthm.iID_ParentID as HangMucParentId,
		dthm.smaOrder AS MaOrDer,
		dthm.sTenHangMuc as STenHangMuc,
		gthm.fGiaTri as FGiaTriDuocDuyet,
		null as FGiaTriConLai,
		gthm.fGiaTri as FTienGoiThauTruocDC,
		isnull(cast(case when parentId.iID_ParentID is not null or dthm.iID_ParentID is null then 1 else 0 end as bit),0)  as IsHangCha
	from VDT_DA_HopDong_GoiThau_HangMuc gthm

	inner join VDT_DA_ChuTruongDauTu_DM_HangMuc dthm ON dthm.iID_ChuTruongDauTu_DM_HangMucID = gthm.iID_HangMucID
	inner join VDT_DA_HopDong_GoiThau_NhaThau gtnt ON gtnt.Id = gthm.iID_HopDongGoiThauNhaThauID
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = gthm.iID_ChiPhiID

	left join
			(
				select distinct tb2.iID_ParentID from VDT_DA_HopDong_GoiThau_HangMuc tb1
				inner join VDT_DA_ChuTruongDauTu_DM_HangMuc tb2 ON tb1.iID_HangMucID = tb2.iID_ChuTruongDauTu_DM_HangMucID  and tb2.iID_ParentID is not null
				inner join VDT_DA_HopDong_GoiThau_NhaThau tb3 ON tb3.Id = tb1.iID_HopDongGoiThauNhaThauID AND tb3.iID_HopDongID = @iIdHopDong
			
			) as parentId ON parentId.iID_ParentID = gthm.iID_HangMucID
 

	where gtnt.iID_HopDongID = @iIdHopDong
	order by MaOrDer
END