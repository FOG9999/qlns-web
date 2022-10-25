var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsPBDVEdit_Url_Frame = '';

function BangDuLieu_onKeypress_F2() {

    var yes = confirm("F2: Đ/c chắc chắn muốn phân quyền toàn bộ đơn vị năm hiện tại?");
    if (!yes)
        return;

    CheckAll("1");
}

function BangDuLieu_onKeypress_F3() {

    var yes = confirm("F3: Đ/c chắc chắn muốn bỏ phân quyền toàn bộ đơn vị năm hiện tại?");
    if (!yes)
        return;

    CheckAll("");
}

function CheckAll(cs) {
    for (var i = 0; i < Bang_nH; i++) {
        Bang_GanGiaTriThatChoO_colName(i, "bCon", cs);
    }
}

function jsPBDV_LoadLaiChiTiet() {
    if ($("#tabs-1").css("display") != "none") {
        var url = jsPBDVEdit_Url_Frame;
        var controls = $('input[search-control="1"]');
        var i;
        for (i = 0; i < controls.length; i++) {
            var field = $(controls[i]).attr("search-field");
            var value = $(controls[i]).val();
            url += "&" + field + "=" + encodeURI(value);
        }

        document.getElementById('ifrChiTiet').contentWindow.Bang_GanMangGiaTri_Bang_arrGiaTri();

        jsPBDV_Search_inteval = setInterval(function () {
            jsPBDV_Search_clearInterval();
            document.getElementById("ifrChiTiet").src = url;
        }, 100);

    }
}

var jsPBDV_Search_inteval = null;

function jsPBDV_Search_onkeypress(e) {
    jsPBDV_Search_clearInterval();
    if (e.keyCode == 13) {
        $('input[search-control="1"]').attr('disabled', true);
        setTimeout(function () {
            $('input[search-control="1"]').attr('disabled', false)
        }, 1000);
        jsPBDV_LoadLaiChiTiet();
    }
    else {
        jsPBDV_Search_inteval = setInterval(function () { jsPBDV_Search_clearInterval(); jsPBDV_LoadLaiChiTiet(); }, 2000);
    }
}

function jsPBDV_Search_clearInterval() {
    clearInterval(jsPBDV_Search_inteval);
}