

declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2020
declare @namNS int									set @namNS = 2019
declare @id_donvi nvarchar(20)						set @id_donvi=null
declare @nganh nvarchar(20)						set @nganh=null

declare @id_phongban nvarchar(20)					set @id_phongban=null
select * from (
select	XauNoiMa							
							, HangNhap	
							, HangMua	
					from	SKT_ComDatas
					where	NamNS=@namNS
							and (@id_phongban is null or Id_PhongBan=@id_phongban)
							and (@nganh IS NULL OR Ng in (select * from f_split(@nganh)))
							and (HangNhap + HangMua) <> 0
							and Ng <> '00'
				
					) DT

					left join 

					(
					select	iID_MaMucLucNganSach
							, sXauNoiMa
					from	NS_MucLucNganSach
					where	iTrangThai = 1
							and iNamLamViec = @namNS
							and (@nganh is null or sNG in (select * from f_split(@nganh)))
							and sNG <> '00'
					) mlns

					on DT.XauNoiMa = mlns.sXauNoiMa

					left join

					(
					select	Id_MLNhuCau, Id_MLNS
					from	SKT_NCMLNS
					where	NamLamViec = @nam					
					) ncmlns

					on mlns.iID_MaMucLucNganSach = ncmlns.Id_MLNS

					left join

					(
					select	Id = Id_MLNhuCau, Id_MLSKT
					from	SKT_NCSKT
					where	NamLamViec = @nam					
					) ncskt

					on ncmlns.Id_MLNhuCau = ncskt.Id


select		Id_MLSKT
			, HangNhap		=sum(HangNhap)/@dvt 
			, HangMua		=sum(HangMua)/@dvt
from
			(	
			select	* 
			from
					(
					select	XauNoiMa							
							, HangNhap	
							, HangMua	
					from	SKT_ComDatas
					where	NamNS=@namNS
							and (@id_phongban is null or Id_PhongBan=@id_phongban)
							and (@nganh IS NULL OR Ng in (select * from f_split(@nganh)))
							and (HangNhap + HangMua) <> 0
				
					) DT

					left join 

					(
					select	iID_MaMucLucNganSach
							, sXauNoiMa
					from	NS_MucLucNganSach
					where	iTrangThai = 1
							and iNamLamViec = @namNS
							and (@nganh is null or sNG in (select * from f_split(@nganh)))
							and sNG <> '00'
					) mlns

					on DT.XauNoiMa = mlns.sXauNoiMa

					left join

					(
					select	Id_MLNhuCau, Id_MLNS
					from	SKT_NCMLNS
					where	NamLamViec = @nam					
					) ncmlns

					on mlns.iID_MaMucLucNganSach = ncmlns.Id_MLNS

					left join

					(
					select	Id = Id_MLNhuCau, Id_MLSKT
					from	SKT_NCSKT
					where	NamLamViec = @nam					
					) ncskt

					on ncmlns.Id_MLNhuCau = ncskt.Id_MLSKT

					left join

					(
					select	Id as Id_SKT
							, KyHieu 
					from	SKT_MLSKT 
					where	NamLamViec=@nam
					) ml
					on ncskt.Id=ml.Id_SKT
			)as DT2
where		Id_MLSKT is not null
group by	Id_MLSKT