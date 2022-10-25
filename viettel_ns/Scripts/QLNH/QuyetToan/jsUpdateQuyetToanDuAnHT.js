var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

///save data ---------------------------------
function Save(isTongHop) {
    var data = GetDataQuyetToanDienDo();
    if (!ValidateData(data, isTongHop)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanDuAnHoanThanh/Save",
        data: { data: data },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QuyetToanDuAnHoanThanh/Detail?id=" + r.dataID + "&edit=" + !isTongHop;
            } else {
                var Title = 'Lỗi lưu thông tin quyết toán dự án hoàn thành';
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
    if (!ValidateData(data,true)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanDuAnHoanThanh/SaveTongHop",
        data: { data: data, listId: listId },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QuyetToanDuAnHoanThanh";
            } else {
                var Title = 'Lỗi lưu thông tin quyết toán dự án hoàn thành';
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
    console.log($("#slbTrangThai").val())
    var data = {};
    data.ID = $("#hidQTDuAnID").val();
    data.sSoDeNghi = $("<div/>").text($.trim($("#txtSoDeNghi").val())).html();
    data.dNgayDeNghi = $("#txtNgayDeNghi").val();
    data.iNamBaoCaoTu = $("#slbNamBaoCaoTu").val() == 0 ? null : $("#slbNamBaoCaoTu").val();
    data.iNamBaoCaoDen = $("#slbNamBaoCaoDen").val() == 0 ? null : $("#slbNamBaoCaoDen").val();
    data.iTrangThai = $("#slbTrangThai").val() == 0 ? null : $("#slbTrangThai").val();

    data.sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();
    data.iID_DonViID = $("#slbDonVi").val() == GUID_EMPTY ? null : $("#slbDonVi").val();
    data.iID_TiGiaID = $("#slbTiGia").val() == GUID_EMPTY ? null : $("#slbTiGia").val();
    data.iID_MaDonVi = $("<div/>").text($.trim($("#slbDonVi").find("option:selected").data("madonvi"))).html();
    return data;
}


function ValidateData(data, isTongHop) {
    var Title = 'Lỗi thêm mới/chỉnh sửa quyết toán dự án hoàn thành';
    var Messages = [];

    if (data.sSoDeNghi == null || data.sSoDeNghi == "") {
        Messages.push("Số đề nghị chưa nhập !");
    }

    if (!isTongHop) {
        if (data.iID_DonViID == null || data.iID_DonViID == GUID_EMPTY) {
            Messages.push("Đơn vị  chưa chọn !");
        }
        if (data.iNamBaoCaoTu == null || data.iNamBaoCaoTu == 0) {
            Messages.push("Năm báo cáo từ chưa chọn !");
        }
        if (data.iNamBaoCaoDen == null || data.iNamBaoCaoDen == 0) {
            Messages.push("Năm báo cáo đến chưa chọn !");
        }
        if (data.iID_TiGiaID == null || data.iID_DonViID == GUID_EMPTY) {
            Messages.push("Tỉ giá chưa chọn !");
        }
    }
    if (data.iTrangThai == null || data.iTrangThai == 0) {
        Messages.push("Trạng thái chưa chọn !");
    }
    
    if (data.iNamBaoCaoTu > data.iNamBaoCaoDen) {
        Messages.push("Năm báo cáo từ không lớn hơn năm báo cáo đến !");
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

