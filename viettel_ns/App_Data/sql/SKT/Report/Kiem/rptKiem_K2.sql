declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
declare @nganh	nvarchar(2000)						set @nganh=null
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi='29'
declare @loai nvarchar(2000)					set @loai=2


--###--

select	Id_DonVi, TenDonVi, sLNS,sL,sK, sM,sTM, sTTM, sNG
		,sXauNoiMa = sLNS + '-' + sL + '-' + sK+ '-' +sM+ '-' +sTM+ '-' +sTTM+ '-' +sNG
		,Nganh= sM + '-' + sTM+'-' + sTTM + '-' + sNG
		,TenNganh, TuChi
from
(
		select * 
		from (
		select	Id_DonVi	=iID_MaDonVi,
				sLNS,sL,sK, sM,sTM, sTTM,sNG,
				TuChi		=sum(rTuChi)/@dvt
		from	DT_ChungTuChiTiet_PhanCap
		where	iTrangThai=1
				and iNamLamViec=@nam
				and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
				and (@nganh is null or sNG in (select * from f_split(@nganh)))
				and sLNS in (1020100) AND MaLoai = ''
		group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG

		union

		select	Id_DonVi	= iID_MaDonVi,
				sLNS = '1020100' ,sL,sK, sM,sTM, sTTM,sNG,
				TuChi		= sum(rTuChi+rHangNhap+rHangMua)/@dvt
		from	DT_ChungTuChiTiet
		where	iTrangThai=1
				and iNamLamViec=@nam
				and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
				and (@nganh is null or sNG in (select * from f_split(@nganh)))
				and MaLoai = 1
				and sLNS='1040100' 
		group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG
		) as t
) as t1

-- lay ten nganh
inner join 
(select sNG as nganh_id,sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iNamLamViec=@nam and iTrangThai=1 and sLNS='') as nganh
on t1.sNG=nganh.nganh_id

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=t1.Id_DonVi

left join 
(select M, Tm, Ttm, Ng, Loai = CASE DacThu WHEN 1 THEN 2 ELSE 1 END  from SKT_MLDacThu where NamNS = @nam) mldt
on t1.sM = mldt.M and t1.sTM = mldt.Tm and t1.sTTM = mldt.Ttm and t1.sNG = mldt.Ng

where	TuChi<>0 and (@loai is null or Loai = @loai)
order by Nganh, Id_DonVi, sLNS,sL,sK, sM,sTM, sTTM,sNG