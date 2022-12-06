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
    if (!ValidateBaoCao()) {
        return false;
    }
    var iDonVi = $("#slbDonVi").val();
    var iChuongTrinh = $("#slbChuongTrinh").val();
    var iHopDong = $("#slbHopDong").val();
    var url = "/QLNH/ChenhLechTiGiaHoiDoai/ExportChenhLechTiGia?iDonVi=" + iDonVi
        + "&iChuongTrinh=" + iChuongTrinh
        + "&iHopDong=" + iHopDong
        + "&tieude1=" + encodeURIComponent($.trim($("#txtTieuDe1").val()))
        + "&tieude2=" + encodeURIComponent($.trim($("#txtTieuDe2").val()))
        + "&tieude3=" + encodeURIComponent($.trim($("#txtTieuDe3").val()))
        + "&tendonvicaptren=" + encodeURIComponent($.trim($("#txtDonViCapTren").val()))
        + "&tendonvi=" + encodeURIComponent($.trim($("#txtDonVi").val()))
        + "&ext=" + fileType;
    var arrLink = [];
    arrLink.push(url);
    openLinks(arrLink);
}

function OpenModalBaoCao() {
    $.ajax({
        type: "POST",
        url: "/QLNH/ChenhLechTiGiaHoiDoai/OpenModalBaoCao",
        success: function (data) {
            $('#modalChenhLechTiGia').modal('toggle');
            $('#modalChenhLechTiGia').modal('show');
            $("#modalChenhLechTiGia").html(data);
        }
    });
}

function ValidateBaoCao() {
    var Title = 'Lỗi hạng mục chưa nhập trên báo cáo';
    var Messages = [];

    if ($.trim($("#txtDonViCapTren").val()) == "") {
        Messages.push("Đơn vị cấp trên chưa được nhập!");
    }

    if ($.trim($("#txtDonVi").val()) == "") {
        Messages.push("Đơn vị chưa được nhập!");
    }

    if (Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}