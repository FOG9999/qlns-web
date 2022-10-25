var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ChangePage(iCurrentPage = 1) {
    var sCode = $("#txt_MaLoaiDuToanSearch").val();
    var sName = $("#txt_TenLoaiDuToanSearch").val();
    GetListData(sCode, sName, iCurrentPage);
}

function CancelSaveData() {
    window.location.href = "/DanhMuc/DMLoaiDuToan/Index";
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMLoaiDuToan/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucLoaiDuToan").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalDanhMucLoaiDuToanLabel").html('Thêm mới danh mục loại dự toán');
            }
            else {
                $("#modalDanhMucLoaiDuToanLabel").html('Cập nhật danh mục loại dự toán');
            }
        }
    });
}

function getModalCheckExistMa() {
    var Title = 'Lỗi lưu danh mục loại dự toán';
    var messErr = [];
    messErr.push('Mã dự toán đã tồn tại!');
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: messErr, Category: ERROR },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function SaveData() {
    var data = {};
    data.iID_LoaiDuToan = $("#txt_ID_LoaiDuToan").val();
    data.sMaLoaiDuToan = $("#txt_MaLoaiDuToan").val();
    data.sTenLoaiDuToan = $("#txt_TenLoaiDuToan").val();
    data.sGhiChu = $("#txt_GhiChu").val();

    if (!ValidateData(data)) {
        return false;
    }

    var dataCheckExist = {};
    dataCheckExist.sCode = data.sMaLoaiDuToan;
    if (data.iID_LoaiDuToan == GUID_EMPTY || data.iID_LoaiDuToan == null) {
        dataCheckExist.id = null;
    } else {
        dataCheckExist.id = data.iID_LoaiDuToan;
    }

    $.ajax({
        type: "POST",
        url: "/DanhMuc/DMLoaiDuToan/CheckExistMaDMLoaiDuAn",
        data: {
            id: dataCheckExist.id,
            sCode: dataCheckExist.sCode
        },
        success: function (r) {
            if (r == "False") {
                getModalCheckExistMa();
                return;
            }
            $.ajax({
                type: "POST",
                url: "/DanhMuc/DMLoaiDuToan/DMLoaiDuToanSave",
                data: { data: data },
                success: function (r) {
                    if (r == "True") {
                        window.location.href = "/DanhMuc/DMLoaiDuToan/Index";
                    }
                }
            });
        }
    });
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa danh mục loại dự toán';
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
        url: "/DanhMuc/DMLoaiDuToan/DMLoaiDuToanDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMLoaiDuToan/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucLoaiDuToan").html(data);
            $("#modalDanhMucLoaiDuToanLabel").html('Chi tiết danh mục loại dự toán');
        }
    });
}

function ValidateData(data) {
    var sMessError = [];
    var Title = 'Lỗi thêm mới danh mục loại dự toán';
    if (data.sMaLoaiDuToan == null || data.sMaLoaiDuToan == "") {
        sMessError.push("Mã danh mục chưa nhập");
    } else if (data.sMaLoaiDuToan.length > 50) {
        sMessError.push("Mã danh mục vượt quá số kí tự !");
    }

    if (data.sTenLoaiDuToan == null || data.sTenLoaiDuToan == "") {
        sMessError.push("Tên danh mục chưa nhập !");
    } else if (data.sTenLoaiDuToan.length > 500) {
        sMessError.push("Tên danh mục vượt quá số kí tự !");
    }

    if (sMessError != null && sMessError != undefined && sMessError.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: sMessError, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}

function GetListData(sCode, sName, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { code: sCode, name: sName, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}

function GetItemData(id) {
    window.location.href = "/DanhMuc/DMLoaiDuToan/Update/" + id;
}

