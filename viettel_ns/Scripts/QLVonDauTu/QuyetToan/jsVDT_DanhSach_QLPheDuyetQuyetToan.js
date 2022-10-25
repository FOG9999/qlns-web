var trLoaiCongTrinh;
var typeSearch = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

$(document).ready(function ($) {
    FormatMoneyItem();
});

/*NinhNV start*/

function SearchData(iCurrentPage = 1) {
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var sTenDuAn = $("#sTenDuAn").val();
    var fTienQuyetToanPheDuyetFrom = UnFormatNumber($("#fTienQuyetToanPheDuyetFrom").val());
    var fTienQuyetToanPheDuyetTo = UnFormatNumber($("#fTienQuyetToanPheDuyetTo").val());
    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sTenDuAn, fTienQuyetToanPheDuyetFrom, fTienQuyetToanPheDuyetTo, iCurrentPage);
}

function ResetSearchData(iCurrentPage = 1) {
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var sTenDuAn = "";
    var fTienQuyetToanPheDuyetFrom = "";
    var fTienQuyetToanPheDuyetTo = "";
    $("#sSoQuyetDinh").val();
    $("#dNgayQuyetDinhFrom").val();
    $("#dNgayQuyetDinhTo").val();
    $("#sTenDuAn").val(); 
    UnFormatNumber($("#fTienQuyetToanPheDuyetFrom").val());
    UnFormatNumber($("#fTienQuyetToanPheDuyetTo").val());
    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sTenDuAn, fTienQuyetToanPheDuyetFrom, fTienQuyetToanPheDuyetTo, iCurrentPage);
}

function ChangePage(iCurrentPage) {
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var sTenDuAn = $("#sTenDuAn").val();
    var fTienQuyetToanPheDuyetFrom = UnFormatNumber($("#fTienQuyetToanPheDuyetFrom").val());
    var fTienQuyetToanPheDuyetTo = UnFormatNumber($("#fTienQuyetToanPheDuyetTo").val());
    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sTenDuAn, fTienQuyetToanPheDuyetFrom, fTienQuyetToanPheDuyetTo, iCurrentPage);
}

function GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sTenDuAn, fTienQuyetToanPheDuyetFrom, fTienQuyetToanPheDuyetTo, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, sTenDuAn: sTenDuAn, fTienQuyetToanPheDuyetFrom: fTienQuyetToanPheDuyetFrom, fTienQuyetToanPheDuyetTo: fTienQuyetToanPheDuyetTo, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#sSoQuyetDinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#sTenDuAn").val(sTenDuAn);
            $("#fTienQuyetToanPheDuyetFrom").val(FormatNumber(fTienQuyetToanPheDuyetFrom));
            $("#fTienQuyetToanPheDuyetTo").val(FormatNumber(fTienQuyetToanPheDuyetTo));
        }
    });
}

function BtnCreateDataClick() {
    window.location.href = "/QLVonDauTu/QLPheDuyetQuyetToan/CreateNew/";
}

function GetItemData(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetQuyetToan/CreateNew/" + id;
}

function ViewItemDetail(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetQuyetToan/ViewDetailQTDA/" + id;
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa phê duyệt quyết toán dự án';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "DeleteItemQTDA('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });

    //$.ajax({
    //    type: "POST",
    //    url: "/QLDuAn/checkDeleteDuAn",
    //    dataType: "json",
    //    data: { id: id },
    //    success: function (r) {
    //        if (r.bIsComplete) {
    //            var Title = 'Xác nhận xóa phê duyệt quyết toán dự án';
    //            var Messages = [];
    //            Messages.push('Bạn có chắc chắn muốn xóa?');
    //            var FunctionName = "DeleteItemDA('" + id + "')";
    //            $.ajax({
    //                type: "POST",
    //                url: "/Modal/OpenModal",
    //                data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
    //                success: function (data) {
    //                    $("#divModalConfirm").html(data);
    //                }
    //            });
    //        } else {
    //            var Title = 'Lỗi khi xóa phê duyệt quyết toán dự án';
    //            var messErr = [];
    //            messErr.push(r.errMes);
    //            $.ajax({
    //                type: "POST",
    //                url: "/Modal/OpenModal",
    //                data: { Title: Title, Messages: messErr, Category: ERROR },
    //                success: function (data) {
    //                    $("#divModalConfirm").html(data);
    //                }
    //            });
    //            return;
    //        }
    //    }
    //});  
}

function DeleteItemQTDA(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/VDTQTDADelete",
        data: { id: id },
        success: function (r) {
            if (r) {
                SearchData();
            }
        }
    });
}

function OpenExport(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/ExportQuyetToan",
        data: { iID_QuyetToanID: id },
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/QLPheDuyetQuyetToan/ExportExcelQuyetToan";
            }
        }
    });
}

/*NinhNV end*/

/* Xem chi tiết dự án */
function TinhLaiDongTong(idBang) {
    var tongGiaTriToTrinh = 0;
    var tongGiaTriThamDinh = 0;
    var tongGiaTriPheDuyet = 0;

    $("#" + idBang + " .r_GiaTriToTrinh").each(function () {
        tongGiaTriToTrinh += parseInt(UnFormatNumber($(this).html()));
        $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    });

    $("#" + idBang + " .r_GiaTriThamDinh").each(function () {
        tongGiaTriThamDinh += parseInt(UnFormatNumber($(this).html()));
        $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    });

    $("#" + idBang + " .r_GiaTriPheDuyet").each(function () {
        tongGiaTriPheDuyet += parseInt(UnFormatNumber($(this).html()));
        $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    });

    $("#" + idBang + " .cpdt_tong_giatritotrinh").html(FormatNumber(tongGiaTriToTrinh));
    $("#" + idBang + " .cpdt_tong_gaitrithamdinh").html(FormatNumber(tongGiaTriThamDinh));
    $("#" + idBang + " .cpdt_tong_giatripheduyet").html(FormatNumber(tongGiaTriPheDuyet));

}

function FormatMoneyItem() {
    $(".main .clsMoney").each(function () {
        $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    });
}