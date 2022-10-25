var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaPhuongThuc = "";
    var sTenVietTat = "";
    var sTenPhuongThuc = "";
    var sMoTa = "";

    GetListData(sMaPhuongThuc, sTenVietTat, sTenPhuongThuc, sMoTa, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaPhuongThuc = $("<div/>").text($.trim($("#txtMaPhuongThuc").val())).html();
    var sTenVietTat = $("<div/>").text($.trim($("#txtTenVietTat").val())).html();
    var sTenPhuongThuc = $("<div/>").text($.trim($("#txtTenPhuongThuc").val())).html();
    var sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();

    GetListData(sMaPhuongThuc, sTenVietTat, sTenPhuongThuc, sMoTa, iCurrentPage);
}

function GetListData(sMaPhuongThuc, sTenVietTat, sTenPhuongThuc, sMoTa, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucPhuongThucChonNhaThau/DanhMucPhuongThucChonNhaThauSearch",
        data: { _paging: _paging, sMaPhuongThuc: sMaPhuongThuc, sTenVietTat: sTenVietTat, sTenPhuongThuc: sTenPhuongThuc, sMoTa: sMoTa },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtMaPhuongThuc").val($("<div/>").html(sMaPhuongThuc).text());
            $("#txtTenVietTat").val($("<div/>").html(sTenVietTat).text());
            $("#txtTenPhuongThuc").val($("<div/>").html(sTenPhuongThuc).text());
            $("#txtMoTa").val($("<div/>").html(sMoTa).text());
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucPhuongThucChonNhaThau/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalPhuongThuc").html(data);
            $("#modalPhuongThucLabel").html('Chi tiết phương thức chọn nhà thầu');
        }
    });
}

function OpenModal(id) {

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucPhuongThucChonNhaThau/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalPhuongThuc").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalPhuongThucLabel").html('Thêm mới phương thức chọn nhà thầu');
            } else {
                $("#modalPhuongThucLabel").html('Sửa phương thức chọn nhà thầu');
            }
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucPhuongThucChonNhaThau/PhuongThucChonNhaThauDelete",
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
    var Title = 'Xác nhận xóa phương thức chọn nhà thầu';
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
    data.ID = $("#iID_PhuongThucModal").val();
    data.sMaPhuongThuc = $("<div/>").text($.trim($("#txtsMaPhuongThuc").val())).html();
    data.sTenVietTat = $("<div/>").text($.trim($("#txtsTenVietTat").val())).html();
    data.sTenPhuongThuc = $("<div/>").text($.trim($("#txtsTenPhuongThuc").val())).html();
    data.sMoTa = $("<div/>").text($.trim($("#txtsMoTa").val())).html();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucPhuongThucChonNhaThau/PhuongThucChonNhaThauSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucPhuongThucChonNhaThau";
            }
            else {
                var Title = 'Lỗi lưu phương thức chọn nhà thầu';
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
    var Title = "Lỗi " + ((data.ID == "" || data.ID == GUID_EMPTY) ? "thêm mới" : "chỉnh sửa") + " phương thức loại hợp đồng";
    var Messages = [];

    if (data.sMaPhuongThuc == null || data.sMaPhuongThuc == "") {
        Messages.push("Mã phương thức chọn nhà thầu chưa nhập !");
    }

    if (data.sTenPhuongThuc == null || data.sTenPhuongThuc == "") {
        Messages.push("Tên phương thức chọn nhà thầu chưa nhập");
    }

    if ($.trim($("#txtsMaHinhThuc").val()).length > 50) {
        Messages.push("Mã phương thức chọn nhà thầu nhập quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenVietTat").val()).length > 50) {
        Messages.push("Tên viết tắt nhập quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenHinhThuc").val()).length > 300) {
        Messages.push("Tên phương thức chọn nhà thầu nhập quá 300 kí tự !");
    }

    if ($.trim($("#txtsMoTa").val()).length > 255) {
        Messages.push("Mô tả phương thức chọn nhà thầu nhập quá 255 kí tự !");
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