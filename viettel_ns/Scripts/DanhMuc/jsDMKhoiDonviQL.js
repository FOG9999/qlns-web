var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function CancelSaveData() {
    window.location.href = "/DanhMuc/DMKhoiDonviQL/Index";
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMKhoiDonViQL/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucKhoiDonViQL").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalDanhMucKhoiDonViQLLabel").html('Thêm mới danh mục khối đơn vị quản lí');
            }
            else {
                $("#modalDanhMucKhoiDonViQLLabel").html('Cập nhật danh mục khối đơn vị quản lí');
            }
        }
    });
}

function SaveData() {
    var data = {};
    data.iID_Khoi = $("#txt_IDKhoi").val();
    data.sMaKhoi = $("#txtMaKhoi").val();
    data.sTenKhoi = $("#txt_TenKhoi").val();
    data.sGhiChu = $("#txt_GhiChu").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/DMKhoiDonviQL/DMKhoiDonviQLSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete == false) {
                var Title = 'Lỗi lưu danh mục khối đơn vị quản lí';
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
                return;
            }
            window.location.href = "/DanhMuc/DMKhoiDonviQL/Index";
        }
    });
}

function ValidateData(data) {
    var sMessError = "";

    if (data.sMaKhoi == null || data.sMaKhoi == "") {
        sMessError += "Mã khối chưa nhập !\n";
    } else if (data.sMaKhoi.length > 50) {
        sMessError += "Mã khối vượt quá số kí tự !\n";
    }

    if (data.sTenKhoi == null || data.sTenKhoi == "") {
        sMessError += "Tên khối chưa nhập !\n";
    } else if (data.sTenKhoi.length > 500) {
        sMessError += "Tên khối vượt quá số kí tự !\n";
    }

    if (sMessError != null && sMessError != "") {
        alert(sMessError);
        return false;
    }

    return true;
}

function GetItemData(id) {
    window.location.href = "/DanhMuc/DMKhoiDonviQL/Update/" + id;
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMKhoiDonviQL/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucKhoiDonViQL").html(data);
            $("#modalDanhMucKhoiDonViQLLabel").html('Chi tiết danh mục khối đơn vị quản lí');
        }
    });
}

function ChangePage(iCurrentPage = 1) {
    var sCode = $("#txt_MaKhoiSearch").val();
    var sName = $("#txt_TenKhoiSearch").val();
    GetListData(sCode, sName, iCurrentPage);
}

function ResetChangePage(iCurrentPage = 1) {
    var sCode = "";
    var sName = "";
    GetListData(sCode, sName, iCurrentPage);
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
            $("#txt_MaKhoiSearch").val(sCode);
            $("#txt_TenKhoiSearch").val(sName);
        }
    });
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa danh mục khối đơn vị';
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
        url: "/DanhMuc/DMKhoiDonviQL/DMKhoiDonviQLDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function GetDetail(id) {
    window.location.href = "/DanhMuc/DMKhoiDonviQL/Detail/" + id;
}