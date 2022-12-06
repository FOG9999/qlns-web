var jsNDDV_Url_Frame = '';

/* Su kien BangDuLieu_onKeypress_F2
- Muc dinh: Gọi bảng phân quyền chi tiết người dùng đơn vị ấn phím F2
*/
function BangDuLieu_onKeypress_F2(h, c) {
    var iID_MaChiTiet = Bang_arrMaHang[h];

    var iID_MaMucLuc = iID_MaChiTiet.split('_')[0];
    url = unescape(jsNDDV_Url + '?maND=' + iID_MaMucLuc);
    window.open(url, '_blank');
}

function jsNDDV_LoadLaiChiTiet() {
    if ($("#tabs-1").css("display") != "none") {
        var url = jsNDDV_Url_Frame;
        var controls = $('input[search-control="1"]');
        var i;
        for (i = 0; i < controls.length; i++) {
            var field = $(controls[i]).attr("search-field");
            var value = $(controls[i]).val();
            url += "&" + field + "=" + encodeURI(value);
        }
       
        document.getElementById('ifrChiTiet').contentWindow.Bang_GanMangGiaTri_Bang_arrGiaTri();

        jsNDDV_Search_inteval = setInterval(function () {
            jsNDDV_Search_clearInterval();
            document.getElementById("ifrChiTiet").src = url;
        }, 100);

    }
}

var jsNDDV_Search_inteval = null;

function jsNDDV_Search_onkeypress(e) {
    jsNDDV_Search_clearInterval();
    if (e.keyCode == 13) {
        $('input[search-control="1"]').attr('disabled', true);
        setTimeout(function () {
            $('input[search-control="1"]').attr('disabled', false)
        }, 1000);
        jsNDDV_LoadLaiChiTiet();
    }
    else {
        jsNDDV_Search_inteval = setInterval(function () { jsNDDV_Search_clearInterval(); jsNDDV_LoadLaiChiTiet(); }, 2000);
    }
}

function jsNDDV_Search_clearInterval() {
    clearInterval(jsNDDV_Search_inteval);
}