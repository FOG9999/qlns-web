

--###--

select	iID_MaChuKy,sKyHieu,sTen,sMoTa,iSTT 
from	NS_DanhMucChuKy
where	iTrangThai=1
		and (@sKyHieu is null or sKyHieu like @sKyHieu)
		and (@sTen is null or sTen like @sTen)
order by iSTT,sKyHieu


--update NS_DanhMucChuKy
--set iSTT=99999
