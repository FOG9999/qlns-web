select khv.*,nns.sTen as sTenNguonVon,mlns.sMoTa as sTenLoaiNganSach, mlns.sL as sLoai,mlns.sK as sKhoan,dv.sTen as sTenDonvi
from VDT_KHV_KeHoach5Nam khv
left join NS_NguonNganSach nns ON nns.iID_MaNguonNganSach = khv.iID_NguonVonID
left join NS_MucLucNganSach mlns ON mlns.iID_MaMucLucNganSach = khv.iID_KhoanNganSachID
left join NS_DonVi dv ON dv.iID_Ma = khv.iID_DonViQuanLyID
where khv.iID_KeHoach5NamID = @id

--select khv.*,nns.sTen as sTenNguonVon,mlns.sMoTa as sTenLoaiNganSach, mlns.sL as sLoai,mlns.sK as sKhoan,dv.sTen as sTenDonvi
--from VDT_KHV_KeHoach5Nam khv
--left join NS_NguonNganSach nns ON nns.iID_MaNguonNganSach = khv.iID_NguonVonID
--left join NS_MucLucNganSach mlns ON mlns.iID_MaMucLucNganSach = khv.iID_KhoanNganSachID
--left join NS_DonVi dv ON dv.iID_Ma = khv.iID_DonViQuanLyID
--where khv.iID_KeHoach5NamID = @id
--AND ( @dNgayLap IS NULL OR ( khv.dNgayQuyetDinh >= @dNgayLap ) ) 