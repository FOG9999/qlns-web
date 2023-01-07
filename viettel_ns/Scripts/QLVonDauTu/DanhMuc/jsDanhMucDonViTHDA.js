var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaDonViTHDA = "";
    var sTenDonViTHDA = "";

    GetListData(sMaDonViTHDA, sTenDonViTHDA, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaDonViTHDA = $("#txtMaDonViTHDA").val();
    var sTenDonViTHDA = $("#txtTenDonViTHDA").val();

    GetListData(sMaDonViTHDA, sTenDonViTHDA, iCurrentPage);
}

function GetListData(sMaDonViTHDA, sTenDonViTHDA, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMDonViThucHienDuAn/DanhMucDonViThucHienDuAnSearch",
        data: { _paging: _paging, sMaDonViTHDA: sMaDonViTHDA, sTenDonViTHDA: sTenDonViTHDA},
        success: function (data) {
            $("#lstDataView").html(data);
            
            $("#txtMaDonViTHDA").val(sMaDonViTHDA);
            $("#txtTenDonViTHDA").val(sTenDonViTHDA);
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMDonViThucHienDuAn/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalDonViTHDA").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalDonViTHDALabel").html('Thêm mới đơn vị thực hiện dự án');
            }
            else {
                $("#modalDonViTHDALabel").html('Sửa thông tin đơn vị thực hiện dự án');
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "it",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMDonViThucHienDuAn/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalDonViTHDA").html(data);
            $("#modalDonViTHDALabel").html('Chi tiết đơn vị thực hiện dự án');
            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}

function DeleteItem(id, sTenDonVi) {
    var Title = 'Xác nhận xóa đơn vị thực hiện dự án';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "','" + sTenDonVi + "')";

    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function Delete(id, sTenDonVi) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDMDonViThucHienDuAn/DonViThucHienDuAnDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                alert("Xoa bản ghi " + sTenDonVi + " thành công.");
                ChangePage();
            }
        }
    });
}

function Save() {
    var data = {};
    data.iID_DonVi = $("#iID_DonViTHDAModal").val();
    data.iID_MaDonVi = $("#txtsMaDonViTHDA").val();
    data.sTenDonVi = $("#txtsTenDonViTHDA").val();
    data.sDiaChi = $("#txtsDiaChi").val();
    data.iID_DonViCha = $("#iID_DonViChaIDModal").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDMDonViThucHienDuAn/DonViThucHienDuAnSave",
        data: { data: data},
        success: function (r) {
            if (r.bIsComplete) {
                if (data.iID_DonVi == undefined || data.iID_DonVi == null || data.iID_DonVi == GUID_EMPTY || data.iID_DonVi == "") {
                    alert("Thêm mới bản ghi " + data.sTenDonVi + " thành công.")

                } else {
                    alert("Cập nhật bản ghi " + data.sTenDonVi + " thành công.")

                }
                window.location.href = "/QLVonDauTu/QLDMDonViThucHienDuAn";
            } else {
                var Title = 'Lỗi lưu nhà thầu';
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
    var Title = 'Lỗi thêm mới đơn vị thực hiện dự án';
    if (data.iID_DonVi != undefined && data.iID_DonVi != null && data.iID_DonVi != '' && data.iID_DonVi != GUID_EMPTY) {
        Title = 'Lỗi chỉnh sửa đơn vị thực hiện dự án';
    }

    var Messages = [];

    if (data.iID_MaDonVi == null || data.iID_MaDonVi == "") {
        Messages.push("Mã đơn vị chưa nhập !");
    }

    if (data.sTenDonVi == null || data.sTenDonVi == "") {
        Messages.push("Tên đơn vị chưa nhập !");
    }

    //if (CheckExistMaDonViThucHienDuAn(data.iID_MaDonVi)) {
    //    Messages.push("Đã tồn tại mã nhà thầu !");
    //}

    if (Messages != null && Messages != undefined && Messages.length > 0) {
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

function CheckExistMaDonViThucHienDuAn(iID_MaDonVi) {

}
