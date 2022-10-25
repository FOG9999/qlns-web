var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaLoaiTaiSan = "";
    var sTenLoaiTaiSan = "";
    var sMoTa = "";

    GetListData(sMaLoaiTaiSan, sTenLoaiTaiSan, sMoTa, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaLoaiTaiSan = $("<div/>").text($.trim($("#txtMaLoaiTaiSan").val())).html();
    var sTenLoaiTaiSan = $("<div/>").text($.trim($("#txtTenLoaiTaiSan").val())).html();
    var sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();

    GetListData(sMaLoaiTaiSan, sTenLoaiTaiSan, sMoTa, iCurrentPage);
}

function GetListData(sMaLoaiTaiSan, sTenLoaiTaiSan, sMoTa, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTaiSan/DanhMucTaiSanSearch",
        data: { _paging: _paging, sMaLoaiTaiSan: sMaLoaiTaiSan, sTenLoaiTaiSan: sTenLoaiTaiSan, sMoTa: sMoTa },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtMaLoaiTaiSan").val($("<div/>").html(sMaLoaiTaiSan).text());
            $("#txtTenLoaiTaiSan").val($("<div/>").html(sTenLoaiTaiSan).text());
            $("#txtMoTa").val($("<div/>").html(sMoTa).text());
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTaiSan/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalTaiSan").html(data);
            $("#modalTaiSanLabel").html('Chi tiết tài sản');
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTaiSan/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalTaiSan").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalTaiSanLabel").html('Thêm mới tài sản');
            } else {
                $("#modalTaiSanLabel").html('Sửa tài sản');
            }
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucTaiSan/TaiSanDelete",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                ChangePage();
            } else {
                var Title = 'Lỗi';
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

function Xoa(id) {
    var Title = 'Xác nhận xóa chứng từ tài sản';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}

function Save() {
    var data = {};
    data.ID = $("#iID_TaiSanModal").val();
    data.sMaLoaiTaiSan = $("<div/>").text($.trim($("#txtsMaLoaiTaiSan").val())).html();
    data.sTenLoaiTaiSan = $("<div/>").text($.trim($("#txtsTenLoaiTaiSan").val())).html();
    data.sMoTa = $("<div/>").text($.trim($("#txtsMoTa").val())).html();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucTaiSan/TaiSanSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucTaiSan";
            } else {
                var Title = 'Lỗi lưu tài sản';
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
    var Title = 'Lỗi thêm mới/chỉnh sửa tài sản';
    var Messages = [];

    if (data.sMaLoaiTaiSan == null || data.sMaLoaiTaiSan == "") {
        Messages.push("Mã tài sản chưa nhập !");
    }

    if (data.sTenLoaiTaiSan == null || data.sTenLoaiTaiSan == "") {
        Messages.push("Tên tài sản chưa nhập");
    }

    if ($.trim($("#txtsMaLoaiTaiSan").val()) != "" && $.trim($("#txtsMaLoaiTaiSan").val()).length > 50) {
        Messages.push("Mã tài sản vượt quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenLoaiTaiSan").val()) != "" && $.trim($("#txtsTenLoaiTaiSan").val()).length > 50) {
        Messages.push("Tên tài sản vượt quá 50 kí tự !");
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