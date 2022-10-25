var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsQuyetToan_Url_Frame = "";

function BangDuLieu_onCellAfterEdit(h, c) {
     var tong = 0;
    for (var i = 1; i < Bang_nH - 1; i++) {
        if (Bang_arrLaHangCha[i] == false) {
            tong += Bang_arrGiaTri[i][c];
        }
    }
    Bang_GanGiaTriThatChoO(0, c, tong);
    Bang_GanGiaTriThatChoO(Bang_nH - 1, c, tong);
    BangDuLieu_CapNhapLaiHangCha(h, c);
}

function BangDuLieu_CapNhapLaiHangCha(h, c) {
    if (Bang_arrType[c] == 1) {
        var csCha = h, GiaTri;
        while (Bang_arrCSCha[csCha] > 0) {
            csCha = Bang_arrCSCha[csCha];
            GiaTri = BangDuLieu_TinhTongHangCon(csCha, c);
            Bang_GanGiaTriThatChoO(csCha, c, GiaTri);
        }
    }
}

function BangDuLieu_TinhTongHangCon(csCha, c) {
    var h, vR = 0;
    for (h = 0; h < Bang_arrCSCha.length; h++) {
        if (csCha == Bang_arrCSCha[h]) {
            vR += parseFloat(Bang_arrGiaTri[h][c]);
        }
    }
    return vR;
}

function BangDuLieu_onKeypress_Delete(h, c) {
    if (BangDuLieu_DuocSuaChiTiet && h != null) {
        Bang_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
        else if (Bang_nH > 0) {

        }
    }
}