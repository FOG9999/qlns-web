declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @ng	nvarchar(2000)							set @ng='20,21,22,23,24,25,26,27,28,29,41,42,44,67,70,71,72,73,74,75,76,77,78,81,82'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null


--###--
SELECT  rTuChiDT=sum(rTuChiDT)
		, rTuChiDN=sum(rTuChiDN)
		, rTuChiBV=sum(rTuChiBV)
from    (
		SELECT		rTuChiDT= case when iID_MaPhongBanDich <> '06' and iID_MaDonVi not in ('76','77','79') and LEN(iID_MaDonVi) = 2 and iID_MaDonVi not in ('30','34','35','50','69','70','71','72','73','78','80','88','89','90','91','92','93','94') then SUM((rTuChi+rPhanCap)/@dvt) else 0 end
					, rTuChiDN= case when (iID_MaPhongBanDich = '06' and LEN(iID_MaDonVi) = 2) or iID_MaDonVi in ('30','34','35','50','69','70','71','72','73','78','80','88','89','90','91','92','93','94') then SUM((rTuChi)/@dvt) else 0 end
					, rTuChiBV= case when (iID_MaPhongBanDich <> '06' and iID_MaDonVi in ('76','77','79')) or LEN(iID_MaDonVi) > 2 then SUM((rTuChi)/@dvt) else 0 end
		FROM		DT_ChungTuChiTiet
		WHERE		iTrangThai <> 0 AND (@@DKLNSNV) AND iNamLamViec=@nam @@DKPB @@DKDV
		group by	iID_MaPhongBanDich, iID_MaDonVi) re