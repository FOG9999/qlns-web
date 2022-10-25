var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsQuyetToan_Url_Frame = "";
function BangDuLieu_onLoad() {
    if (typeof Bang_arrCSMaCot["bDongY"] == "undefined") {
        BangDuLieu_CoCotDuyet = false;
    }
    else {
        BangDuLieu_CoCotDuyet = true;
    }
    if (typeof Bang_arrCSMaCot["rTongSo"] == "undefined") {
        BangDuLieu_CoCotTongSo = false;
    }
    else {
        BangDuLieu_CoCotTongSo = true;
    }
}

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

function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    if (h != null && BangDuLieu_DuocSuaChiTiet) {
        Bang_ThemHang(h, hGiaTri);
        //Gan lai ma hang ="": Hang moi
        Bang_arrMaHang[h] = "";
        Bang_keys.fnSetFocus(h, 0);
    }
}

function BangDuLieu_onKeypress_F2(h, c) {
    BangDuLieu_ThemHangMoi(h + 1, h);
}

//function BangDuLieu_onKeypress_F4(h, c) {
//    parent.jsBH_Dialog_Show();
//}


//function BangDuLieu_onKeypress_F5(h, c) {
//    parent.jsGTST_Dialog_Show();
//}

//function BangDuLieu_onKeypress_F6(h, c) {
//    parent.jsGTBL_Dialog_Show();
//}