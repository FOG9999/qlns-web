
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='0dba0387-adce-4ef1-8609-b40d9fea1506'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @phongban nvarchar(2)					set @phongban=null
declare @loai nvarchar(2)					set @loai='8'

--###--

select	Id_DonVi, TenDonVi, sLNS,sL,sK, sM,sTM, sTTM, sNG
		,sXauNoiMa = sLNS + '-' + sL + '-' + sK+ '-' +sM+ '-' +sTM+ '-' +sTTM+ '-' +sNG
		,Nganh= sM + '-' + sTM+'-' + sTTM + '-' + sNG
		,TenNganh, TuChi
from
(
-- phan cap
-- tự chi
select	Id_DonVi	=iID_MaDonVi,
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rTuChi)/@dvt
from	DTBS_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and rTuChi <> 0
		and iID_MaChungTu in (
							 select	iID_MaChungTuChiTiet 
							 FROM	DTBS_ChungTuChiTiet 
							 WHERE  iTrangThai=1 
									AND iNamLamViec=@iNamLamViec
									AND (MaLoai='')
									AND iID_MaDonVi=@iID_MaDonVi
									AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) AND rPhanCap <> 0)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG

union all

select	Id_DonVi	=iID_MaDonVi,
		sLNS = '1020100',sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rTuChi)/@dvt
from	DTBS_ChungTuChiTiet
where	iTrangThai=1
		and rTuChi <> 0 and MaLoai = 1 and @iID_MaDonVi = '51'
		and iID_MaChungTu in (
							 select	iID_MaChungTuChiTiet 
							 FROM	DTBS_ChungTuChiTiet 
							 WHERE  iTrangThai=1 
									AND iNamLamViec=@iNamLamViec
									AND MaLoai=''
									AND iID_MaDonVi=@iID_MaDonVi
									AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) AND rPhanCap <> 0)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG
) as t1

-- lay ten nganh
inner join 
(select sLNS as nganh_id, TenNganh = case when sLNS = '1020100' then N'Ngân sách nghiệp vụ ngành' else sLNS + ' - ' + sMoTa end from NS_MucLucNganSach where iNamLamViec=@iNamLamViec and iTrangThai=1 and sLNS <> '' and sL = '') as nganh
on t1.sLNS=nganh.nganh_id

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@iNamLamViec and iTrangThai=1) as dv
on dv.dv_id=t1.Id_DonVi

where	TuChi<>0
order by Nganh, Id_DonVi, sLNS,sL,sK, sM,sTM, sTTM,sNG