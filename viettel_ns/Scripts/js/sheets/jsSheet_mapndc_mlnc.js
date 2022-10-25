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

function BangDuLieu_onKeypress_F9() {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".sheet-search button[type=submit]").click()
}

function Bang_onKeypress_F(strKeyEvent) {
    var h = Bang_keys.Row();
    console.log(h);
    var c = Bang_keys.Col() - Bang_nC_Fixed;
    var TenHam = Bang_ID + '_onKeypress_' + strKeyEvent;
    var fn = window[TenHam];
    if (typeof fn == 'function') {
        if (fn(h, c) == false) {
            return false;
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

//function BangDuLieu_onKeypress_F2(h, c) {
//    var iID_MaChiTiet = Bang_arrMaHang[h];

//    var iID_MaMucLuc = iID_MaChiTiet.split('_')[0];
//    url = unescape(BangDuLieu_Url_Mapping + '?id=' + iID_MaMucLuc + '&loai=1');
//    window.open(url, '_blank');
//}

//function BangDuLieu_onKeypress_F3(h, c) {
//    var iID_MaChiTiet = Bang_arrMaHang[h];

//    var iID_MaMucLuc = iID_MaChiTiet.split('_')[0];
//    url = unescape(BangDuLieu_Url_Mapping + '?id=' + iID_MaMucLuc + '&loai=2');
//    window.open(url, '_blank');
//}

var parentWindow = window.parent;
function BangDuLieu_onDblClick(h, c) {
    var isParent = Bang_arrLaHangCha[h];
    if (isParent != undefined && !isParent) {
        var iID_NoiDungChi = Bang_arrGiaTri[h][4];
        var sTenNoiDungChi = Bang_arrGiaTri[h][1];
        var sMaNoiDungChi = Bang_arrGiaTri[h][5];

        parentWindow.btnViewMappingNdcChiTiet(iID_NoiDungChi, sMaNoiDungChi, sTenNoiDungChi);
    }
    return true;
}