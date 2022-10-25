var jsBTCBQP_Index_Url_Frame = '';

function jsBTCBQP_Index_LoadLaiChiTiet() {
    if ($("#tabs-1").css("display") != "none") {
        var url = jsBTCBQP_Index_Url_Frame;
        var controls = $('input[search-control="1"]');
        var i;
        for (i = 0; i < controls.length; i++) {
            var field = $(controls[i]).attr("search-field");
            var value = $(controls[i]).val();
            url += "&" + field + "=" + encodeURI(value);
        }

        document.getElementById('ifrChiTiet').contentWindow.Bang_GanMangGiaTri_Bang_arrGiaTri();

        jsBTCBQP_Index_Search_inteval = setInterval(function () {
            jsBTCBQP_Index_Search_clearInterval();
            document.getElementById("ifrChiTiet").src = url;
        }, 100);

    }
}

var jsBTCBQP_Index_Search_inteval = null;

function jsBTCBQP_Index_Search_onkeypress(e) {
    jsBTCBQP_Index_Search_clearInterval();
    if (e.keyCode == 13) {
        $('input[search-control="1"]').attr('disabled', true);
        setTimeout(function () {
            $('input[search-control="1"]').attr('disabled', false)
        }, 1000);
        jsBTCBQP_Index_LoadLaiChiTiet();
    }
    else {
        jsBTCBQP_Index_Search_inteval = setInterval(function () { jsBTCBQP_Index_Search_clearInterval(); jsBTCBQP_Index_LoadLaiChiTiet(); }, 2000);
    }
}

function jsBTCBQP_Index_Search_clearInterval() {
    clearInterval(jsBTCBQP_Index_Search_inteval);
}

function BangDuLieu_onKeypress_F2(h, c) {
    var Id_MaChiTiet = Bang_arrMaHang[h];

    var Id_Ma = Id_MaChiTiet.split('_')[0];
    url = unescape(jsNguon_BTCBQP_1_Url_Frame + '&Id_MaNguon=' + Id_Ma);
    window.open(url, '_blank');
}

function BangDuLieu_onKeypress_F3(h, c) {
    var Id_MaChiTiet = Bang_arrMaHang[h];

    var Id_Ma = Id_MaChiTiet.split('_')[0];
    url = unescape(jsNguon_BTCBQP_2_Url_Frame + '&Id_MaNguon=' + Id_Ma);
    window.open(url, '_blank');
}