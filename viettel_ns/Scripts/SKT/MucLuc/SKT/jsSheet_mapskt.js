var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;

function BangDuLieu_onKeypress_Delete(h, c) {
    if (BangDuLieu_DuocSuaChiTiet && h != null) {
        BangDuLieu_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
        else if (Bang_nH > 0) {

        }
    }
}

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];
        Bang_HienThiDuLieu();
        return true;
    }
    return false;
}

function BangDuLieu_onKeypress_F2(h, c) {
    var iID_MaChiTiet = Bang_arrMaHang[h];

    var iID_MaMucLuc = iID_MaChiTiet.split('_')[0];
    url = unescape(BangDuLieu_Url_Mapping + '?id=' + iID_MaMucLuc + '&loai=' + 1);
    window.open(url, '_blank');
}

function BangDuLieu_onKeypress_F3(h, c) {
    var iID_MaChiTiet = Bang_arrMaHang[h];

    var iID_MaMucLuc = iID_MaChiTiet.split('_')[0];
    url = unescape(BangDuLieu_Url_Mapping + '?id=' + iID_MaMucLuc + '&loai=' + 2);
    window.open(url, '_blank');
}