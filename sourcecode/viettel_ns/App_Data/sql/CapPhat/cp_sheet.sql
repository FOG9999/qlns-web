 


declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '2'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
DECLARE @sLNS NVARCHAR(MAX)							SET @sLNS = '1010000'
DECLARE @iID_MaDonVi NVARCHAR(MAX)					SET @iID_MaDonVi = '14'
DECLARE @loai NVARCHAR(1)							SET @loai = null
--DECLARE @id_chungtu NVARCHAR(100)					SET @id_chungtu = '712adfc8-8c45-44cb-a87d-db39c10ca82b'
DECLARE @id_chungtu NVARCHAR(100)					SET @id_chungtu = '1a40f703-bf9d-4bf0-a5cf-1482cb9c2b1a'

DECLARE @date datetime								SET @date = GETDATE()


--###--


select * from
(

SELECT 
	sLNS,sL,sK,sM
	,sXauNoiMa=CONVERT(nvarchar(10),sLNS)+'-'+CONVERT(nvarchar(10),sL)+'-'+CONVERT(nvarchar(10),sK)+'-'+CONVERT(nvarchar(10),sM)+'_'+iID_MaDonVi
	,iID_MaDonVi
	,rTuChi_PhanBo	=sum(rTuChi_PhanBo)
	,rTuChi_DaCap	=sum(rTuChi_DaCap)
	,rTuChi_ConLai	=sum(rTuChi_PhanBo-rTuChi_DaCap)
	--,rTuChi			=sum(rTuChi)
FROM
(


	select  sLNS,sL,sK,sM
			,iID_MaDonVi
			,rTuChi_PhanBo =sum(rTuChi)
			,rTuChi_DaCap=0
			,rTuChi=0
	from f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,@iID_MaNamNganSach,null,@date,1,@loai)
	where	sLNS in (select * from f_split(@sLNS))
	group by sLNS,sL,sK,sM,iID_MaDonVi

	-- DA CAP PHAT
	union all
	select  sLNS,sL,sK,sM
			,iID_MaDonVi
			,rTuChi_PhanBo =0
			,rTuChi_DaCap=sum(rTuChi)
			,rTuChi=0
	from	CP_CapPhatChiTiet
	where	
			iTrangThai=1
			and iNamLamViec=@iNamLamViec
			and iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
			and iID_MaPhongBan=@iID_MaPhongBan
			and dNgayCapPhat<=@date
			and iID_MaCapPhat<>@id_chungtu
			and sLNS in (select * from f_split(@sLNS))
			and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
	group by sLNS,sL,sK,sM,iID_MaDonVi

)AS A
group by sLNS,sL,sK,sM,iID_MaDonVi
having sum(rTuChi_PhanBo)<>0 or sum(rTuChi_DaCap)<>0

) as dt

-- cap phat
left join 
(
select		 
			id=CONVERT(nvarchar(10),sLNS)+'-'+CONVERT(nvarchar(10),sL)+'-'+CONVERT(nvarchar(10),sK)+'-'+CONVERT(nvarchar(10),sM)+'_'+iID_MaDonVi
			,iID_MaCapPhatChiTiet
			,iID_MaDonVi
			,rTuChi
	from	CP_CapPhatChiTiet
	where	
			iTrangThai=1
			and iNamLamViec=@iNamLamViec
			--and iID_MaNamNganSach=@iID_MaNamNganSach
			and iID_MaPhongBan=@iID_MaPhongBan
			and iID_MaCapPhat=@id_chungtu
			and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
) 
as cp
on cp.id = dt.sXauNoiMa and cp.iID_MaDonVi=dt.iID_MaDonVi
