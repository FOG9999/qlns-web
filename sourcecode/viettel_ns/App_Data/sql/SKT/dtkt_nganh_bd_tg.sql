
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2020 
declare @namNS int									set @namNS = 2019 
declare @id_phongban nvarchar(2000)			set @id_phongban=null

declare @id_donvi nvarchar(2000)			set @id_donvi=null
declare @nganh nvarchar(2000)			set @nganh='60'
select		iID_MaMucLucNganSach as Id
									, sXauNoiMa
									, NamMl = iNamLamViec
						 from		NS_MucLucNganSach 
						 where		iNamLamViec = @namNS
									and iTrangThai = 1
									and iID_MaMucLucNganSach 
												in	(
													select	Id_MLNS
													from	SKT_MLDacThu 
													where	NamLamViec = @nam 
															and NamNS = @namNS
															and DacThu = 0)
															order by sXauNoiMa
select * from 
(select	
								XauNoiMa
								, NamNS
								, Id_DonVi
								, TuChi		= sum(TuChi) /@dvt
						from	SKT_ComDatasDacThu
						where	NamLamViec = @nam
								and (@id_phongban is null or Id_PhongBan = @id_phongban)
								and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
								and (@nganh is null or Ng in (select * from f_split(@nganh)))
						group by NamNS,Id_DonVi,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng
						) as t1 

						left join 

						(select		iID_MaMucLucNganSach as Id
									, sXauNoiMa
									, NamMl = iNamLamViec
						 from		NS_MucLucNganSach 
						 where		iNamLamViec = @namNS
									and iTrangThai = 1
									and iID_MaMucLucNganSach 
												in	(
													select	Id_MLNS
													from	SKT_MLDacThu 
													where	NamLamViec = @nam 
															and NamNS = @namNS
															and DacThu = 1)) as ml
						 ON t1.XauNoiMa = ml.sXauNoiMa
		
						--left join 
		
						--(select		Id_MLNhuCau, Id_MLNS
						-- from		SKT_NCMLNS
						-- where		NamLamViec = @nam 			 
						--) as map

						-- on ml.Id = map.Id_MLNS


--###--
select	*
			from
				(
				select	Id_DonVi
						, Id_MLNhuCau = CASE WHEN Id_DonVi = '29' and Id = '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4' THEN 'b4b66e4a-9c40-4db7-a9eb-8bd5e0e93858' 
											 WHEN Id_DonVi = '31' and Id = '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4' THEN 'e2c28788-3b05-47e5-b97f-9d2b237d2406' 
											 WHEN Id_DonVi = '33' and Id = '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4' THEN '3682b520-6fc6-4883-8a90-3cefef911c5f' 
											 ELSE Id_MLNhuCau end 
						, TuChi 
				from
						(			
						select	
								XauNoiMa
								, NamNS
								, Id_DonVi
								, TuChi		= sum(TuChi) /@dvt
						from	SKT_ComDatasDacThu
						where	NamLamViec = @nam
								and (@id_phongban is null or Id_PhongBan = @id_phongban)
								and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
								and (@nganh is null or Ng in (select * from f_split(@nganh)))
						group by NamNS,Id_DonVi,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng
						) as t1 

						left join 

						(select		iID_MaMucLucNganSach as Id
									, sXauNoiMa
									, NamMl = iNamLamViec
						 from		NS_MucLucNganSach 
						 where		iNamLamViec = @namNS
									and iTrangThai = 1
									and iID_MaMucLucNganSach 
												in	(
													select	Id_MLNS
													from	SKT_MLDacThu 
													where	NamLamViec = @nam 
															and NamNS = @namNS
															and DacThu = 1)) as ml
						 ON t1.XauNoiMa = ml.sXauNoiMa
		
						left join 
		
						(select		Id_MLNhuCau, Id_MLNS
						 from		SKT_NCMLNS
						 where		NamLamViec = @nam 			 
						) as map

						 on ml.Id = map.Id_MLNS
				where	Id_DonVi IS NOT NULL) sdt
