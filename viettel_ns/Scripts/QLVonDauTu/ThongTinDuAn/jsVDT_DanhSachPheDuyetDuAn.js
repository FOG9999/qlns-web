var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ChangePage(iCurrentPage = 1) {
    var sSoQuyetDinh = $("#txtSoQuyetDinh").val();
    var sTenDuAn = $("#txtDuAn").val();
    var dNgayQuyetDinhFrom = $("#txtNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#txtNgayQuyetDinhTo").val();
    var sMaDonVi = $("#txtDonViQuanLy").val();
    GetListData(sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi, iCurrentPage);
}

function ResetChangePage(iCurrentPage = 1) {
    var soQuyetDinh = "";
    var tenDuAn = "";
    var ngayQuyetDinhTu = "";
    var ngayQuyetDinhDen = "";
    var maDonVi = "";

    $("#txtSoQuyetDinh").val("");
    $("#txtDuAn").val("");
    $("#txtNgayQuyetDinhFrom").val("");
    $("#txtNgayQuyetDinhTo").val("");
    $("#txtDonViQuanLy").val("");

    GetListData(soQuyetDinh, tenDuAn, ngayQuyetDinhTu, ngayQuyetDinhDen, maDonVi, iCurrentPage);
}

function GetListData(sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: {
            sSoQuyetDinh: sSoQuyetDinh, sTenDuAn: sTenDuAn, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, sMaDonVi: sMaDonVi, _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoQuyetDinh").val(sSoQuyetDinh);
            $("#txtDuAn").val(sTenDuAn);
            $("#txtNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#txtNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#txtDonViQuanLy").val(sMaDonVi);
        }
    });
}

function formatMoney() {
    $('#tblListVDTPheDuyetDuAn tr').each(function () {
        var soTien = $(this).find(".sotien").text();
        if (soTien) {
            soTien = FormatNumber(soTien);
            $(this).find(".sotien").text(soTien);
        }
    });
}

function themMoi() {
    window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/TaoMoi/";
}

function xemChiTiet(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/ChiTiet/" + id;
}

function sua(parentId, id) {

    if (parentId != "" && parentId != undefined) {
        window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/SuaDieuChinh/" + id;
    } else {
        window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/Sua/" + id;
    }
}

function xoa(id) {
    var title = 'Xác nhận xóa phê duyệt dự án';
    var message = [];
    message.push('Bạn có chắc chắn muốn xóa?');
    var func = "Delete('" + id + "')";

    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: CONFIRM, FunctionName: func },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function PopupModal(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
            $('.modal-backdrop').remove();
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetDuAn/Xoa",
        data: { id: id },
        success: function (r) {
            if (r == true) {
                ChangePage();
            }
            else {
                PopupModal("Lỗi xóa PDDA", ["Bản ghi được sử dụng trong bảng thiết kế thi công và tổng dự toán. Bạn không thể thực hiện thao tác này."], ERROR);
            }
        }
    });
}

function dieuChinh(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/DieuChinh/" + id;
}