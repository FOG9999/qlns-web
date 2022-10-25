
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='07'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach  nvarchar(20)			set @iID_MaNamNganSach = '2'
declare @dNgayChungTu datetime						set @dNgayChungTu = '2018-06-25 00:00:00.000'
declare @iID_MaDonVi		nvarchar(2)				set @iID_MaDonVi='82'
declare @iID_MaChungTu		nvarchar(2000)			set @iID_MaChungTu='0daa0d84-a173-4640-81a6-3b309a80f765'
declare @sLNS		nvarchar(200)					set @sLNS='1'

--###--



select * from
(
select sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,sMoTa, iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha, bLaHangCha
from	NS_MucLucNganSach
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and (sLNS in (select * from f_split(@sLNS)) or left(sLNS,1) in (select * from f_split(@sLNS)))
) as mlns
left join

(

select * from
(

 select iID_MaChungTuChiTiet, 
		--iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha, sLNS,sL,sK,sM,sTM, sTTM,sNG,
		sXauNoiMa as qt_id,
		iID_MaDonVi,
		rChiTieu=0.0,
		rDaQuyetToan=0.0,
		rTuChi,rDonViDeNghi, rVuotChiTieu, rTonThatTonDong,rDaCapTien, rChuaCapTien, sGhiChu
 from	QTA_ChungTuChiTiet
 where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaChungTu=@iID_MaChungTu
		and iID_MaPhongBan=@iID_MaPhongBan
		and iID_MaDonVi=@iID_MaDonVi

) as a

) as qt
on qt.qt_id=mlns.sXauNoiMa
order by sLNS,sL,sK,sM,sTM,sTTM,sNG


 --select iID_MaChungTuChiTiet, 
	--	iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha, sLNS,sL,sK,sM,sTM, sTTM,sNG,sXauNoiMa,
	--	iID_MaDonVi,
	--	rTuChi,rDonViDeNghi, rVuotChiTieu, rTonThatTonDong,rDaCapTien, rChuaCapTien, sGhiChu
 --from	QTA_ChungTuChiTiet
 --where	iTrangThai=1
	--	and iNamLamViec=@iNamLamViec
	--	and iID_MaChungTu=@iID_MaChungTu
	--	and sLNS in (select * from f_split(@sLNS))
