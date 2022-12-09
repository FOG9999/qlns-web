var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaChuDauTu = "";
    var sTenChuDauTu = "";

    GetListData(sMaChuDauTu, sTenChuDauTu, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaChuDauTu = $("#txtMaChuDauTu").val();
    var sTenChuDauTu = $("#txtTenChuDauTu").val();

    GetListData(sMaChuDauTu, sTenChuDauTu, iCurrentPage);
}

function GetListData(sMaChuDauTu, sTenChuDauTu, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMChuDauTu/DanhMucChuDauTuSearch",
        data: { _paging: _paging, sMaChuDauTu: sMaChuDauTu, sTenChuDauTu: sTenChuDauTu},
        success: function (data) {
            $("#lstDataView").html(data);
            
            $("#txtMaChuDauTu").val(sMaChuDauTu);
            $("#txtTenChuDauTu").val(sTenChuDauTu);
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMChuDauTu/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalChuDauTu").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalChuDauTuLabel").html('Thêm mới chủ đầu tư');
            }
            else {
                $("#modalChuDauTuLabel").html('Sửa thông tin chủ đầu tư');
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
        url: "/QLVonDauTu/QLDMChuDauTu/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalChuDauTu").html(data);
            $("#modalChuDauTuLabel").html('Chi tiết chủ đầu tư');
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

function DeleteItem(id) {
    var Title = 'Xác nhận xóa chủ đầu tư';
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
        url: "/QLVonDauTu/QLDMChuDauTu/ChuDauTuDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function Save() {
    var data = {};
    data.ID = $("#iID_ChuDauTuModal").val();
    data.sId_CDT = $("#txtsMaChuDauTu").val();
    data.sTenCDT = $("#txtsTenChuDauTu").val();
    data.sKyHieu = $("#txtsKyHieuChuDauTu").val();
    data.sMoTa = $("#txtsMoTaChuDauTu").val()
    data.sLoai = $("#txtsLoaiChuDauTu").val();
    data.STKTrongNuoc = $("#txtSTKTrongNuoc").val();
    data.ChiNhanhTrongNuoc = $("#txtChiNhanhTN").val();
    data.STKNuocNgoai = $("#txtSTKNgoaiNuoc").val();
    data.ChiNhanhNuocNgoai = $("#txtChiNhanhNN").val();

    if ($("#iID_ChuDauTuChaIDModal").val() != GUID_EMPTY) {
        data.Id_Parent = $("#iID_ChuDauTuChaIDModal").val();
    }

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDMChuDauTu/ChuDauTuSave",
        data: { data: data},
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLDMChuDauTu";
            } else {
                var Title = 'Lỗi lưu chủ đầu tư';
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
    var Title = 'Lỗi thêm mới chủ đầu tư';
    if ($("#iID_ChuDauTuModal").val() != GUID_EMPTY) {
        Title = 'Lỗi chỉnh sửa chủ đầu tư';
    };
    var Messages = [];

    if (data.sId_CDT == null || data.sId_CDT == "") {
        Messages.push("Mã chủ đầu tư chưa nhập !");
    }

    if (data.sTenCDT == null || data.sTenCDT == "") {
        Messages.push("Tên chủ đầu tư chưa nhập !");
    }

    //if (CheckExistMaChuDauTu(data.sId_CDT)) {
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

function CheckExistMaChuDauTu(sId_CDT) {

}
