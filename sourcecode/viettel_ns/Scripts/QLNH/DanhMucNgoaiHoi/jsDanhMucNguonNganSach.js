var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var MaNguonNganSach = "";
    var sTenNguonNganSach = "";
    var iTrangThai = -1;

    GetListData(MaNguonNganSach, sTenNguonNganSach, iTrangThai, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaNguonNganSach = $("<div/>").text($.trim($("#txtMaNguonNganSach").val())).html();
    var sTenNguonNganSach = $("<div/>").text($.trim($("#txtTenNguonNganSach").val())).html();
    var iTrangThai = $("#slbTrangThaiSearch").val();

    GetListData(sMaNguonNganSach, sTenNguonNganSach, iTrangThai, iCurrentPage);
}

function GetListData(sMaNguonNganSach, sTenNguonNganSach, iTrangThai, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNguonNganSach/DanhMucNguonNganSachSearch",
        data: { _paging: _paging, sMaNguonNganSach: sMaNguonNganSach, sTenNguonNganSach: sTenNguonNganSach, iTrangThai: iTrangThai },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtMaNguonNganSach").val($("<div/>").html(sMaNguonNganSach).text());
            $("#txtTenNguonNganSach").val($("<div/>").html(sTenNguonNganSach).text());
            $("#slbTrangThaiSearch").val(iTrangThai);
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNguonNganSach/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalNguonNganSach").html(data);
            $("#modalNguonNganSachLabel").html('Chi tiết nguồn ngân sách');
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNguonNganSach/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalNguonNganSach").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalNguonNganSachLabel").html('Thêm mới nguồn ngân sách');
            } else {
                $("#modalNguonNganSachLabel").html('Sửa nguồn ngân sách');
            }

            $("#txtsMaLoaiNguonNganSach").keydown(function (event) {
                ValidateInputKeydown(event, this, 1);
            }).blur(function (event) {
                ValidateInputFocusOut(event, this, 1);
            });
            $("#txtsSTT").keydown(function (event) {
                ValidateInputKeydown(event, this, 1);
            }).blur(function (event) {
                ValidateInputFocusOut(event, this, 1);
            });
            $("#slbTrangThai").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucNguonNganSach/NguonNganSachDelete",
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
    var Title = 'Xác nhận xóa chứng từ nguồn ngân sách';
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
    data.iID_NguonNganSach = $("#iID_NguonNganSachModal").val();
    data.iID_MaNguonNganSach = $.trim($("#txtsMaLoaiNguonNganSach").val());
    data.sTen = $("<div/>").text($.trim($("#txtsTenNguonNganSach").val())).html();
    data.iTrangThai = $("#slbTrangThai").val();
    data.iSTT = $.trim($("#txtsSTT").val());
    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucNguonNganSach/NguonNganSachSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucNguonNganSach";
            } else {
                var Title = 'Lỗi lưu nguồn ngân sách';
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
    var Title = 'Lỗi thêm mới/chỉnh sửa nguồn ngân sách';
    var Messages = [];

    if (data.iID_MaNguonNganSach == null || data.iID_MaNguonNganSach == "") {
        Messages.push("Mã nguồn ngân sách chưa nhập !");
    }

    if (data.sTen == null || data.sTen == "") {
        Messages.push("Tên nguồn ngân sách chưa nhập !");
    }

    if ($.trim($("#txtsTenNguonNganSach").val()).length > 50) {
        Messages.push("Tên nguồn ngân sách nhập quá 50 kí tự !");
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