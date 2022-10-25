var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;


///save data ---------------------------------
function Save(isTongHop) {
    var data = GetDataQuyetToanDienDo();
    if (!ValidateData(data, isTongHop)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanNienDo/Save",
        data: { data: data },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QuyetToanNienDo/Detail?id=" + r.dataID + "&edit=" + !isTongHop;
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

function GetDataQuyetToanDienDo() {
    var data = {};
    data.ID = $("#hidQTNienDoID").val();
    data.sSoDeNghi = $("<div/>").text($.trim($("#txtSoDeNghi").val())).html();
    data.dNgayDeNghi = $("#txtNgayDeNghi").val();
    data.iNamKeHoach = $("#slbNamKeHoach").val() == 0 ? null : $("#slbNamKeHoach").val();
    data.iLoaiQuyetToan = $("#slbLoaiQuyetToan").val() == 0 ? null : $("#slbLoaiQuyetToan").val();

    data.sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();
    data.iID_DonViID = $("#slbDonVi").val() == GUID_EMPTY ? null : $("#slbDonVi").val();
    data.iID_TiGiaID = $("#slbTiGia").val() == GUID_EMPTY ? null : $("#slbTiGia").val();
    data.iID_MaDonVi = $("<div/>").text($.trim($("#slbDonVi").find("option:selected").data("madonvi"))).html();
    return data;
}


///Validate///------------------------------------------------------------------------
function ValidateData(data, isTongHop) {
    var Title = 'Lỗi thêm mới/chỉnh sửa quyết toán niên độ';
    var Messages = [];

    if (data.sSoDeNghi == null || data.sSoDeNghi == "") {
        Messages.push("Số đề nghị chưa nhập !");
    }

    if (!isTongHop) {
        if (data.iID_DonViID == null || data.iID_DonViID == GUID_EMPTY) {
            Messages.push("Đơn vị  chưa chọn !");
        }
        if (data.iLoaiQuyetToan == null || data.iLoaiQuyetToan == 0) {
            Messages.push("Loại quyết toán chưa chọn !");
        }
        if (data.iID_TiGiaID == null || data.iID_TiGiaID == 0) {
            Messages.push("Tỉ giá chưa chọn !");
        }
    }


    if (data.iNamKeHoach == null || data.iNamKeHoach == 0) {
        Messages.push("Năm kế hoạch chưa chọn !");
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
