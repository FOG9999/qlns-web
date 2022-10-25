var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

///save data ---------------------------------
function Save() {
    var data = GetDataKhoiTaoCapPhat();
    if (!ValidateData(data)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/KhoiTaoCapPhat/Save",
        data: { data: data },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/KhoiTaoCapPhat/Detail?id=" + r.dataID + " &edit=true";
            } else {
                var Title = 'Lỗi lưu thông tin khởi tạo cấp phát';
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

function Cancel() {
    window.location.href = "/QLNH/KhoiTaoCapPhat";
}

function SaveTongHop(listId) {
    var data = GetDataQuyetToanDienDo();
    if (!ValidateData(data, true)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanNienDo/SaveTongHop",
        data: { data: data, listId: listId },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QuyetToanNienDo";
            } else {
                var Title = 'Lỗi lưu thông tin quyết toán niên độ';
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

function GetDataKhoiTaoCapPhat() {
    var data = {};
    data.ID = $("#hidKTCapPhatID").val();
    data.dNgayKhoiTao = $("#txtNgayKhoiTao").val();
    data.iNamKhoiTao = $("#slbNamKhoiTao").val() == 0 ? null : $("#slbNamKhoiTao").val();
    data.sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();
    data.iID_DonViID = $("#slbDonVi").val() == GUID_EMPTY ? null : $("#slbDonVi").val();
    data.iID_TiGiaID = $("#slbTiGia").val() == GUID_EMPTY ? null : $("#slbTiGia").val();
    data.iID_MaDonVi = $("<div/>").text($.trim($("#slbDonVi").find("option:selected").data("madonvi"))).html();
    return data;
}


///Validate///------------------------------------------------------------------------
function ValidateData(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa khởi tạo cấp phát';
    var Messages = [];

    if (data.iID_DonViID == null || data.iID_DonViID == GUID_EMPTY) {
        Messages.push("Đơn vị  chưa chọn !");
    }
    if (data.iID_TiGiaID == null || data.iID_TiGiaID == GUID_EMPTY) {
        Messages.push("Tỉ giá chưa chọn !");
    }
    if (data.dNgayKhoiTao == null) {
        Messages.push("Ngày khởi tạo chưa chọn !");
    }
    if ($.trim($("#txtNgayKhoiTao").val()) != "" && !dateIsValid($.trim($("#txtNgayKhoiTao").val()))) {
        Messages.push("Ngày khởi tạo không hợp lệ !");
    }
    if (data.iNamKhoiTao == null || data.iNamKhoiTao == 0) {
        Messages.push("Năm khởi tạo chưa chọn !");
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
