declare @sMaDonViBHXH nvarchar(50) set @sMaDonViBHXH = '1706'
declare @sMaDonViNS nvarchar(50) set @sMaDonViNS = '02'
declare @strDonviBHXH nvarchar(MAX) set @strDonviBHXH = '1,2,3,4,5'

--#DECLARE#--

;WITH orderedTree AS (
        SELECT bhxhdv.*, bhxhdv.iID_BHXH_DonViID AS parent, 1 AS depth, CAST(bhxhdv.iID_MaDonViBHXH AS NVARCHAR(MAX)) AS location,
		CAST(ROW_NUMBER() over (order by iID_MaDonViBHXH) AS nvarchar(max)) as sSTT
        FROM BHXH_DonVi bhxhdv
        WHERE bhxhdv.iID_ParentID IS NULL AND (ISNULL(@sMaDonViNS, '') = '' OR iID_NS_MaDonVi like @sMaDonViNS) --AND bhxhdv.iID_MaDonViBHXH IN (select * FROM splitstring(@strDonviBHXH))
         
        UNION ALL
         
        SELECT child.*, orderedTree.parent, depth + 1, CAST(CONCAT(orderedTree.location, '.', child.iID_MaDonViBHXH) AS NVARCHAR(MAX)) AS location,
		orderedTree.sSTT + '.' + CAST(ROW_NUMBER() over (order by child.iID_MaDonViBHXH) AS nvarchar(max)) as sSTT
        FROM BHXH_DonVi child
        INNER JOIN orderedTree ON child.iID_ParentID = orderedTree.iID_BHXH_DonViID AND (ISNULL(@sMaDonViBHXH, '') = '' OR (child.iID_MaDonViBHXH like @sMaDonViBHXH)) AND child.iID_MaDonViBHXH IN (select * FROM splitstring(@strDonviBHXH))
)

select * from orderedTree 
ORDER BY location, depth