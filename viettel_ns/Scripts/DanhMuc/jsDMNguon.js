var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ChangePage(iCurrentPage = 1) {
    var sMaNguon = $("#txtMaNguonSearch").val();
    var sNoiDung = $("#txtNoiDungSearch").val();
    var iID_NguonCha = $("#txt_IDNguonChaSearch :selected").val();
    GetListData(sMaNguon, sNoiDung, iID_NguonCha, iCurrentPage);
}

function CancelSaveData() {
    window.location.href = "/DanhMuc/DMNguon/Index";
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMNguon/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucNguon").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalDanhMucNguonLabel").html('Thêm mới danh mục nguồn');
            }
            else {
                $("#modalDanhMucNguonLabel").html('Cập nhật danh mục nguồn');
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
    data.sMaCTMT  = $("#txt_MaChuongTrinhMucTieu").val();
    data.sMaNguon = $("#txt_MaNguon").val();
    data.sLoai = $("#txt_Loai").val();
    data.sKhoan = $("#txt_Khoan").val();
    data.sNoiDung = $("#txt_NoiDung").val();
    data.iID_Nguon = $("#txt_ID_Nguon").val();
    data.iID_NguonCha = $("#txt_iID_NguonCha :selected").val();
    if (data.iID_NguonCha == GUID_EMPTY) {
        data.iID_NguonCha = null;
    }

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/DanhMuc/DMNguon/DMNguonSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/DanhMuc/DMNguon/Index";
            } else {
                var Title = 'Lỗi lưu danh mục nguồn';
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
    $.ajax({
        type: "POST",
        url: "/DanhMuc/DMNguon/CheckHasChild",
        data: { iID_Nguon: id },
        success: function (data) {
            if (data.status == true) {
                var Title = 'Lỗi xóa danh mục nguồn';
                var Messages = [];
                Messages.push('Hãy xóa các nguồn con!');
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
            else {
                var Title = 'Xác nhận xóa danh mục nguồn';
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
        }
    });
        //var Title = 'Xác nhận xóa danh mục nguồn';
        //var Messages = [];
        //Messages.push('Bạn có chắc chắn muốn xóa?');
        //var FunctionName = "Delete('" + id + "')";
        //$.ajax({
        //    type: "POST",
        //    url: "/Modal/OpenModal",
        //    data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        //    success: function (data) {
        //        $("#divModalConfirm").html(data);
        //    }
        //});
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/DanhMuc/DMNguon/DMNguonDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

//function CheckHasChild(iID_Nguon) {
//    $.ajax({
//        type: "POST",
//        url: "/DanhMuc/DMNguon/CheckHasChild",
//        data: { iID_Nguon: iID_Nguon },
//        success: function (data) {
//            if (data.status == true) {
//                var Title = 'Lỗi xóa danh mục nguồn';
//                var Messages = [];
//                Messages.push('Hãy xóa các nguồn con!');
//                $.ajax({
//                    type: "POST",
//                    url: "/Modal/OpenModal",
//                    data: { Title: Title, Messages: Messages, Category: ERROR },
//                    success: function (data) {
//                        $("#divModalConfirm").html(data);
//                    }
//                });
//            }
//            else {

//            }
//        }
//    });
//}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMNguon/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucNguon").html(data);
            $("#modalDanhMucNguonLabel").html('Chi tiết danh mục nguồn');
        }
    });
}

function ValidateData(data) {
    var sMessError = [];
    var Title = 'Lỗi thêm mới danh mục nguồn';
    if (data.sMaCTMT == null || data.sMaCTMT  == "") {
        sMessError.push("Mã danh mục chương trình mục tiêu !");
    } else if (data.sMaCTMT.length > 50) {
        sMessError.push("Mã danh mục chương trình mục tiêu vượt quá số kí tự !");
    }

    if (data.sMaNguon == null || data.sMaNguon  == "") {
        sMessError.push("Mã danh mục nguồn chưa nhập !");
    } else if (data.sMaNguon .length > 50) {
        sMessError.push("Mã danh mục nguồn vượt quá số kí tự !");
    }

    if (data.sLoai == null || data.sLoai  == "") {
        sMessError.push("Loại danh mục chưa nhập");
    } else if (data.sLoai .length > 50) {
        sMessError.push("Loại danh mục vượt quá số kí tự !");
    }

    if (data.sKhoan == null || data.sKhoan  == "") {
        sMessError.push("Khoản danh mục chưa nhập");
    } else if (data.sKhoan .length > 50) {
        sMessError.push("Khoản danh mục vượt quá số kí tự !");
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

function GetListData(sMaNguon, sNoiDung, iID_NguonCha, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sMaNguon: sMaNguon, sNoiDung: sNoiDung, iID_NguonCha: iID_NguonCha, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}

function GetItemData(id) {
    window.location.href = "/DanhMuc/DMLoaiDuToan/Update/" + id;
}

