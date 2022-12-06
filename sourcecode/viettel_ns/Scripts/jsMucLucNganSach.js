var jsMLNS_Url_Frame = '';
var jsMLNS_Url = '';
function jsMLNS_LoadLaiChiTiet() {
    if ($("#tabs-1").css("display") != "none") {
        var url = jsMLNS_Url_Frame;
        var controls = $('input[search-control="1"]');
        var i;
        for (i = 0; i < controls.length; i++) {
            var field = $(controls[i]).attr("search-field");
            var value = $(controls[i]).val();
            url += "&" + field + "=" + encodeURI(value);
        }
        jsMLNS_Search_inteval = setInterval(function () {
            jsMLNS_Search_clearInterval();
            document.getElementById("ifrChiTietMLNS").src = url;
        }, 100);

    }
}

var jsMLNS_Search_inteval = null;

function jsMLNS_Search_onkeypress(e) {
    jsMLNS_Search_clearInterval();
    if (e.keyCode == 13) {
        $('input[search-control="1"]').attr('disabled', true);
        setTimeout(function () {
            $('input[search-control="1"]').attr('disabled', false)
        }, 1000);
        jsMLNS_LoadLaiChiTiet();
    }
    else {
        jsMLNS_Search_inteval = setInterval(function () { jsMLNS_Search_clearInterval(); jsMLNS_LoadLaiChiTiet(); }, 2000);
    }
}

function jsMLNS_Search_clearInterval() {
    clearInterval(jsMLNS_Search_inteval);
}

function reloadPage() {
    window.location.href = jsMLNS_Url;
}