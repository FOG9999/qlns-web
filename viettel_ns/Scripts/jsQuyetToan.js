var jsQuyetToan_Search_inteval = null;
var jsQuyetToan_Url_Frame = "";
var check = true;
function jsQuyetToan_LoadLaiChiTiet() {
    if ($("#tabs-1").css("display") != "none") {
        //check = false;
        var url = jsQuyetToan_Url_Frame;
        var controls = $('input[search-control="1"]');
        var i;
        for (i = 0; i < controls.length; i++) {
            var field = $(controls[i]).attr("search-field");
            var value = $(controls[i]).val();
            url += "&" + field + "=" + encodeURI(value);
        }
        //ShowPopupThucHien();
        //document.getElementById("formDuyet").submit();
        //gan gia tri
        document.getElementById('ifrChiTietChungTu').contentWindow.Bang_GanMangGiaTri_Bang_arrGiaTri();
        //goi submit
        //$("#ifrChiTietChungTu").contents().find("#formDuyet").submit();
        //reload lai trang
        jsQuyetToan_Search_inteval = setInterval(function () {
            jsQuyetToan_Search_clearInterval();
            document.getElementById("ifrChiTietChungTu").src = url;
            //check = true;

        }, 100);
    }
}
function ShowPopupThucHien() {
    $("#ifrChiTietChungTu").contents().find("#dvText").show();
    $("#ifrChiTietChungTu").contents().find("body").append('<div id="fade"></div>'); //Add the fade layer to bottom of the body tag.
    $("#ifrChiTietChungTu").contents().find("#fade").css({ 'filter': 'alpha(opacity=40)' }).fadeIn(); //Fade in the fade layer 
}
var jsQuyetToan_Search_inteval = null;
function jsQuyetToan_Search_onkeypress(e) {
    jsQuyetToan_Search_clearInterval();
    if (check == true) {
        if (e.keyCode == 13) {

            $('input[search-control="1"]').attr('disabled', true);
            setTimeout(function () {
                $('input[search-control="1"]').attr('disabled', false)
            }, 5000);
            jsQuyetToan_LoadLaiChiTiet();

        }
        else {
          //  jsQuyetToan_Search_inteval = setInterval(function () { jsQuyetToan_Search_clearInterval(); jsQuyetToan_LoadLaiChiTiet(); }, 2000);
        }
    }
}
function jsQuyetToan_Search_clearInterval() {
    clearInterval(jsQuyetToan_Search_inteval);
}