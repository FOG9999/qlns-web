


--declare @iNamLamViec int							set @iNamLamViec = 2019
--declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
--declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '2'
--declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
--DECLARE @sLNS NVARCHAR(MAX)							SET @sLNS = '2020201'
--DECLARE @iID_MaDonVi NVARCHAR(MAX)					SET @iID_MaDonVi = '03,57,99'
--DECLARE @loai NVARCHAR(1)							SET @loai = null
--DECLARE @id_chungtu NVARCHAR(100)					SET @id_chungtu = 'd64e1f77-63d5-4a29-8dc7-814e44207a11'
--DECLARE @date datetime								SET @date = GETDATE()


declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = null
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2,4'
--DECLARE @sLNS NVARCHAR(MAX)							SET @sLNS = '2010100,2010200,2010201,2010202,2010203,2010204,2010205,2010207,2010300,2010400,2019900,2020100,2020200,2020300,2020301,2020400,2020500,2020501,2020502,2020700,2020700,2020800,2020900,2020901,2020902,2021000,2021100,2021200,2021300,2021400,2021500,2021600,2021700,2021800,2021900,2029900,2030100,2030200,2030300,2039900,2040100,2040200,2040201,2040202,2040300,2040301,2040302,2040400,2040500,2040700,2040700,2040800,2040801,2040900,2041000,2041100,2041200,2041201,2041202,2041203,2041205,2041300,2041400,2049900,2050100,2050200,2059900,2070101,2070102,2070103,2070199,2070200,2070201,2070202,2070300,2070400,2070401,2070402,2070403,2070501,2070502,2070701,2070702,2070700,2070701,2070708,2070709,2070801,2070802,2070901,2070902,2070903,2071000,2071001,2071002,2071200,2071201,2079900,2070000,2070100,2070200,2070300,2070400,2070500,2070700,2070701,2070702,2070700,2070800,2070900,2071000,2071100,2071101,2071102,2071200,2071300,2071400,2071401,2071402,2071403,2071404,2071500,2071600,2071700,2071800,2071801,2071802,2071900,2072000,2079900,2080000,2089900,2090100,2090101,2090200,2090201,2090202,2099900,2100000,2109900,2110101,2110102,2110400,2110401,2110500,2110700,2110701,2110702,2111000,2111100,2111200,2111300,2111401,2111402,2111403,2111404,2119900,2120100,2120200,2120900,2129900,2130100,2130200,2130201,2130300,2130400,2130401,2130402,2130403,2130404,2130405,2130407,2130407,2130500,2130700,2139900,2140100,2140101,2140109,2140199,2140200,2149900,2150300,2160100,2169900,2170100,2170200,2170300,2170400,2179900,2180100,2180101,2180102,2180105,2180107,2180200,2180300,2180301,2180302,2180401,2189900,2200000,2209900,2960101,2960201,2980200,2980400'
DECLARE @sLNS NVARCHAR(MAX)							SET @sLNS = '3010000'
DECLARE @iID_MaDonVi NVARCHAR(MAX)					SET @iID_MaDonVi = null
DECLARE @loai NVARCHAR(1)							SET @loai = null
--DECLARE @id_chungtu NVARCHAR(100)					SET @id_chungtu = '712adfc8-8c45-44cb-a87d-db39c10ca82b'
DECLARE @id_chungtu NVARCHAR(100)					SET @id_chungtu = '075bb218-17ce-4cc0-b7f2-43228913c6c6'

DECLARE @date datetime								SET @date = GETDATE()


--###--


select * from
(

SELECT 
	sLNS,sL,sK,sM,sTM,sTTM,sNG
	,sXauNoiMa=		CONVERT(nvarchar(10),sLNS)+'-'
				+	CONVERT(nvarchar(10),sL)+'-'
				+	CONVERT(nvarchar(10),sK)+'-'
				+	CONVERT(nvarchar(10),sM)+'-'
				+	CONVERT(nvarchar(10),sTM)+'-'
				+	CONVERT(nvarchar(10),sTTM)+'-'
				+	CONVERT(nvarchar(10),sNG)+'_'
				+	iID_MaDonVi
	--,sXauNoiMa=CONVERT(nvarchar(10),sLNS)+'-'+CONVERT(nvarchar(10),sL)+'-'+CONVERT(nvarchar(10),sK)+'-'+CONVERT(nvarchar(10),sM)+'-'+CONVERT(nvarchar(10),sTM)
	--,sXauNoiMa_Cha=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM

	,iID_MaDonVi
	,rTuChi_PhanBo	=sum(rTuChi_PhanBo)
	,rTuChi_DaCap	=sum(rTuChi_DaCap)
	,rTuChi_ConLai	=sum(rTuChi_PhanBo-rTuChi_DaCap)
	--,rTuChi			=sum(rTuChi)
FROM
(


	select  sLNS,sL,sK,sM,sTM,sTTM,sNG
			,iID_MaDonVi
			,rTuChi_PhanBo =sum(rTuChi)
			,rTuChi_DaCap=0
			,rTuChi=0
	from f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,@iID_MaNamNganSach,null,@date,1,@loai)
	where	(@sLNS is null or sLNS in (select * from f_split(@sLNS)))
	group by sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	-- DA CAP PHAT
	union all
	select  sLNS,sL,sK,sM,sTM,sTTM,sNG
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
	group by sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

)AS A
group by sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
having sum(rTuChi_PhanBo)<>0 or sum(rTuChi_DaCap)<>0

) as dt

-- cap phat
full join 
(
select		 
			id=
			CONVERT(nvarchar(10),sLNS)+'-'
				+	CONVERT(nvarchar(10),sL)+'-'
				+	CONVERT(nvarchar(10),sK)+'-'
				+	CONVERT(nvarchar(10),sM)+'-'
				+	CONVERT(nvarchar(10),sTM)+'-'
				+	CONVERT(nvarchar(10),sTTM)+'-'
				+	CONVERT(nvarchar(10),sNG)+'_'
				+	iID_MaDonVi
			--CONVERT(nvarchar(10),sLNS)+'-'+CONVERT(nvarchar(10),sL)+'-'+CONVERT(nvarchar(10),sK)+'-'+CONVERT(nvarchar(10),sM)+'-'+CONVERT(nvarchar(10),sTM)+'_'+iID_MaDonVi
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
