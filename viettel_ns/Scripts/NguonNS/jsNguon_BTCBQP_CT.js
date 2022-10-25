function BangDuLieu_onCellAfterEdit(h, c) {
    var cs = "Chọn";    
    if (Bang_LayGiaTri(h, Bang_arrMaCot[c]) == "0") {    
        cs = "Không chọn";
    }
    Bang_arrGiaTri[h][c] =cs;
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