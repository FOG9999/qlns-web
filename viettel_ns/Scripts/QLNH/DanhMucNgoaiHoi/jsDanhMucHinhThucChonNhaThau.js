var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaHinhThuc = "";
    var sTenVietTat = "";
    var sTenHinhThuc = "";
    var sMoTa = "";

    GetListData(sMaHinhThuc, sTenVietTat,sTenHinhThuc, sMoTa, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaHinhThuc = $("<div/>").text($.trim($("#txtMaHinhThuc").val())).html();
    var sTenVietTat = $("<div/>").text($.trim($("#txtTenVietTat").val())).html();
    var sTenHinhThuc = $("<div/>").text($.trim($("#txtTenHinhThuc").val())).html();
    var sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();

    GetListData(sMaHinhThuc, sTenVietTat, sTenHinhThuc, sMoTa, iCurrentPage);
}
function GetListData(sMaHinhThuc, sTenVietTat, sTenHinhThuc, sMoTa, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucHinhThucChonNhaThau/DanhMucHinhThucChonNhaThauSearch",
        data: { _paging: _paging, sMaHinhThuc: sMaHinhThuc, sTenVietTat: sTenVietTat, sTenHinhThuc: sTenHinhThuc, sMoTa: sMoTa },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtMaHinhThuc").val($("<div/>").html(sMaHinhThuc).text());
            $("#txtTenVietTat").val($("<div/>").html(sTenVietTat).text());
            $("#txtTenHinhThuc").val($("<div/>").html(sTenHinhThuc).text());
            $("#txtMoTa").val($("<div/>").html(sMoTa).text());
        }
    });
}
function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucHinhThucChonNhaThau/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalHinhThuc").html(data);
            $("#modalHinhThucLabel").html('Chi tiết hình thức chọn nhà thầu');
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucHinhThucChonNhaThau/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalHinhThuc").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalHinhThucLabel").html('Thêm mới hình thức chọn nhà thầu');
            }
            else {
                $("#modalHinhThucLabel").html('Sửa hình thức chọn nhà thầu');
            }
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucHinhThucChonNhaThau/HinhThucChonNhaThauDelete",
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
    var Title = 'Xác nhận xóa hình thức chọn nhà thầu';
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
    data.ID = $("#iID_HinhThucModal").val();
    data.sMaHinhThuc = $("<div/>").text($.trim($("#txtsMaHinhThuc").val())).html();
    data.sTenVietTat = $("<div/>").text($.trim($("#txtsTenVietTat").val())).html();
    data.sTenHinhThuc = $("<div/>").text($.trim($("#txtsTenHinhThuc").val())).html();
    data.sMoTa = $("<div/>").text($.trim($("#txtsMoTa").val())).html();
    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucHinhThucChonNhaThau/HinhThucChonNhaThauSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucHinhThucChonNhaThau";
            }
            else {
                var Title = 'Lỗi lưu hình thức chọn nhà thầu';
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
    var Title = "Lỗi " + ((data.ID == "" || data.ID == GUID_EMPTY) ? "thêm mới" : "chỉnh sửa") + " hình thức chọn nhà thầu";
    var Messages = [];

    if (data.sMaHinhThuc == null || data.sMaHinhThuc == "") {
        Messages.push("Mã hình thức chọn nhà thầu chưa nhập !");
    }

    if (data.sTenHinhThuc == null || data.sTenHinhThuc == "") {
        Messages.push("Tên hình thức chọn nhà thầu chưa nhập !");
    }

    if ($.trim($("#txtsMaHinhThuc").val()).length > 50) {
        Messages.push("Mã hình thức chọn nhà thầu nhập quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenVietTat").val()).length > 50) {
        Messages.push("Tên viết tắt nhập quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenHinhThuc").val()).length > 300) {
        Messages.push("Tên hình thức chọn nhà thầu nhập quá 300 kí tự !");
    }

    if ($.trim($("#txtsMoTa").val()).length > 255) {
        Messages.push("Mô tả hình thức chọn nhà thầu nhập quá 255 kí tự !");
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