var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;

function BangDuLieu_onCellAfterEdit(h, c) {
    var cs = "Chọn";
    if (Bang_LayGiaTri(h, Bang_arrMaCot[c]) == "0") {
        cs = "Không chọn";
    }
    Bang_arrGiaTri[h][c] = cs;
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

function BangDuLieu_onKeypress_F2() {

    var yes = confirm("F2: Đ/c chắc chắn muốn chọn toàn bộ mục lục ngân sách hiện tại?");
    if (!yes)
        return;

    CheckAll("Chọn");
}

function BangDuLieu_onKeypress_F3() {

    var yes = confirm("F3: Đ/c chắc chắn muốn bỏ chọn toàn bộ mục lục ngân sách hiện tại?");
    if (!yes)
        return;

    CheckAll("Không chọn");
}

function CheckAll(cs) {
    for (var i = 0; i < Bang_nH; i++) {
        Bang_GanGiaTriThatChoO_colName(i, "Map", cs);
    }
}