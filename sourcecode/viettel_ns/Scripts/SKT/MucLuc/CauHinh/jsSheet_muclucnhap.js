var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsQuyetToan_Url_Frame = "";

function BangDuLieu_onCellAfterEdit(h, c) {
    if (Bang_arrMaCot[c] == "Map"){
        var cs = "Khoá";
        if (Bang_LayGiaTri(h, Bang_arrMaCot[c]) == "0") {
            cs = "Không khóa";
        }
        Bang_arrGiaTri[h][c] = cs;
    }
}

