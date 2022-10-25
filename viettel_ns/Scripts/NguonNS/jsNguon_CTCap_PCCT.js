var jsNguon_CTCap_Url_Frame = '';
var jsNguon_CTCap_Url = '';
function jsNguon_CTCap_LoadLaiChiTiet() {
    var url = jsNguon_CTCap_Url_Frame;
  
    var controls = $('input[search-control="1"]');
    var i;
    for (i = 0; i < controls.length; i++) {
        var field = $(controls[i]).attr("search-field");
        var value = $(controls[i]).val();
        url += "&" + field + "=" + encodeURI(value);
    }
    document.getElementById("ifrChiTietChungTu").src = url;

}

var jsNguon_CTCap_Search_inteval = null;
function jsNguon_CTCap_Search_clearInterval() {
    clearInterval(jsNguon_CTCap_Search_inteval);
}
function jsNguon_CTCap_Search_onkeypress(e) {
    jsNguon_CTCap_Search_clearInterval();
    if (e.keyCode == 13) {
        jsNguon_CTCap_LoadLaiChiTiet();
    }
    else {
        jsNguon_CTCap_Search_inteval = setInterval(function () { jsNguon_CTCap_Search_clearInterval(); jsNguon_CTCap_LoadLaiChiTiet(); }, 2000);
    }
}

function BangDuLieu_onCellAfterEdit(h, c) {
    
    if (Bang_arrMaCot[c] == "SoTien") {
        var ls = Bang_arrMaHang[h].split("_");
        var csCha = Bang_arrCSCha[h];
        var value = parseFloat(Bang_arrGiaTri[csCha][c]) - (parseFloat(Bang_arrGiaTri[h][c]) - parseFloat(ls[2]));
        Bang_GanGiaTriThatChoO(csCha, c, value);
        var mauDuong = '#ffff00';
        var mauBang0 = '#3399FF';
        var mauAm = '#FF0000';
        if (value > 0)
            Bang_GanGiaTriThatChoO_colName(csCha, "sMauSac", mauDuong);
        else if (value < 0)
            Bang_GanGiaTriThatChoO_colName(csCha, "sMauSac", mauAm);
        else
            Bang_GanGiaTriThatChoO_colName(csCha, "sMauSac", mauBang0);
        Bang_arrMaHang[h] = ls[0] + "_" + ls[1] + "_" + Bang_arrGiaTri[h][c];
    }
    
    return true;    
}

function BangDuLieu_onKeypress_Delete(h) {
    if (BangDuLieu_DuocSuaChiTiet && h != null) {
        Bang_XoaHang(h);
        var csSoTien = Bang_arrCSMaCot['SoTien'];
        var csCha = Bang_arrCSCha[h];
        var value = parseFloat(Bang_arrGiaTri[csCha][csSoTien]);
        if (Bang_arrHangDaXoa[h])
        {
            value += parseFloat(Bang_arrGiaTri[h][csSoTien]);
        } else
        {
            value -= parseFloat(Bang_arrGiaTri[h][csSoTien]);
        }
        Bang_GanGiaTriThatChoO(csCha, csSoTien, value);
        var mauDuong = '#ffff00';
        var mauBang0 = '#3399FF';
        var mauAm = '#FF0000';
        if (value > 0)
            Bang_GanGiaTriThatChoO_colName(csCha, "sMauSac", mauDuong);
        else if (value < 0)
            Bang_GanGiaTriThatChoO_colName(csCha, "sMauSac", mauAm);
        else
            Bang_GanGiaTriThatChoO_colName(csCha, "sMauSac", mauBang0);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, csSoTien-2);
        }
    }
}

function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    if (h != null && BangDuLieu_DuocSuaChiTiet) {
        if (Bang_arrLaHangCha[hGiaTri] == false) {
            Bang_ThemHang(h, hGiaTri);
            //Gan cac gia tri tien bang 0
            for (var c = Bang_nC_Fixed; c < Bang_nC; c++) {
                if (Bang_arrType[c] == 0 || Bang_arrType[c] == 3) {
                    Bang_GanGiaTriThatChoO(h, c, "");
                }
                else {
                    Bang_GanGiaTriThatChoO(h, c, 0);
                }
            }
            var or = Bang_arrMaHang[hGiaTri].split('_');
            Bang_arrMaHang[h] = "_" + or[1] + "_0";
            Bang_keys.fnSetFocus(h, 0);
        }
    }


}

/* Su kien BangDuLieu_onKeypress_F2
- Muc dinh: goi ham them hang khi an phim F2
*/
function BangDuLieu_onKeypress_F2(h, c) {   
    BangDuLieu_ThemHangMoi(h + 1, h);   
}