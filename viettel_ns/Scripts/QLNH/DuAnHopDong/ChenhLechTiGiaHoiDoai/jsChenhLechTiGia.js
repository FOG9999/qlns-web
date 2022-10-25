var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

$(document).ready(function () {
    $("#slbDonVi").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbChuongTrinh").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbHopDong").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
});

function ResetChangePage(iCurrentPage = 1) {
    GetListData(GUID_EMPTY, "", GUID_EMPTY, GUID_EMPTY, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var iDonVi = $("#slbDonVi").val();
    var maDonVi = $("<div/>").text($.trim($("#slbDonVi").find("option:selected").data("madonvi"))).html();
    var iChuongTrinh = $("#slbChuongTrinh").val();
    var iHopDong = $("#slbHopDong").val();
    GetListData(iDonVi, maDonVi, iChuongTrinh, iHopDong, iCurrentPage);
}

function GetListData(iDonVi, maDonVi, iChuongTrinh, iHopDong, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { iDonVi: iDonVi, maDonVi: maDonVi, iChuongTrinh: iChuongTrinh, iHopDong: iHopDong, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#slbDonVi").val(iDonVi);
            $("#slbChuongTrinh").val(iChuongTrinh);
            $("#slbHopDong").val(iHopDong);
        }
    });
}

function ChangeSelectDonVi() {
    var iDonVi = $("#slbDonVi").val();
    var maDonVi = $("#slbDonVi").find("option:selected").data("madonvi");
    $.ajax({
        type: "POST",
        url: "/QLNH/ChenhLechTiGiaHoiDoai/ChangeSelectDonVi",
        data: { iDonVi: iDonVi, maDonVi: maDonVi },
        success: function (data) {
            if (data) {
                $("#slbChuongTrinh").empty().html(data.htmlCT);
                $("#slbHopDong").empty().html(data.htmlHD);
            }
        }
    });
}

function ChangeSelectChuongTrinh() {
    var iChuongTrinh = $("#slbChuongTrinh").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ChenhLechTiGiaHoiDoai/ChangeSelectChuongTrinh",
        data: { iChuongTrinh: iChuongTrinh },
        success: function (data) {
            if (data) {
                $("#slbHopDong").empty().html(data);
            }
        }
    });
}

function ExportChenhLechTiGia(fileType) {
    var iDonVi = $("#slbDonVi").val();
    var iChuongTrinh = $("#slbChuongTrinh").val();
    var iHopDong = $("#slbHopDong").val();
    var url = "/QLNH/ChenhLechTiGiaHoiDoai/ExportChenhLechTiGia?iDonVi=" + iDonVi
        + "&iChuongTrinh=" + iChuongTrinh + "&iHopDong=" + iHopDong + "&ext=" + fileType;
    var arrLink = [];
    arrLink.push(url);
    openLinks(arrLink);
}