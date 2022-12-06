

--###--

select	iID_MaChuKy, sKyHieu, sTen, 
		sMoTa = case sKyHieu 
				when ''  then sTen
				else sKyHieu + ' - ' + sTen
				end, 
				iSTT from NS_DanhMucChuKy
where	iTrangThai=1
order by iSTT,sKyHieu


--update NS_DanhMucChuKy
--set iSTT=99999
