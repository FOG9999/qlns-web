rptDuToan_TongHopCuc_ThuNop_ChiTietdeclare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
declare @xLNS nvarchar(MAX)							set @xLNS='1030100,1030900'


--###--

SELECT		iID_MaDonVi
			, sTen as sTenDonVi
			@@DKSELECT
FROM		(
			-- Dữ liệu ngoài B6 --
			SELECT		*
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai in (1,2)
						AND  sLNS LIKE '8%' AND iNamLamViec=@nam @@DKDV @@DKPB
			) as a

			inner join 

			(select iID_MaDonVi as id, sTen from NS_DonVi where iNamLamViec_DonVi = @nam and iTrangThai = 1) as dv

			on a.iID_MaDonVi = dv.id
GROUP BY	iID_MaDonVi,sTen
HAVING		@@DKHAVING 
ORDER BY	iID_MaDonVi