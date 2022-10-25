var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaNoiDungChi = "";
    var sTenNoiDungChi = "";
    var sMoTa = "";
    GetListData(sMaNoiDungChi, sTenNoiDungChi, sMoTa, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaNoiDungChi = $("<div/>").text($.trim($("#txtMaNoiDungChi").val())).html();
    var sTenNoiDungChi = $("<div/>").text($.trim($("#txtTenNoiDungChi").val())).html();
    var sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();

    GetListData(sMaNoiDungChi, sTenNoiDungChi, sMoTa, iCurrentPage);
}

function GetListData(sMaNoiDungChi, sTenNoiDungChi, sMoTa, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNoiDungChi/DanhMucNoiDungChiSearch",
        data: { _paging: _paging, sMaNoiDungChi: sMaNoiDungChi, sTenNoiDungChi: sTenNoiDungChi, sMoTa: sMoTa },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtMaNoiDungChi").val($("<div/>").html(sMaNoiDungChi).text());
            $("#txtTenNoiDungChi").val($("<div/>").html(sTenNoiDungChi).text());
            $("#txtMoTa").val($("<div/>").html(sMoTa).text());
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNoiDungChi/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalNoiDung").html(data);
            $("#modalNoiDungLabel").html('Chi tiết nội dung chi');
        }
    });
}

function OpenModal(id) {

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNoiDungChi/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalNoiDung").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalNoiDungLabel").html('Thêm mới nội dung chi');
            }
            else {
                $("#modalNoiDungLabel").html('Sửa nội dung chi');
            }
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucNoiDungChi/NoiDungChiDelete",
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
    var Title = 'Xác nhận xóa chứng từ nội dung chi';
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
    data.ID = $("#iID_NoiDungModal").val();
    data.sMaNoiDungChi = $("<div/>").text($.trim($("#txtsMaNoiDungChi").val())).html();
    data.sTenNoiDungChi = $("<div/>").text($.trim($("#txtsTenNoiDungChi").val())).html();
    data.sMoTa = $("<div/>").text($.trim($("#txtsMoTa").val())).html();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucNoiDungChi/NoiDungChiSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucNoiDungChi";
            } else {
                var Title = 'Lỗi lưu nội dung chi';
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
    var Title = 'Lỗi thêm mới/chỉnh sửa loại hợp đồng';
    var Messages = [];

    if (data.sMaNoiDungChi == null || data.sMaNoiDungChi == "") {
        Messages.push("Mã nội dung chi chưa nhập !");
    }

    if (data.sTenNoiDungChi == null || data.sTenNoiDungChi == "") {
        Messages.push("Tên nội dung chi chưa nhập");
    }

    if ($.trim($("#txtsMaNoiDungChi").val()).length > 50) {
        Messages.push("Mã nội dung chi nhập quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenNoiDungChi").val()).length > 50) {
        Messages.push("Tên nội dung chi nhập quá 50 kí tự !");
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