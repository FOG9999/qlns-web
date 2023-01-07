var trLoaiCongTrinh;
var typeSearch = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

$(document).ready(function ($) {
    FormatMoneyItem();

    //$("#iID_CapPheDuyetID").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
   // $("#iID_DonViQuanLyID").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    $("#tThoiGianThucHienTuNam").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });

    $("#tThoiGianThucHienDenNam").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });

    $("#txtTenDuAn").keyup(function (event) {
        ValidateMaxLength(this, 2000);
    });
});

/*NinhNV start*/

function SearchData(iCurrentPage = 1) {
    var sTenDuAn = $.trim($("#txtTenDuAn").val());
    var sKhoiCong = $("#tThoiGianThucHienTuNam").val();
    var sKetThuc = $("#tThoiGianThucHienDenNam").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var iID_CapPheDuyetID = $("#iID_CapPheDuyetID").val();
    var iID_LoaiCongTrinhID = $("#hid_iID_LoaiCongTrinhID").val();
    GetListData(sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID, typeSearch, iCurrentPage);
}

function ResetSearchData(iCurrentPage = 1) {
    var sTenDuAn = "";
    var sKhoiCong = "";
    var sKetThuc = "";
    var iID_DonViQuanLyID = "";
    var iID_CapPheDuyetID = "";
    var iID_LoaiCongTrinhID = "";
    $("#txtTenDuAn").val("");
    $("#tThoiGianThucHienTuNam").val("");
    $("#tThoiGianThucHienDenNam").val("");
    $("#iID_DonViQuanLyID").val("");
    $("#iID_CapPheDuyetID").val("");
    $("#hid_iID_LoaiCongTrinhID").val("");
    GetListData(sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID, typeSearch, iCurrentPage);
}

function TabSearchData(type, iCurrentPage = 1) {
    typeSearch = type;
    var sTenDuAn = "";
    var sKhoiCong = "";
    var sKetThuc = "";
    var iID_DonViQuanLyID = "";
    var iID_CapPheDuyetID = "";
    var iID_LoaiCongTrinhID = "";

    GetListData(sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID, typeSearch, iCurrentPage);
}

function ChangePage(iCurrentPage) {
    var sTenDuAn = $("#txtTenDuAn").val();
    var sKhoiCong = $("#tThoiGianThucHienTuNam").val();
    var sKetThuc = $("#tThoiGianThucHienDenNam").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var iID_CapPheDuyetID = $("#iID_CapPheDuyetID").val();
    var iID_LoaiCongTrinhID = $("#iID_LoaiCongTrinhID").val();
    GetListData(sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID, typeSearch, iCurrentPage);
}

function GetListData(sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID, iTrangThai, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sTenDuAn: sTenDuAn, sKhoiCong: sKhoiCong, sKetThuc: sKetThuc, iID_DonViQuanLyID: iID_DonViQuanLyID, iID_CapPheDuyetID: iID_CapPheDuyetID, iID_LoaiCongTrinhID: iID_LoaiCongTrinhID, iTrangThai: iTrangThai, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            setSearchCondition(sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID);
        }
    });
}

function setSearchCondition(sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID) {
    $("#txtTenDuAn").val(sTenDuAn);
    $("#tThoiGianThucHienTuNam").val(sKhoiCong);
    $("#tThoiGianThucHienDenNam").val(sKetThuc);
    $("#iID_DonViQuanLyID").val(iID_DonViQuanLyID);
    $("#iID_CapPheDuyetID").val(iID_CapPheDuyetID);
    $("#hid_iID_LoaiCongTrinhID").val(iID_LoaiCongTrinhID);
}

function BtnCreateDataClick() {
    window.location.href = "/QLVonDauTu/QLDuAn/CreateNew/";
}

function GetItemData(id) {
    window.location.href = "/QLVonDauTu/QLDuAn/CreateNew/" + id;
}

function ViewItemDetail(id) {
    window.location.href = "/QLVonDauTu/QLDuAn/xemChiTietDuAn/" + id;
}

function DeleteItem(id) {
    $.ajax({
        type: "POST",
        url: "/QLDuAn/checkDeleteDuAn",
        dataType: "json",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                var Title = 'Xác nhận xóa dự án';
                var Messages = [];
                Messages.push('Bạn có chắc chắn muốn xóa?');
                var FunctionName = "DeleteItemDA('" + id + "')";
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            } else {
                var Title = 'Lỗi khi xóa dự án';
                var messErr = [];
                messErr.push(r.errMes);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return;
            }
        }
    });  
}

function DeleteItemDA(id) {
    $.ajax({
        type: "POST",
        url: "/QLDuAn/VDTDuAnDelete",
        data: { id: id },
        success: function (r) {
            if (r.status) {
                alert(r.sMessage);
                SearchData();
            }
        }
    });
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/QLDuAn/Index";
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

function OnExportExcel() {
    var lstId = [];
    $(".iIdExport:checked").each(function (index, item) {
        lstId.push($(item).val());
    });
    if (lstId == null || lstId.length == 0) {
        alert("Chưa chọn bản ghi nào để xuất excel !");
        return;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDuAn/OnExport",
        data: { ids: lstId },
        success: function (data) {
            if (data.status) {
                window.open("/QLVonDauTu/QLDuAn/ExportReport");
            }
            else {
                var Title = 'Lỗi in báo cáo';
                var messErr = [];
                messErr.push(data.listErrMess);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return false;
            }
        }
    });
}