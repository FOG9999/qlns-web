var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;


function ResetChangePage(iCurrentPage = 1) {
    var sMa = "";
    var sTenVietTat = "";
    var sTen = "";
    var sMoTa = "";

    GetListData(sMa, sTenVietTat, sMoTa, sTen, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMa = $("<div/>").text($.trim($("#txtMa").val())).html();
    var sTenVietTat = $("<div/>").text($.trim($("#txtTenVietTat").val())).html();
    var sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();
    var sTen = $("<div/>").text($.trim($("#txtTen").val())).html();

    GetListData(sMa, sTenVietTat, sMoTa, sTen, iCurrentPage);
}

function GetListData(sMa, sTenVietTat, sMoTa, sTen, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucPhanCapPheDuyet/DanhMucPhanCapPheDuyetSearch",
        data: { _paging: _paging, sMa: sMa, sTenVietTat: sTenVietTat, sMoTa: sMoTa, sTen: sTen },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtMa").val($("<div/>").html(sMa).text());
            $("#txtTenVietTat").val($("<div/>").html(sTenVietTat).text());
            $("#txtMoTa").val($("<div/>").html(sMoTa).text());
            $("#txtTen").val($("<div/>").html(sTen).text());
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucPhanCapPheDuyet/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalPhanCapPheDuyet").html(data);
            $("#modalPhanCapPheDuyetLabel").html('Xem chi tiết thông tin phân cấp phê duyệt');
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucPhanCapPheDuyet/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalPhanCapPheDuyet").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalPhanCapPheDuyetLabel").html('Thêm mới thông tin phân cấp phê duyệt');
            } else {
                $("#modalPhanCapPheDuyetLabel").html('Sửa phân cấp phê duyệt');
            }
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucPhanCapPheDuyet/PhanCapPheDuyetPopupDelete",
        data: { id: id },
        success: function (data) {
            $("#contentModalPhanCapPheDuyet").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalPhanCapPheDuyetLabel").html('Xóa !');
            } else {
                $("#modalPhanCapPheDuyetLabel").html('Xác nhận xóa thông tin phân cấp phê duyệt');
            }
        }
    });
}

function DeleteItem(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucPhanCapPheDuyet/PhanCapPheDuyetDelete",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                ChangePage();
            } else {
                var Title = 'Lỗi xóa phân cấp phê duyệt';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

function Save() {
    var data = {};
    data.ID = $("#iID_PhanCapPheDuyetModal").val();
    data.sMa = $("<div/>").text($.trim($("#txtsMa").val())).html();
    data.sTenVietTat = $("<div/>").text($.trim($("#txtsTenVietTat").val())).html();
    data.sTen = $("<div/>").text($.trim($("#txtsTen").val())).html();
    data.sMoTa = $("<div/>").text($.trim($("#txtsMoTa").val())).html();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucPhanCapPheDuyet/PhanCapPheDuyetSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucPhanCapPheDuyet";
            } else {
                var Title = 'Lỗi lưu phân cấp phê duyệt';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

function ValidateData(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa phân cấp phê duyệt';
    var Messages = [];

    if (data.sMa == null || data.sMa == "") {
        Messages.push("Mã phân cấp phê duyệt chưa nhập !");
    }

    if (data.sTen == null || data.sTen == "") {
        Messages.push("Tên phân cấp phê duyệt chưa nhập !");
    }

    if ($.trim($("#txtsMa").val()) != "" && $.trim($("#txtsMa").val()).length > 100) {
        Messages.push("Mã phân cấp phê duyệt vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsTen").val()) != "" && $.trim($("#txtsTen").val()).length > 300) {
        Messages.push("Tên phân cấp phê duyệt vượt quá 300 kí tự !");
    }

    if ($.trim($("#txtsTenVietTat").val()) != "" && $.trim($("#txtsTenVietTat").val()).length > 300) {
        Messages.push("Tên viết tắt phân cấp phê duyệt vượt quá 300 kí tự !");
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