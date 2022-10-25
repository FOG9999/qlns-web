declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2019
declare @Ng nvarchar(2)								set @Ng = '51'

--###--

select		Id_DonVi = iID_MaDonVi
			, TenDonVi = sTenDonVi	
			, sXauNoiMa
			, Nganh = sM + '-' + sTM+'-' + sTTM + '-' + sNG
			, TenNganh
			, sNG
			, TuChi = sum((rTuChi+rHangNhap+rHangMua)/@dvt)
from
			(
			-- dự toán
			select	* 
			from	DT_ChungTuChiTiet
			where	(@Ng is null or sNG in (select * from f_split(@Ng)))
					and iNamLamViec = @nam
					and sLNS like '1%' and (sNG in ('60','61','62','64','65','66'))
			) as t1

			-- lay ten nganh
			inner join 
			(select sNG as nganh_id,sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iNamLamViec = @nam and iTrangThai=1 and sLNS='') as nganh
			on t1.sNG=nganh.nganh_id
group by	iID_MaDonVi,sTenDonVi,sXauNoiMa,sM,sTM,sTTM,sNG,TenNganh
order by	sXauNoiMa,iID_MaDonVi