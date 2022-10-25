var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ChangePage(iCurrentPage = 1) {
    var sCode = $("#txt_MaNoiDungChiSearch").val();
    var sName = $("#txt_TenNoiDungChiSearch").val();
    GetListData(sCode, sName, iCurrentPage);
}

function CancelSaveData() {
    window.location.href = "/DanhMuc/DMNoiDungChi/Index";
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMNoiDungChi/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucNoiDungChi").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalDanhMucNoiDungChiLabel").html('Thêm mới danh mục nội dung chi');
            }
            else {
                $("#modalDanhMucNoiDungChiLabel").html('Cập nhật danh mục nội dung chi');
            }
        }
    });
}

function SaveData() {
    var data = {};
    data.iID_NoiDungChi = $("#txt_ID_NoiDungChi").val();
    data.sMaNoiDungChi = $("#txt_MaNoiDungChi").val();
    data.sTenNoiDungChi = $("#txt_TenNoiDungChi").val();
    data.iID_Parent = $("#iID_Parent").val();
    data.sGhiChu = $("#txt_GhiChu").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/DMNoiDungChi/DMNoiDungChiSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/DanhMuc/DMNoiDungChi/Index";
            } else {
                var Title = 'Lỗi lưu danh mục nội dung chi';
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

function DeleteItem(id) {
    var Title = 'Xác nhận xóa danh mục nội dung chi';
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
        url: "/DanhMuc/DMNoiDungChi/DMNoiDungChiDelete",
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
        url: "/DanhMuc/DMNoiDungChi/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucNoiDungChi").html(data);
            $("#modalDanhMucNoiDungChiLabel").html('Chi tiết danh mục nội dung chi');
        }
    });
}

function ValidateData(data) {
    var Title = "Lỗi thêm mới danh mục nội dung chi";
    var sMessError = [];

    if (data.sMaNoiDungChi == null || data.sMaNoiDungChi == "") {
        sMessError.push("Mã nội dung chi chưa nhập !");
    } else if (data.sMaNoiDungChi.length > 50) {
        sMessError.push("Mã nội dung chi vượt quá số kí tự !");
    }

    if (data.sTenNoiDungChi == null || data.sTenNoiDungChi == "") {
        sMessError.push("Tên nội dung chi chưa nhập !");
    } else if (data.sTenNoiDungChi.length > 500) {
        sMessError.push("Tên nội dung chi vượt quá số kí tự !");
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
        data: { code: sCode, name: sName, _paging : _paging },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}
