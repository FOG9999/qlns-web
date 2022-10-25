var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;

function BangDuLieu_onCellAfterEdit(h, c) {
    if (Bang_arrMaCot[c] == "Hide"){
        var cs = "Hiện";
        if (Bang_LayGiaTri(h, Bang_arrMaCot[c]) == "1") {
            cs = "Ẩn";
        }
        Bang_arrGiaTri[h][c] = cs;
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
        Bang_arrMaHang[h] = "";
        Bang_keys.fnSetFocus(h, 0);
    }
}

function BangDuLieu_onKeypress_F2(h, c) {
    BangDuLieu_ThemHangMoi(h + 1, h);
}

function BangDuLieu_onKeypress_F3() {

    var yes = confirm("F3: Đ/c chắc chắn muốn chọn hiện toàn bộ mục lục số kiểm tra trong báo cáo?");
    if (!yes)
        return;

    CheckAll("Ẩn");
}

function BangDuLieu_onKeypress_F4() {

    var yes = confirm("F3: Đ/c chắc chắn muốn chọn ẩn toàn bộ mục lục số kiểm tra trong báo cáo?");
    if (!yes)
        return;

    CheckAll("Hiện");
}

function CheckAll(cs) {
    for (var i = 0; i < Bang_nH; i++) {
        Bang_GanGiaTriThatChoO_colName(i, "Hide", cs);
    }
}