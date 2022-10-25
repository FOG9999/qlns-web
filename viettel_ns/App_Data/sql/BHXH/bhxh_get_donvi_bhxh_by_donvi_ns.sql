declare @iID_NS_DonVi nvarchar(50) set @iID_NS_DonVi = '51'

--#DECLARE#--
;WITH orderedTree AS (
        SELECT bhxhdv.*, bhxhdv.iID_BHXH_DonViID AS parent, 1 AS depth, CAST(bhxhdv.iID_MaDonViBHXH AS NVARCHAR(MAX)) AS location
        FROM BHXH_DonVi bhxhdv
        WHERE bhxhdv.iID_ParentID IS NULL AND iID_NS_MaDonVi like @iID_NS_DonVi
         
        UNION ALL
         
        SELECT
                child.*, orderedTree.parent, depth + 1, CAST(CONCAT(orderedTree.location, '.', child.iID_MaDonViBHXH) AS NVARCHAR(MAX)) AS location
        FROM BHXH_DonVi child
        INNER JOIN orderedTree ON child.iID_ParentID = orderedTree.iID_BHXH_DonViID
)
SELECT * FROM orderedTree order by location