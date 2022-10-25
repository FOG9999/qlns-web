var jsTimKiemCT_Url_Frame = '';
var jsTimKiemCT_Url = '';
function jsTimKiemCT_LoadLaiChiTiet() {
    if ($("#tabs-1").css("display") != "none") {
        var url = jsTimKiemCT_Url_Frame;
        var controls = $('input[search-control="1"]');
        var i;
        for (i = 0; i < controls.length; i++) {
            var field = $(controls[i]).attr("search-field");
            var value = $(controls[i]).val();
            url += "&" + field + "=" + encodeURI(value);
        }
        jsTimKiemCT_Search_inteval = setInterval(function () {
            jsTimKiemCT_Search_clearInterval();
            document.getElementById("ifrChiTietMLNS").src = url;
        }, 100);

    }
}

var jsTimKiemCT_Search_inteval = null;

function jsTimKiemCT_Search_onkeypress(e) {
    jsTimKiemCT_Search_clearInterval();
    if (e.keyCode == 13) {
        $('input[search-control="1"]').attr('disabled', true);
        setTimeout(function () {
            $('input[search-control="1"]').attr('disabled', false)
        }, 1000);
        jsTimKiemCT_LoadLaiChiTiet();
    }
    else {
        jsTimKiemCT_Search_inteval = setInterval(function () { jsTimKiemCT_Search_clearInterval(); jsTimKiemCT_LoadLaiChiTiet(); }, 2000);
    }
}

function jsTimKiemCT_Search_clearInterval() {
    clearInterval(jsTimKiemCT_Search_inteval);
}

function reloadPage() {
    window.location.href = jsTimKiemCT_Url;
}