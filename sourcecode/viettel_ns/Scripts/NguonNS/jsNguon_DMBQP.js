var jsDMBQPEdit_Url_Frame = '';

function BangDuLieu_onKeypress_Delete(h, c) {
    if (BangDuLieu_DuocSuaChiTiet && h != null) {
        Bang_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
    }
}

function BangDuLieu_onKeypress_F2(h) {

    var yes = confirm("F2: Đ/c chắc chắn muốn thêm một hàng mới cùng cấp với hàng hiện tại?");

    if (!yes)
        return;

    BangDuLieu_ThemHangMoiCungCap(h + 1, h);
}

function BangDuLieu_ThemHangMoiCungCap(h, hGiaTri) {
    if (h != null && Bang_arrLaHangCha[hGiaTri] == false) {
        Bang_ThemHang(h, hGiaTri);
        Bang_GanGiaTriThatChoO(h, 0, Bang_arrGiaTri[hGiaTri][2] + '-');
        Bang_GanGiaTriThatChoO(h, 1, "");
        Bang_GanGiaTriThatChoO(h, 2, Bang_arrGiaTri[hGiaTri][2]);
        Bang_GanGiaTriThatChoO(h, 3, Bang_arrGiaTri[hGiaTri][3]);
        Bang_GanGiaTriThatChoO(h, 4, "");
        Bang_GanGiaTriThatChoO(h, 5, Bang_arrGiaTri[hGiaTri][5]);
        Bang_arrMaHang[h] = "_";
        Bang_keys.fnSetFocus(h, 0);
    }
}

function BangDuLieu_onKeypress_F3(h) {

    var yes = confirm("F3: Đ/c chắc chắn muốn thêm hàng con cho hàng hiện tại?");
    if (!yes)
        return;
    BangDuLieu_ThemHangMoiCon(h + 1, h);
}

function BangDuLieu_ThemHangMoiCon(h, hGiaTri) {
    if (h != null && Bang_arrLaHangCha[hGiaTri] == false) {
        Bang_ThemHang(h, hGiaTri);
        Bang_GanGiaTriThatChoO(h, 0, Bang_arrGiaTri[hGiaTri][0] + '-');
        Bang_GanGiaTriThatChoO(h, 1, "");
        Bang_GanGiaTriThatChoO(h, 2, Bang_arrGiaTri[hGiaTri][0]);
        Bang_GanGiaTriThatChoO(h, 3, false);
        Bang_GanGiaTriThatChoO(h, 4, "");
        Bang_GanGiaTriThatChoO(h, 5, Bang_arrGiaTri[hGiaTri][4]);
        Bang_arrMaHang[h] = "_";
        Bang_keys.fnSetFocus(h, 0);
    }
}

function jsDMBQP_LoadLaiChiTiet() {
    if ($("#tabs-1").css("display") != "none") {
        var url = jsDMBQPEdit_Url_Frame;

        var controls = $('input[search-control="1"]');
        var i;
        for (i = 0; i < controls.length; i++) {
            var field = $(controls[i]).attr("search-field");
            var value = $(controls[i]).val();
            url += "&" + field + "=" + encodeURI(value);
        }

        document.getElementById('ifrChiTiet').contentWindow.Bang_GanMangGiaTri_Bang_arrGiaTri();

        jsDMBQP_Search_inteval = setInterval(function () {
            jsDMBQP_Search_clearInterval();
            document.getElementById("ifrChiTiet").src = url;
        }, 100);

    }
}

var jsDMBQP_Search_inteval = null;

function jsDMBQP_Search_onkeypress(e) {
    jsDMBQP_Search_clearInterval();
    if (e.keyCode == 13) {
        $('input[search-control="1"]').attr('disabled', true);
        setTimeout(function () {
            $('input[search-control="1"]').attr('disabled', false)
        }, 1000);
        jsDMBQP_LoadLaiChiTiet();
    }
    else {
        jsDMBQP_Search_inteval = setInterval(function () { jsDMBQP_Search_clearInterval(); jsDMBQP_LoadLaiChiTiet(); }, 2000);
    }
}

function jsDMBQP_Search_clearInterval() {
    clearInterval(jsDMBQP_Search_inteval);
}