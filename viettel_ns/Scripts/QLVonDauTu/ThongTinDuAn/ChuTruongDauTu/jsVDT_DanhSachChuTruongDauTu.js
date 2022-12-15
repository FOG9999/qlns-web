function ChangePage(iCurrentPage = 1) {
    var sSoQuyetDinh = $.trim($("#txtSoQuyetDinh").val());
    var sNoiDung = $.trim($("#txtNoiDung").val());
    var fTongMucDauTuFrom = $("#txtTongMucDauTuFrom").val() == "" ? 0 : parseFloat(UnFormatNumber($("#txtTongMucDauTuFrom").val()));
    var fTongMucDauTuTo = $("#txtTongMucDauTuTo").val() == "" ? 0 : parseFloat(UnFormatNumber($("#txtTongMucDauTuTo").val()));
    var dNgayQuyetDinhFrom = $("#txtNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#txtNgayQuyetDinhTo").val();
    var sMaDonVi = $("#txtDonViQuanLy").val();
    GetListData(sSoQuyetDinh, sNoiDung, fTongMucDauTuFrom, fTongMucDauTuTo, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi, iCurrentPage);
}

function ResetChangePage(iCurrentPage = 1) {
    var soQuyetDinh = "";
    var sNoiDung = "";
    var fTongMucDauTuFrom = "";
    var fTongMucDauTuTo = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var sMaDonVi = "";

    $("#txtSoQuyetDinh").val("");
    $("#txtNoiDung").val("");
    $("#txtTongMucDauTuFrom").val("");
    $("#txtTongMucDauTuTo").val("");
    $("#txtNgayQuyetDinhFrom").val("");
    $("#txtNgayQuyetDinhTo").val("");
    $("#txtDonViQuanLy").val("");

    GetListData(soQuyetDinh, sNoiDung, fTongMucDauTuFrom, fTongMucDauTuTo, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi, iCurrentPage);
}

function GetListData(sSoQuyetDinh, sNoiDung, fTongMucDauTuFrom, fTongMucDauTuTo, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: {
            sSoQuyetDinh: sSoQuyetDinh, sNoiDung: sNoiDung, fTongMucDauTuFrom: fTongMucDauTuFrom, fTongMucDauTuTo: fTongMucDauTuTo, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom,
            dNgayQuyetDinhTo: dNgayQuyetDinhTo, sMaDonVi: sMaDonVi, _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoQuyetDinh").val(sSoQuyetDinh);
            $("#txtNoiDung").val(sNoiDung);
            $("#txtTongMucDauTuFrom").val(fTongMucDauTuFrom == 0 ? "" : FormatNumber(fTongMucDauTuFrom));
            $("#txtTongMucDauTuTo").val(fTongMucDauTuTo == 0 ? "" : FormatNumber(fTongMucDauTuTo));
            $("#txtNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#txtNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#txtDonViQuanLy").val(sMaDonVi);
        }
    });
}

function formatMoney() {
    $('#tblListVDTChuTruongDauTu tr').each(function () {
        var soTien = $(this).find(".sotien").text();
        if (soTien) {
            soTien = FormatNumber(soTien);
            $(this).find(".sotien").text(soTien);
        }
    });
}

function themMoi() {
    window.location.href = "/QLVonDauTu/ChuTruongDauTu/TaoMoi/";
}

function xemChiTiet(id) {
    window.location.href = "/QLVonDauTu/ChuTruongDauTu/ChiTiet/" + id;
}

function sua(parentId,id) {
    //window.location.href = "/QLVonDauTu/ChuTruongDauTu/Sua/" + id;

    if (parentId != "" && parentId != undefined) {
        window.location.href = "/QLVonDauTu/ChuTruongDauTu/SuaDieuChinh/" + id;
    } else {
        window.location.href = "/QLVonDauTu/ChuTruongDauTu/Sua/" + id;
    }
}


function xoa(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/ChuTruongDauTu/Xoa",
        data: { id: id },
        success: function (r) {
            if (r.status) {
                alert(r.sMessage);
                ChangePage();
            } else {
                alert(r.sMessage);
            }
        }
    });
}

function dieuChinh(id) {
    window.location.href = "/QLVonDauTu/ChuTruongDauTu/DieuChinh/" + id;
}

//export
function ExportChuTruongDauTu() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/ChuTruongDauTu/ChuTruongDauTuExport",
        success: function (data) {
            if (data.bIsComplete) {
                window.location.href = "/QLVonDauTu/ChuTruongDauTu/ExportExcelChuTruongDauTu";
            }
        }
    });
}