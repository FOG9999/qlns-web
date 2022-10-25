

--###--

select	iID_MaDonVi, sMoTa=iID_MaDonVi + ' - ' + sTen from NS_DonVi
where   iTrangThai=1
        and iNamLamViec_DonVi=@nam
        and iID_MaDonVi in (
                    select iID_MaDonVi from (select iID_MaPhongBan, iID_MaDonVi from NS_PhongBan_DonVi where iTrangThai=1 and iNamLamViec=@nam) as a
                    inner join 
                    (select iID_MaPhongBan, sKyHieu from NS_PhongBan where iTrangThai=1) as b
                    on b.iID_MaPhongBan=a.iID_MaPhongBan 
                    where (@id_phongban is null or sKyHieu=@id_phongban)
                )
		and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
order by iID_MaDonVi