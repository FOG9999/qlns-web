var trLoaiCongTrinh;
var typeSearch = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ChangePage(iCurrentPage = 1) {
    var sSoQuyetDinh = $("#txtSoQuyetDinh").val();
    var sTenDuAn = $("#txtTenDuAn").val();
    var dNgayQuyetDinhFrom = $("#txtNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#txtNgayQuyetDinhTo").val();
    var sDonViQuanLy = $("#drpDonViQuanLy option:selected").val();
    GetListData(sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sDonViQuanLy, iCurrentPage);
}

function GetListData(sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sDonViQuanLy, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KHLuaChonNhaThau/KHLuaChonNhaThauView",
        data: { sSoQuyetDinh: sSoQuyetDinh, sTenDuAn: sTenDuAn, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, sDonViQuanLy: sDonViQuanLy, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            setSearchCondition(sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sDonViQuanLy);
        }
    });
}

function setSearchCondition(sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sDonViQuanLy) {
    $("#txtSoQuyetDinh").val(sSoQuyetDinh);
    $("#txtTenDuAn").val(sTenDuAn);
    $("#txtNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
    $("#txtNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
    $("#drpDonViQuanLy").val(sDonViQuanLy);
}

function Update(id = '', bIsDieuChinh) {
    if (bIsDieuChinh) {
        window.location.href = "/QLVonDauTu/KHLuaChonNhaThau/Update?id=" + id + "&bIsDieuChinh=" + bIsDieuChinh;

    } else {
        window.location.href = "/QLVonDauTu/KHLuaChonNhaThau/Update/" + id;
    }
}

function ViewItemDetail(id) {
    window.location.href = "/QLVonDauTu/KHLuaChonNhaThau/Detail/" + id;
}

//function DeleteItem(id) {
//    if (confirm("Bạn có chắc chắn muốn xóa?")) {
//        $.ajax({
//            type: "POST",
//            url: "/KHLuaChonNhaThau/DeleteItem",
//            dataType: "json",
//            data: { id: id },
//            success: function (r) {
//                if (r.bIsComplete) {
//                    window.location.href = "/QLVonDauTu/KHLuaChonNhaThau";
//                }
//                else {
//                    alert("Xóa kế hoạch lựa chọn nhà thầu thất bại.");
//                    return false;
//                }
//            }
//        });  
//    }
//}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa kế hoạch lựa chọn nhà thầu';
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
        url: "/KHLuaChonNhaThau/DeleteItem",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete == true) {
                window.location.href = "/QLVonDauTu/KHLuaChonNhaThau";
            }
            else {
                var Title = 'Lỗi xóa kế hoạch lựa chọn nhà thầu';
                var Messages = [];
                Messages.push("Lỗi xóa kế hoạch lựa chọn nhà thầu!");
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

function dieuChinh(id) {
    window.location.href = "/QLVonDauTu/KHLuaChonNhaThau/DieuChinh/" + id;
}