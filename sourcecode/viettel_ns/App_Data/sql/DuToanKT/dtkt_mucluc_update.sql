declare @nam int									set @nam = 2018 

--###--

UPDATE	DTKT_ChungTuChiTiet
set		Code			 = (select top(1)  Code 
							from			DTKT_MucLuc
							where			iTrangThai = 1
											and	NamLamViec = @nam
											and DTKT_ChungTuChiTiet.Ng = DTKT_MucLuc.Ng 
											and DTKT_ChungTuChiTiet.Nganh = DTKT_MucLuc.Nganh
											and DTKT_ChungTuChiTiet.Id_Mucluc = DTKT_MucLuc.Id),		
		Id_MucLuc_Parent = (select top(1)	Id_Parent 
							from			DTKT_MucLuc
							where			iTrangThai = 1
											and	NamLamViec = @nam
											and DTKT_ChungTuChiTiet.Ng = DTKT_MucLuc.Ng 
											and DTKT_ChungTuChiTiet.Nganh = DTKT_MucLuc.Nganh
											and DTKT_ChungTuChiTiet.Id_Mucluc = DTKT_MucLuc.Id),		
		sMoTa			 = (select top(1)	sMoTa 
							from			DTKT_MucLuc
							where			iTrangThai = 1
											and	NamLamViec = @nam
											and DTKT_ChungTuChiTiet.Ng = DTKT_MucLuc.Ng 
											and DTKT_ChungTuChiTiet.Nganh = DTKT_MucLuc.Nganh
											and DTKT_ChungTuChiTiet.Id_Mucluc = DTKT_MucLuc.Id)
where	iTrangThai = 1
		and NamLamViec = @nam