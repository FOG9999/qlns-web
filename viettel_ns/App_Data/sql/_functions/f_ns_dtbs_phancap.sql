USE [viettel_ns]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[f_ns_nammlns]
(    
      @iNamLamViec		int	  
)

RETURNS TABLE
AS
RETURN
SELECT *  
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        --AND (MaLoai='' or MaLoai='2')
		AND iID_MaPhongBanDich=@iID_MaPhongBan
		AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
        AND (
			
			iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
			-- phan cap lan 2: cho cac bql
			OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
				
			-- phan cap lan 2: cho b
			OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))


			OR 
				iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (   
																select iID_MaChungTu from DTBS_ChungTu
																where iID_MaChungTu in (
																					select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																					where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu)))))

			-- phan cap lan 2
			OR iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									where iID_MaChungTu in (
															select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
															where iID_MaChungTu in (   
																					select iID_MaChungTu from DTBS_ChungTu
																					where iID_MaChungTu in (
																											select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																											where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))

				
				
			
			)
 
/*

SELECT * FROM f_ns_dtbs_phancap(2018,'07','29','f053c61e-6041-46ce-a87c-8c2f9aef12cc,fa234f87-000c-4605-9f82-e49cf2a08fae')

*/
GO
