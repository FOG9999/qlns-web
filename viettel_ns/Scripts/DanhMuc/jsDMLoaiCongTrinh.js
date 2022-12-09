var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

$(document).ready(function () {
    GetListData();
});

function CancelSaveData() {
    window.location.href = "/DanhMuc/DMLoaiCongTrinh/Index";
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMLoaiCongTrinh/GetModal",
        data: { id: id },
        success: function (data) {
            console.log(data);
            $("#contentModalDanhMucLoaiCongTrinh").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalDanhMucLoaiCongTrinhLabel").html('Thêm mới danh mục loại công trình');
            }
            else {
                $("#modalDanhMucLoaiCongTrinhLabel").html('Cập nhật danh mục loại công trình');
            }
        }
    });
}

function SaveData() {
    if (!(/^\d*$/.test($("#txt_ThuTu").val()))) {
        var Title = 'Lỗi lưu danh mục loại công trình';
        var messErr = [];
        messErr.push("Thứ tự phải là số !");
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

    var data = {};
    data.iID_LoaiCongTrinh = $("#txt_ID_LoaiCongTrinh").val();
    data.sMaLoaiCongTrinh = $("#txt_MaCongTrinh").val();
    data.sTenVietTat = $("#txt_TenVietTat").val();
    data.sTenLoaiCongTrinh = $("#txt_TenCongTrinh").val();
    data.sMoTa = $("#txt_NoiDung").val();
    data.iThuTu = parseInt($("#txt_ThuTu").val());
    data.iID_Parent = $("#txt_IdParent :selected").val();

    data.L = $("#txt_L").val();
    data.K = $("#txt_K").val();
    data.LNS = $("#txt_LNS").val();
    data.M = $("#txt_M").val();
    data.TM = $("#txt_TM").val();
    data.TTM = $("#txt_TTM").val();
    data.NG = $("#txt_NG").val();
    data.TNG1 = $("#txt_TNG1").val();
    data.TNG2 = $("#txt_TNG2").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/DanhMuc/DMLoaiCongTrinh/DMLoaiCongTrinhSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/DanhMuc/DMLoaiCongTrinh/Index";
            } else {
                var Title = 'Lỗi lưu danh mục loại công trình';
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
    var Title = 'Xác nhận xóa danh mục loại công trình';
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
        url: "/DanhMuc/DMLoaiCongTrinh/DeleteItem",
        data: { id: id },
        success: function (r) {
            if (!r.bIsComplete) {
                var Title = 'Lỗi xóa danh mục loại công trình';
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
            GetListData();
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/DanhMuc/DMLoaiCongTrinh/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalDanhMucLoaiCongTrinh").html(data);
            $("#modalDanhMucLoaiCongTrinhLabel").html('Chi tiết danh mục loại công trình');
        }
    });
}

function GetItemData(id) {
    $("#modalDanhMucLoaiCongTrinh").modal('toggle');
    OpenModal(id);
}

function ViewDetail(id) {
    $("#modalDanhMucLoaiCongTrinh").modal('toggle');
    OpenModalDetail(id);
}

