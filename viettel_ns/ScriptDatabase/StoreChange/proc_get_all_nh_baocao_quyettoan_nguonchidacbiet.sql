ALTER PROCEDURE [dbo].[proc_get_all_nh_baocao_quyettoan_nguonchidacbiet]
	@iNamKeHoach int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	CREATE TABLE #Temp
	(
		ID uniqueidentifier,
		sTen nvarchar(MAX),
		fQTKinhPhiDuocCap_TongSo_USD float,
		fQTKinhPhiDuocCap_TongSo_VND float,
		fQTKinhPhiDuocCap_NamNay_USD float,
		fQTKinhPhiDuocCap_NamNay_VND float,
		fQTKinhPhiDuocCap_NamTruocChuyenSang_USD float,
		fQTKinhPhiDuocCap_NamTruocChuyenSang_VND float,
		fDeNghiQTNamNay_USD float,
		fDeNghiQTNamNay_VND float,
		fDeNghiChuyenNamSau_USD float,
		fDeNghiChuyenNamSau_VND float,
		fThuaThieuKinhPhiTrongNam_USD float,
		fThuaThieuKinhPhiTrongNam_VND float,
		IDParent uniqueidentifier,
		IsBold int,
		IsData int
	)
	
	insert into #Temp (ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData)
	SELECT qtct.ID, qtnd.sSoDeNghi AS sTen,
		qtct.fQTKinhPhiDuocCap_TongSo_USD,
		qtct.fQTKinhPhiDuocCap_TongSo_VND,
		qtct.fQTKinhPhiDuocCap_NamNay_USD,
		qtct.fQTKinhPhiDuocCap_NamNay_VND,
		qtct.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,
		qtct.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,
		qtct.fDeNghiQTNamNay_USD,
		qtct.fDeNghiQTNamNay_VND,
		qtct.fDeNghiChuyenNamSau_USD,
		qtct.fDeNghiChuyenNamSau_VND,
		qtct.fThuaThieuKinhPhiTrongNam_USD,
		qtct.fThuaThieuKinhPhiTrongNam_VND,
		qtct.iID_KHCTBQP_NhiemVuChiID AS IDParent,
		0 AS IsBold,
		0 AS IsData
	FROM NH_QT_QuyetToanNienDo qtnd
	INNER JOIN NH_QT_QuyetToanNienDo_ChiTiet qtct ON qtct.iID_QuyetToanNienDoID = qtnd.ID
	WHERE qtnd.iNamKeHoach = @iNamKeHoach AND qtct.bIsSaveTongHop = 1
		
	insert into #Temp (ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData)
	select ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData
	from(
		SELECT
		nvc.ID,
		nvc.sTenNhiemVuChi AS sTen,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_USD, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD, 0))) AS fQTKinhPhiDuocCap_TongSo_USD,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_VND, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND, 0))) AS fQTKinhPhiDuocCap_TongSo_VND,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_USD, 0)) AS fQTKinhPhiDuocCap_NamNay_USD,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_VND, 0)) AS fQTKinhPhiDuocCap_NamNay_VND,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD, 0)) AS fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND, 0)) AS fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,
		SUM(ISNULL(temp.fDeNghiQTNamNay_USD, 0)) AS fDeNghiQTNamNay_USD,
		SUM(ISNULL(temp.fDeNghiQTNamNay_VND, 0)) AS fDeNghiQTNamNay_VND,
		SUM(ISNULL(temp.fDeNghiChuyenNamSau_USD, 0)) AS fDeNghiChuyenNamSau_USD,
		SUM(ISNULL(temp.fDeNghiChuyenNamSau_VND, 0)) AS fDeNghiChuyenNamSau_VND,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_USD, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD, 0))
			- SUM(ISNULL(temp.fDeNghiQTNamNay_USD, 0)) - SUM(ISNULL(temp.fDeNghiChuyenNamSau_USD, 0))) AS fThuaThieuKinhPhiTrongNam_USD,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_VND, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND, 0))
			- SUM(ISNULL(temp.fDeNghiQTNamNay_VND, 0)) - SUM(ISNULL(temp.fDeNghiChuyenNamSau_VND, 0))) AS fThuaThieuKinhPhiTrongNam_VND,
		nvc.iID_DonViID AS IDParent,
		0 AS IsBold,
		1 AS IsData
		FROM NH_KHChiTietBQP_NhiemVuChi AS nvc
		INNER JOIN #Temp AS temp ON nvc.ID = temp.IDParent
		GROUP BY nvc.ID,nvc.sTenNhiemVuChi,nvc.iID_DonViID
	) AS A
		
	insert into #Temp (ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData)
	select ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData
	from(
		SELECT
		dv.iID_Ma AS ID,
		CONCAT(dv.iID_MaDonVi, ' - ', ISNULL(dv.sTen, '')) AS sTen,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_USD, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD, 0))) AS fQTKinhPhiDuocCap_TongSo_USD,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_VND, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND, 0))) AS fQTKinhPhiDuocCap_TongSo_VND,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_USD, 0)) AS fQTKinhPhiDuocCap_NamNay_USD,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_VND, 0)) AS fQTKinhPhiDuocCap_NamNay_VND,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD, 0)) AS fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,
		SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND, 0)) AS fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,
		SUM(ISNULL(temp.fDeNghiQTNamNay_USD, 0)) AS fDeNghiQTNamNay_USD,
		SUM(ISNULL(temp.fDeNghiQTNamNay_VND, 0)) AS fDeNghiQTNamNay_VND,
		SUM(ISNULL(temp.fDeNghiChuyenNamSau_USD, 0)) AS fDeNghiChuyenNamSau_USD,
		SUM(ISNULL(temp.fDeNghiChuyenNamSau_VND, 0)) AS fDeNghiChuyenNamSau_VND,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_USD, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD, 0))
			- SUM(ISNULL(temp.fDeNghiQTNamNay_USD, 0)) - SUM(ISNULL(temp.fDeNghiChuyenNamSau_USD, 0))) AS fThuaThieuKinhPhiTrongNam_USD,
		(SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamNay_VND, 0)) + SUM(ISNULL(temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND, 0))
			- SUM(ISNULL(temp.fDeNghiQTNamNay_VND, 0)) - SUM(ISNULL(temp.fDeNghiChuyenNamSau_VND, 0))) AS fThuaThieuKinhPhiTrongNam_VND,
		NULL AS IDParent,
		1 AS IsBold,
		2 AS IsData
		FROM NS_DonVi AS dv
		INNER JOIN #Temp as temp on dv.iID_Ma = temp.IDParent
		WHERE dv.iTrangThai = 1 AND dv.iNamLamViec_DonVi = @iNamKeHoach
		GROUP BY dv.iID_Ma,CONCAT(dv.iID_MaDonVi, ' - ', ISNULL(dv.sTen, ''))
	) AS B
		
	;WITH #Tree(ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData,position)
	AS (
		select temp.ID,temp.sTen,
			temp.fQTKinhPhiDuocCap_TongSo_USD,temp.fQTKinhPhiDuocCap_TongSo_VND,
			temp.fQTKinhPhiDuocCap_NamNay_USD,temp.fQTKinhPhiDuocCap_NamNay_VND,
			temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,temp.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,
			temp.fDeNghiQTNamNay_USD,temp.fDeNghiQTNamNay_VND,
			temp.fDeNghiChuyenNamSau_USD,temp.fDeNghiChuyenNamSau_VND,
			temp.fThuaThieuKinhPhiTrongNam_USD,temp.fThuaThieuKinhPhiTrongNam_VND,
			temp.IDParent,temp.IsBold,temp.IsData,
			CAST(ROW_NUMBER() OVER(ORDER BY temp.sTen) AS NVARCHAR(MAX)) AS position
		from #Temp as temp
		where temp.IDParent is null
		UNION ALL
		select
			child.ID,child.sTen,
			child.fQTKinhPhiDuocCap_TongSo_USD,child.fQTKinhPhiDuocCap_TongSo_VND,
			child.fQTKinhPhiDuocCap_NamNay_USD,child.fQTKinhPhiDuocCap_NamNay_VND,
			child.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,child.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,
			child.fDeNghiQTNamNay_USD,child.fDeNghiQTNamNay_VND,
			child.fDeNghiChuyenNamSau_USD,child.fDeNghiChuyenNamSau_VND,
			child.fThuaThieuKinhPhiTrongNam_USD,child.fThuaThieuKinhPhiTrongNam_VND,
			child.IDParent,child.IsBold,child.IsData,
			CONCAT(parent.position,'.',CAST(ROW_NUMBER() OVER(ORDER BY child.sTen) AS NVARCHAR(MAX))) AS position
		from #Temp as child 
		inner join #Tree as parent on parent.ID = child.IDParent
	)
	SELECT * INTO #Data FROM #Tree;
	
	insert into #Data (ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData,position)
	select ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData,position
	from (
		SELECT
		NULL AS ID,
		N'Tổng cộng: ' AS sTen,
		SUM(ISNULL(data.fQTKinhPhiDuocCap_TongSo_USD, 0)) AS fQTKinhPhiDuocCap_TongSo_USD,
		SUM(ISNULL(data.fQTKinhPhiDuocCap_TongSo_VND, 0)) AS fQTKinhPhiDuocCap_TongSo_VND,
		SUM(ISNULL(data.fQTKinhPhiDuocCap_NamNay_USD, 0)) AS fQTKinhPhiDuocCap_NamNay_USD,
		SUM(ISNULL(data.fQTKinhPhiDuocCap_NamNay_VND, 0)) AS fQTKinhPhiDuocCap_NamNay_VND,
		SUM(ISNULL(data.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD, 0)) AS fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,
		SUM(ISNULL(data.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND, 0)) AS fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,
		SUM(ISNULL(data.fDeNghiQTNamNay_USD, 0)) AS fDeNghiQTNamNay_USD,
		SUM(ISNULL(data.fDeNghiQTNamNay_VND, 0)) AS fDeNghiQTNamNay_VND,
		SUM(ISNULL(data.fDeNghiChuyenNamSau_USD, 0)) AS fDeNghiChuyenNamSau_USD,
		SUM(ISNULL(data.fDeNghiChuyenNamSau_VND, 0)) AS fDeNghiChuyenNamSau_VND,
		SUM(ISNULL(data.fThuaThieuKinhPhiTrongNam_USD, 0)) AS fThuaThieuKinhPhiTrongNam_USD,
		SUM(ISNULL(data.fThuaThieuKinhPhiTrongNam_VND, 0)) AS fThuaThieuKinhPhiTrongNam_VND,
		NULL AS IDParent,
		1 AS IsBold,
		3 AS IsData,
		'0' AS position
		FROM #Data AS data
		WHERE data.IsData = 2
	) AS C

	SELECT ID,position,sTen,cast('/' + replace(position, '.', '/') + '/' as hierarchyid) AS sort,
		fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,
		fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,
		fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,
		fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,
		fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,
		fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,
		IDParent,IsBold,IsData
	INTO #GetData
	FROM #Data dt
	WHERE dt.IsData <> 0 AND dt.IsData <> 3

	insert into #GetData (ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData,position,sort)
	select ID,sTen,fQTKinhPhiDuocCap_TongSo_USD,fQTKinhPhiDuocCap_TongSo_VND,fQTKinhPhiDuocCap_NamNay_USD,fQTKinhPhiDuocCap_NamNay_VND,fQTKinhPhiDuocCap_NamTruocChuyenSang_USD,fQTKinhPhiDuocCap_NamTruocChuyenSang_VND,fDeNghiQTNamNay_USD,fDeNghiQTNamNay_VND,fDeNghiChuyenNamSau_USD,fDeNghiChuyenNamSau_VND,fThuaThieuKinhPhiTrongNam_USD,fThuaThieuKinhPhiTrongNam_VND,IDParent,IsBold,IsData,position,NULL AS sort
	from #Data dt
	WHERE dt.IsData = 3
	
	SELECT * FROM #GetData ORDER BY sort;

	DROP TABLE #Data;
	DROP TABLE #Temp;
	DROP TABLE #GetData;
END