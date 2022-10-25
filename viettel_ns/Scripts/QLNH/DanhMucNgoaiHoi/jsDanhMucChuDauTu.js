var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaChuDauTu = "";
    var sTenChuDauTu = "";

    GetListData(sMaChuDauTu, sTenChuDauTu, iCurrentPage);
}
function ChangePage(iCurrentPage = 1) {
    var sMaChuDauTu = $("<div/>").text($.trim($("#txtMaChuDauTu").val())).html();
    var sTenChuDauTu = $("<div/>").text($.trim($("#txtTenChuDauTu").val())).html();

    GetListData(sMaChuDauTu, sTenChuDauTu, iCurrentPage);
}
function GetListData(sMaChuDauTu, sTenChuDauTu, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucChuDauTu/DanhMucChuDauTuSearch",
        data: { _paging: _paging, sMaChuDauTu: sMaChuDauTu, sTenChuDauTu: sTenChuDauTu },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtMaChuDauTu").val($("<div/>").html(sMaChuDauTu).text());
            $("#txtTenChuDauTu").val($("<div/>").html(sTenChuDauTu).text());
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucChuDauTu/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalChuDauTu").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
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

            $("#iID_ChuDauTuChaIDModal").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucChuDauTu/GetModalDetail",
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
        url: "/QLNH/DanhMucChuDauTu/ChuDauTuDelete",
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

function Save() {
    var data = {};
    data.ID = $("#iID_ChuDauTuModal").val();
    data.sId_CDT = $("<div/>").text($.trim($("#txtsMaChuDauTu").val())).html();
    data.sTenCDT = $("<div/>").text($.trim($("#txtsTenChuDauTu").val())).html();
    data.sKyHieu = $("<div/>").text($.trim($("#txtsKyHieuChuDauTu").val())).html();
    data.sMoTa = $("<div/>").text($.trim($("#txtsMoTaChuDauTu").val())).html();
    data.sLoai = $("<div/>").text($.trim($("#txtsLoaiChuDauTu").val())).html();
    if ($("#iID_ChuDauTuChaIDModal").val() != GUID_EMPTY) {
        data.Id_Parent = $("#iID_ChuDauTuChaIDModal").val();
    }

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucChuDauTu/ChuDauTuSave",
        data: { data: data},
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucChuDauTu";
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
    var Title = 'Lỗi thêm mới/chỉnh sửa chủ đầu tư';
    var Messages = [];

    if (data.sId_CDT == null || data.sId_CDT == "") {
        Messages.push("Mã chủ đầu tư chưa nhập !");
    }

    if (data.sTenCDT == null || data.sTenCDT == "") {
        Messages.push("Tên chủ đầu tư chưa nhập !");
    }

    if ($.trim($("#txtsMaChuDauTu").val()).length > 50) {
        Messages.push("Mã chủ đầu tư nhập quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenChuDauTu").val()).length > 500) {
        Messages.push("Tên chủ đầu tư nhập quá 500 kí tự !");
    }

    if ($.trim($("#txtsKyHieuChuDauTu").val()).length > 20) {
        Messages.push("Ký hiệu chủ đầu tư nhập quá 20 kí tự !");
    }

    if ($.trim($("#txtsMoTaChuDauTu").val()).length > 500) {
        Messages.push("Mô tả nhập quá 500 kí tự !");
    }

    if ($.trim($("#txtsLoaiChuDauTu").val()).length > 50) {
        Messages.push("Loại chủ đầu tư nhập quá 50 kí tự !");
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
