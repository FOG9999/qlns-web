var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
function ResetChangePage(iCurrentPage = 1) {
    GetListData("");
}
function ChangePage() {
    var iNamKeHoach = $("#slbNamKeHoachFillter").val();
    if (iNamKeHoach == "") {
        PopupError();
        return false;
    }
    GetListData(iNamKeHoach);
}
function GetListData(iNamKeHoach) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { iNamKeHoach: iNamKeHoach },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#slbNamKeHoachFillter").val(iNamKeHoach);
        }
    });
}

function ExportQTNguonChiDacBiet(fileType) {
    var iNamKeHoach = $("#slbNamKeHoachFillter").val();
    if (iNamKeHoach == "") {
        PopupError();
        return false;
    }
    var url = "/QLNH/BaoCaoQuyetToanNguonChiDacBiet/ExportQuyetToanNguonChiDacBiet?iNamKeHoach=" + iNamKeHoach + "&ext=" + fileType;
    var arrLink = [];
    arrLink.push(url);
    openLinks(arrLink);
}

function PopupError() {
    var Title = 'Thông báo';
    var messErr = ["Vui lòng chọn năm!"];
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: messErr, Category: ERROR },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}