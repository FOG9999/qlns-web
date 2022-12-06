
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='07'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach  nvarchar(20)			set @iID_MaNamNganSach = '2'
declare @iID_MaDonVi		nvarchar(2)				set @iID_MaDonVi='82'
declare @iID_MaChungTu		nvarchar(2000)			set @iID_MaChungTu='89f3a14d-d05f-4d06-98d7-dcc21a0382e6'
--declare @iID_MaChungTu		nvarchar(2000)			set @iID_MaChungTu='89f3a14d-d05f-4d06-98d7-dcc21a0382e6'
--declare @sLNS		nvarchar(200)					set @sLNS='2'
declare @dNgayChungTu datetime						set @dNgayChungTu = '2018-07-05 00:00:00.000'
declare @iThang_Quy		int							set @iThang_Quy=1

declare @sLNS1		nvarchar(200)					set @sLNS1='1'
declare @sLNS		nvarchar(7)						set @sLNS=null
declare @sL			nvarchar(3)						set @sL=NULL
declare @sK			nvarchar(3)						set @sK=NULL
declare @sM			nvarchar(4)						set @sM=null
declare @sTM		nvarchar(4)						set @sTM=NULL
declare @sTTM		nvarchar(2)						set @sTTM=NULL
declare @sNG		nvarchar(2)						set @sNG=NULL
--###--



select	sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,sMoTa, iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha, bLaHangCha,iID_MaDonVi,
			--rDaQuyetToan=0.0,
		iID_MaChungTuChiTiet,rChiTieu, rDaQuyetToan,rTuChi,rDonViDeNghi, rVuotChiTieu, rTonThatTonDong,rDaCapTien, rChuaCapTien, sGhiChu
from
(
select sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,sMoTa, iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha, bLaHangCha
from	NS_MucLucNganSach
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and (sLNS in (select * from f_split(@sLNS1)) or left(sLNS,1) in (select * from f_split(@sLNS1)))
		
		--and (@sLNS is null or sLNS like @sLNS)
		--and (@sL is null or sL like @sL) 
		--and (@sK is null or sK like @sK) 
		--and (@sM is null or sM like @sM) 
		--and (@sTM is null or sTM like @sTM) 
		--and (@sTTM is null or sTTM like @sTTM) 
		--and (@sNG is null or sNG like @sNG) 
) as mlns
	left join
	(select iID_MaChungTuChiTiet, 
			sXauNoiMa as id,
			iID_MaDonVi,
			--rDaQuyetToan=0.0,
			rTuChi,rDonViDeNghi, rVuotChiTieu, rTonThatTonDong,rDaCapTien, rChuaCapTien, sGhiChu
		from	QTA_ChungTuChiTiet
		where	iTrangThai=1
			and iNamLamViec=@iNamLamViec
			and iID_MaChungTu=@iID_MaChungTu
			and iID_MaPhongBan=@iID_MaPhongBan
			and iID_MaDonVi=@iID_MaDonVi
			and (sLNS in (select * from f_split(@sLNS1)) or left(sLNS,1) in (select * from f_split(@sLNS1)))
			and (@sLNS is null or sLNS like @sLNS) 
			and (@sL is null or sL like @sL) 
			and (@sK is null or sK like @sK) 
			and (@sM is null or sM like @sM) 
			and (@sTM is null or sTM like @sTM) 
			and (@sTTM is null or sTTM like @sTTM) 
			and (@sNG is null or sNG like @sNG)
	) as qt
	on mlns.sXauNoiMa=qt.id

	left join
	-- chi tieu
	(select sXauNoiMa as id, CAST(rTuChi as decimal(18,0)) as rChiTieu 
	from	f_chitieu(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,@iID_MaNamNganSach, @iID_MaNguonNganSach,@sLNS1, @dNgayChungTu,1,null)
	where	(@sLNS is null or sLNS like @sLNS) 
		and (@sL is null or sL like @sL) 
		and (@sK is null or sK like @sK) 
		and (@sM is null or sM like @sM) 
		and (@sTM is null or sTM like @sTM) 
		and (@sTTM is null or sTTM like @sTTM) 
		and (@sNG is null or sNG like @sNG)
	) as dt
	on mlns.sXauNoiMa=dt.id

	-- da quyet toan
	left join
	(
	select  
		--iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha, sLNS,sL,sK,sM,sTM, sTTM,sNG,
		sXauNoiMa as id,
		--iID_MaDonVi,
		rDaQuyetToan	=sum(rTuChi)
	from	QTA_ChungTuChiTiet
	where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaNguonNganSach=@iID_MaNguonNganSach
		and iID_MaNamNganSach=@iID_MaNamNganSach
		and iID_MaPhongBan=@iID_MaPhongBan
		and iID_MaDonVi=@iID_MaDonVi
		and (sLNS in (select * from f_split(@sLNS1)) or left(sLNS,1) in (select * from f_split(@sLNS1)))

		and (@sLNS is null or sLNS like @sLNS) 
		and (@sL is null or sL like @sL) 
		and (@sK is null or sK like @sK) 
		and (@sM is null or sM like @sM) 
		and (@sTM is null or sTM like @sTM) 
		and (@sTTM is null or sTTM like @sTTM) 
		and (@sNG is null or sNG like @sNG) 

		and (iThang_Quy<@iThang_Quy 
			or (iThang_Quy=@iThang_Quy  
				and iID_MaChungTu in (	select iID_MaChungTu from QTA_ChungTu 
										where	iTrangThai=1  
												and iNamLamViec=@iNamLamViec 
												and iID_MaPhongBan=@iID_MaPhongBan 
												and dNgayChungTu<@dNgayChungTu)))
	group by iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha, sLNS,sL,sK,sM,sTM, sTTM,sNG,sXauNoiMa,	iID_MaDonVi
	
	) as qt1
	on mlns.sXauNoiMa=qt1.id
where   (@sLNS is null or sLNS like @sLNS)
		and (@sL is null or sL like @sL or sL='') 
		and (@sK is null or sK like @sK or sK='') 
		and (@sM is null or sM like @sM or sM='') 
		and (@sTM is null or sTM like @sTM or sTM='') 
		and (@sTTM is null or sTTM like @sTTM or sTTM='') 
		and (@sNG is null or sNG like @sNG or sNG='') 
order by sLNS,sL,sK,sM,sTM,sTTM,sNG
