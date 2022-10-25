var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsQuyetToan_Url_Frame = "";

function BangDuLieu_onCellAfterEdit(h, c) {
    var cs = "Đặc thù";
    if (Bang_LayGiaTri(h, Bang_arrMaCot[c]) == "0") {
        cs = "Không phải đặc thù";
    }
    Bang_arrGiaTri[h][c] = cs;
}

function BangDuLieu_onKeypress_F2() {

    var yes = confirm("F2: Đ/c chắc chắn muốn chọn toàn bộ mục lục ngân sách hiện tại?");
    if (!yes)
        return;

    CheckAll("Đặc thù");
}

function BangDuLieu_onKeypress_F3() {

    var yes = confirm("F3: Đ/c chắc chắn muốn bỏ chọn toàn bộ mục lục ngân sách hiện tại?");
    if (!yes)
        return;

    CheckAll("Không phải đặc thù");
}

function CheckAll(cs) {
    for (var i = 0; i < Bang_nH; i++) {
        Bang_GanGiaTriThatChoO_colName(i, "DacThu", cs);
    }
}