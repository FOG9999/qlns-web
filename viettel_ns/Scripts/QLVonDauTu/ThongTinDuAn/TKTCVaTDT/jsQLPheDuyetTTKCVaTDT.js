/*Khởi tạo*/
var TONG_DU_TOAN = 1;
var checkTongDuToan = TONG_DU_TOAN;
var CONFIRM = 0;
var ERROR = 1;

/*Index*/
function formatMoney() {
    $('#tblListTKTCTDT tr').each(function () {
        var soTien = $(this).find(".sotien").text();
        if (soTien) {
            soTien = formatNumber(soTien);
            $(this).find(".sotien").text(soTien);
        }
    });
}

function formatNumber(n) {
    // format number 1000000 to 1,234,567
    if (Number(n) >= 0) {
        return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
    }
    return "-" + n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
}

function ChangePage(iCurrentPage = 1) {
    var sTenDuAn = $("#txtDuAn").val();
    var sTenDuToan = $("#txtDuToan").val();
    var sSoQuyetDinh = $("#txtSoQuyetDinh").val();
    var dPheduyetTuNgay = $("#txtTuNgayPheDuyet").val();
    var dPheduyetDenNgay = $("#txtDenNgayPheDuyet").val();
    var sDonViQL = $("#txtDonViQL").val();
    var bIsTongDuToan = checkTongDuToan;
    GetListData(sTenDuAn, sSoQuyetDinh, dPheduyetTuNgay, dPheduyetDenNgay, sDonViQL, bIsTongDuToan, sTenDuToan, iCurrentPage);
}

function GetListData(sTenDuAn, sSoQuyetDinh, dPheduyetTuNgay, dPheduyetDenNgay, sDonViQL, bIsTongDuToan, sTenDuToan, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sTenDuAn: sTenDuAn, sSoQuyetDinh: sSoQuyetDinh, dPheduyetTuNgay: dPheduyetTuNgay, dPheduyetDenNgay: dPheduyetDenNgay, sDonViQL: sDonViQL, bIsTongDuToan: bIsTongDuToan, sTenDuToan: sTenDuToan, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtDuAn").val(sTenDuAn);
            $("#txtDuToan").val(sTenDuToan);
            $("#txtSoQuyetDinh").val(sSoQuyetDinh);
            $("#txtTuNgayPheDuyet").val(dPheduyetTuNgay);
            $("#txtDenNgayPheDuyet").val(dPheduyetDenNgay);
            $("#txtDonViQL").val(sDonViQL);
        }
    });
}

function ChangeTab(isTongDuToan) {
    checkTongDuToan = isTongDuToan;
    ChangePage();
}
/*End Index*/

/*Thêm mới*/
function BtnAdd() {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Update";
}

//function DeleteItem(id) {
//    if (!confirm("Bạn có chắc chắn muốn xóa?")) return;
//    $.ajax({
//        type: "POST",
//        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Xoa",
//        data: { id: id },
//        success: function (r) {
//            if (r == true) {
//                ChangePage();
//            }
//        }
//    });
//}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa khởi tạo thông tin dự án';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Xoa",
        data: { id: id },
        success: function (r) {

            if (r.status == true) {
                alert(r.sMessage);
                ChangePage();
            } else {
                var Title = 'Lỗi xóa phê duyệt TKTC và tổng dự toán';
                var Messages = [];
                Messages.push("Lỗi xóa phê duyệt TKTC và tổng dự toán !");
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data1) {
                        $("#divModalConfirm").html(data1);
                    }
                });
            }
        }
    });
}

function sua(parentId, id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Update/" + id;
}

function DetailItem(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Detail/" + id;
}

function DieuChinh(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Update?id=" + id + "&bIsDieuChinh=true";
}