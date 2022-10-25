var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;

document.addEventListener('keydown', eventKey);
function eventKey(e) {
    if (e != null && e != undefined) {
        if (e.key == "F9") {
            BangDuLieu_onKeypress_F9();
        }
    }
}

function BangDuLieu_onKeypress_F9(h, c) {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".sheet-search .btn-info").click();
}

function BangDuLieu_onKeypress_F2() {
    var yes = confirm("F2: Đ/c chắc chắn muốn chọn toàn bộ mục lục ngân sách hiện tại hiện tại?");
    if (!yes)
        return;

    CheckAll(1);
}

function BangDuLieu_onKeypress_F3() {
    var yes = confirm("F3: Đ/c chắc chắn muốn bỏ chọn toàn bộ mục lục ngân sách hiện tại?");
    if (!yes)
        return;

    CheckAll(0);
}

function CheckAll(cs) {
    for (var i = 0; i < Bang_nH; i++) {
        if (!Bang_arrLaHangCha[i]) {
            Bang_GanGiaTriThatChoO_colName(i, "isMap", cs);
        }
    }
}

function BangDuLieu_HamTruocKhiKetThuc(iAction) {
    var html = PopupModal("Thông báo", "Lưu dữ liệu thành công");
    window.parent.loadModal(html);
    return true;
}

function PopupModal(title, message) {
    var result = "";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: 1 },
        async: false,
        success: function (data) {
            result = data;
        }
    });

    return result;
}

function BangDuLieu_onCellAfterEdit(h, c) {
    var checkbox_value = Bang_arrGiaTri[h][c];
    var iID_MaMucLucNganSach = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_MaMucLucNganSach"]];
    for (var i = 0; i < Bang_nH; i++) {

        if (Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_MaMucLucNganSach_Cha"]] == iID_MaMucLucNganSach) {
            if (Bang_arrLaHangCha[h] == true) {
                Bang_GanGiaTriThatChoO_colName(i, "isMap", checkbox_value);
                BangDuLieu_onCellAfterEdit(i, Bang_arrCSMaCot["isMap"]);
            } else {
                Bang_GanGiaTriThatChoO_colName(i, "isMap", checkbox_value);
            }
        }
    }
}