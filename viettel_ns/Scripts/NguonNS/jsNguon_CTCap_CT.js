var BangDuLieu_Id_CTCap = "";

var jsNguon_CTCap_Url_Frame = '';
var jsNguon_CTCap_PC_Url = '';
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
/* Su kien BangDuLieu_onKeypress_F2
- Muc dinh: Gọi bảng chi tiết mục chi bộ quốc phòng theo đơn vị, phòng ban ấn phím F2
*/
function BangDuLieu_onKeypress_F2(h, c) {
    if (Bang_arrLaHangCha[h] == false){
        var iID_MaChiTiet = Bang_arrMaHang[h];

        var iID_MaMucLuc = iID_MaChiTiet.split('_')[0];
        url = unescape(jsNguon_CTCap_PC_Url + '?Id_CTCap=' + BangDuLieu_Id_CTCap + '&Id_NguonBTC=' + iID_MaChiTiet);
        window.open(url, '_blank');
    }
}