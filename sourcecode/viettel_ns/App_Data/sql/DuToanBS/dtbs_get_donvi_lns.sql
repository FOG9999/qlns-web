
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
--declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='fa234f87-000c-4605-9f82-e49cf2a08fae'
--declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='B99EB11D-D5D2-404C-9E53-086908D23F0C'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'
declare @dNgayChungTu datetime						set @dNgayChungTu = '2018-01-15 00:00:00.000'

--###--

 

--select distinct sLNS, sMoTa = SLNS + ' - ' + [dbo].F_MLNS_MoTa(@iNamLamViec,sLNS) from DTBS_ChungTuChiTiet
--where	iTrangThai=1
--		and iNamLamViec=@iNamLamViec
--		and iID_MaPhongBan=@iID_MaPhongBan
--		and iID_MaDonVi in (select * from f_split(@iID_MaDonVi))
--		and iID_MaChungTu in (
--								select iID_MaChungTu from DTBS_ChungTu
--								where	iTrangThai=1 and iNamLamViec=@iNamLamViec
--										and dNgayChungTu=@dNgayChungTu
--										and iID_MaPhongBanDich=@iID_MaPhongBan
--							)

;with ct as(
	select iID_MaChungTu from DTBS_ChungTu
	where	iTrangThai=1 and iNamLamViec=@iNamLamViec
			and dNgayChungTu=@dNgayChungTu
			and sID_MaNguoiDungTao = @username
)

select sLNS,  sMoTa = SLNS + ' - ' + [dbo].F_MLNS_MoTa(@iNamLamViec,sLNS) from
(

select distinct sLNS 
from	DTBS_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaPhongBanDich=@iID_MaPhongBan
		and iID_MaDonVi in (select * from f_split(@iID_MaDonVi))
		and iID_MaChungTu in (select * from ct)
							

union

SELECT  DISTINCT sLNS
FROM    DTBS_ChungTuChiTiet_PhanCap
WHERE   iTrangThai = 1 
        --AND iNamLamViec = @iNamLamViec    
        AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))

		AND (iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM ct))
				
		---- phan cap lan 2: cho cac bql
		--OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
								--where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

		OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
								from DTBS_ChungTuChiTiet 
								where iID_MaChungTu in (
											select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
											where iID_MaChungTu in (   
																	select iID_MaChungTu from DTBS_ChungTu
																	where iID_MaChungTu in (
																							select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																							where iID_MaChungTu in (select * from ct)))))

		---- phan cap lan 2: cho b
		OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
								where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM ct)))
				
		)

) as t
