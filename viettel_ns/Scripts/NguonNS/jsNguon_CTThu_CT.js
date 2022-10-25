var jsNguon_CTThu_Url_Frame = '';
function jsNguon_CTThu_LoadLaiChiTiet() {
    var url = jsNguon_CTThu_Url_Frame;
  
    var controls = $('input[search-control="1"]');
    var i;
    for (i = 0; i < controls.length; i++) {
        var field = $(controls[i]).attr("search-field");
        var value = $(controls[i]).val();
        url += "&" + field + "=" + encodeURI(value);
    }
    document.getElementById("ifrChiTietChungTu").src = url;

}

var jsNguon_CTThu_Search_inteval = null;
function jsNguon_CTThu_Search_clearInterval() {
    clearInterval(jsNguon_CTThu_Search_inteval);
}
function jsNguon_CTThu_Search_onkeypress(e) {
    jsNguon_CTThu_Search_clearInterval();
    if (e.keyCode == 13) {
        jsNguon_CTThu_LoadLaiChiTiet();
    }
    else {
        jsNguon_CTThu_Search_inteval = setInterval(function () { jsNguon_CTThu_Search_clearInterval(); jsNguon_CTThu_LoadLaiChiTiet(); }, 2000);
    }
}

function BangDuLieu_onCellAfterEdit(h, c) {
    
    if (Bang_arrMaCot[c] == "SoTien") {
        BangDuLieu_CapNhapLaiHangCha(h, c);
        var ls = Bang_arrMaHang[h].split("_");
        Bang_arrMaHang[h] = ls[0] + "_" + ls[1] + "_" + Bang_arrGiaTri[h][c];
    }
    
    return true;    
}

function BangDuLieu_CapNhapLaiHangCha(h, c) {
    if (Bang_arrType[c] == 1) {
        var csCha = h, GiaTri;
        while (Bang_arrCSCha[csCha] >= 0) {
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
    }
}