function ValidateData(data) {
    var Title = "Lỗi thêm mới danh mục loại chi phí";
    if ($("#txt_ID_LoaiCongTrinh").val() != GUID_EMPTY) {
        Title = "Lỗi chỉnh sửa danh mục loại chi phí";
    }
    var sMessError = [];

    if (data.sMaLoaiCongTrinh == null || data.sMaLoaiCongTrinh == "") {
        sMessError.push("Mã loại công trình chưa nhập !");
    } else if (data.sMaLoaiCongTrinh.length > 50) {
        sMessError.push("Mã công trình vượt quá số kí tự !");
    }

    if (data.sTenLoaiCongTrinh == null || data.sTenLoaiCongTrinh == "") {
        sMessError.push("Tên loại công trình chưa nhập !");
    } else if (data.sTenLoaiCongTrinh.length > 300) {
        sMessError.push("Tên loại công trình vượt quá số kí tự !");
    }

    if (data.sTenVietTat == null || data.sTenVietTat == "") {
        sMessError.push("Tên viết tắt chưa nhập !");
    } else if (data.sTenVietTat.length > 100) {
        sMessError.push("Tên viết tắt vượt quá số kí tự !");
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
var isShowSearchDMLoaiCongTrinh = true;
function GetListData() {

    var sTenLoaiCongTrinh = $("#txtTenLoaiCongTrinh").val();
    $.ajax({
        type: "POST",
        url: "/DanhMuc/DMLoaiCongTrinh/GetListLoaiCongTrinhInPartial",
        data: { sTenLoaiCongTrinh },
        success: function (r) {
            var columns = [
                { sField: "iID_LoaiCongTrinh", bKey: true },
                { sField: "iID_Parent", bParentKey: true },

                { sTitle: "Mã loại công trình", sField: "sMaLoaiCongTrinh", iWidth: "10%", sTextAlign: "left" },
                { sTitle: "Tên viết tắt", sField: "sTenVietTat", iWidth: "10%", sTextAlign: "left" },
                { sTitle: "Tên loại công trình", sField: "sMaLoaiCongTrinh-sTenLoaiCongTrinh", iWidth: "24%", sTextAlign: "left", bHaveIcon: 1 },
                { sTitle: "Mô tả", sField: "sMoTa", iWidth: "20%", sTextAlign: "left"},
                { sTitle: "Thứ tự", sField: "iThuTu", iWidth: "5%", sTextAlign: "center" },
                { sTitle: "Tên loại công trình cha", sField: "sTenLoaiCha", iWidth: "15%", sTextAlign: "left" },
            ];
            var button = { bUpdate: 1, bDelete: 1, bInfo: 1 };
            var sortedData = r.data.sort((a, b) => {
                if (a.sMaLoaiCongTrinh < b.sMaLoaiCongTrinh) {
                    return -1;
                }
                else return 1;
            })
            var sHtml = GenerateTreeTable(sortedData, columns, button, true, false, isShowSearchDMLoaiCongTrinh)

            $("#ViewTable").html(sHtml);
            filter();
        }
    });
}

function GetListData_By_Name() {
    var sTenLoaiCongTrinh = $("#txtTenLoaiCongTrinh").val();
    var sMaLoaiCongTrinh = $("#txtMaLoai").val();
    var sTenVietTat = $("#txtTenVietTat").val();
    var iThuTu = $("#txtThuTu").val();
    var sMoTa = $("#txtMoTa").val();
    if (sTenLoaiCongTrinh == "" && sMaLoaiCongTrinh == "" && sTenVietTat == "" && iThuTu == "" && sMoTa == "") {
        GetListData();
    } else {
        $.ajax({
            type: "POST",
            url: "/DanhMuc/DMLoaiCongTrinh/GetListLoaiCongTrinhByName",
            data: { sTenLoaiCongTrinh, sTenVietTat, sMaLoaiCongTrinh, iThuTu, sMoTa },
            success: function (r) {
                var columns = [
                    { sField: "iID_LoaiCongTrinh", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },
                    { sTitle: "Mã loại công trình", sField: "sMaLoaiCongTrinh", iWidth: "10%", sTextAlign: "left" },
                    { sTitle: "Tên viết tắt", sField: "sTenVietTat", iWidth: "10%", sTextAlign: "left" },
                    { sTitle: "Tên loại công trình", sField: "sMaLoaiCongTrinh-sTenLoaiCongTrinh", iWidth: "24%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Mô tả", sField: "sMoTa", iWidth: "20%", sTextAlign: "left" },
                    { sTitle: "Thứ tự", sField: "iThuTu", iWidth: "5%", sTextAlign: "center" },
                    { sTitle: "Tên loại công trình cha", sField: "sTenLoaiCha", iWidth: "15%", sTextAlign: "left" }
                ];
                var button = { bUpdate: 1, bDelete: 1, bInfo: 1 };
                var sHtml = GenerateTreeTable(r.data, columns, button, true, true, isShowSearchDMLoaiCongTrinh)
                $("#ViewTable").html(sHtml);
                //Keep value search
                $("#txtTenLoaiCongTrinh").val(sTenLoaiCongTrinh);
                $("#txtMaLoai").val(sMaLoaiCongTrinh);
                $("#txtTenVietTat").val(sTenVietTat);
                $("#txtThuTu").val(iThuTu);
                $("#txtMoTa").val(sMoTa);
                filter();
            }
        });
    }
}
function filter() {
    $('.clearable').on('keypress', function (e) {
        if (e.which === 13) {
            GetListData_By_Name();
        }
    });
    }
