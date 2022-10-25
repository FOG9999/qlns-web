

--select distinct sLNS,sL,sK,sM,sTM from f_mlns(2018)
--group by sLNS,sL,sK,sM,sTM
----order by sXauNoiMa


--select * from f_mlns(2018)
----where	sTM='7008'
--order by sXauNoiMa



-- dieu chinh nhap sai muc luc ngan sach cua b10

select * from CP_CapPhatChiTiet

--update CP_CapPhatChiTiet
--set sXauNoiMa='2030100-100-103-7000-7017-10-72', sK='103',iID_MaMucLucNganSach='8567B0C7-5611-4374-9C31-4A994639D576',iID_MaMucLucNganSach_Cha='AA27A21E-03EF-4388-81F0-FCDB7D76FCD8'
where  iNamLamViec=2018 
		and	sXauNoiMa='2030100-100-101-7000-7017-10-72'
		and sMoTa=N'Nghiên cứu khoa học theo các đề tài - KHQS'
		--and sID_MaNguoiDungTao='chuctc'
