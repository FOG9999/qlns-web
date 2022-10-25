var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function Save() {
    var data = {};
    data.iID_KhoiTaoID = $("#iID_KhoiTaoTTDAModal").val();
    data.iNamKhoiTao = $("#txtNamKhoiTaoModal").val();
    data.iID_DonViID = $('#iID_DonViIDModal').val();
    data.dNgayKhoiTao = $("#dNgayKhoiTaoModal").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/KhoiTaoTTDASave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                //window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAn";
                window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAn/Detail?iID_KhoiTao=" + r.iID_KhoiTao + "&sMaDonVi=" + r.sMaDonVi;
            } else {
                var Title = 'Lỗi lưu khởi tạo thông tin dự án';
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
    var Title = 'Lỗi thêm mới khởi tạo thông tin dự án';
    var Messages = [];

    if (data.iNamKhoiTao == null || data.iNamKhoiTao == "") {
        Messages.push("Năm khởi tạo chưa nhập !");
    }

    if (data.iID_DonViID == null || data.iID_DonViID == GUID_EMPTY || data.iID_DonViID == "") {
        Messages.push("Chưa chọn đơn vị quản lý !");
    }

    if (data.dNgayKhoiTao == null || data.dNgayKhoiTao == "") {
        Messages.push("Ngày khởi tạo chưa nhập !");
    }

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

function ResetChangePage(iCurrentPage = 1) {
    var iNamKhoiTao = "";
    var sTenDonVi = "";

    GetListData(iNamKhoiTao, sTenDonVi, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var iNamKhoiTao = $("#txtNamKhoiTao").val();
    var sTenDonVi = $("#txtDonViQL").val();

    GetListData(iNamKhoiTao, sTenDonVi, iCurrentPage);
}

function GetListData(iNamKhoiTao, sTenDonVi, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/KhoiTaoTTDAListView",
        data: { iNamKhoiTao: iNamKhoiTao, sTenDonVi: sTenDonVi, _paging: _paging},
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtNamKhoiTao").val(iNamKhoiTao);
            $("#txtDonViQL").val(sTenDonVi);
        }
    });
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa khởi tạo thông tin dự án';
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
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/DeleteKhoiTaoTTDA",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
            else {
                var Title = 'Lỗi xóa khởi tạo thông tin dự án';
                var Messages = [];
                Messages.push("Lỗi xóa khởi tạo thông tin dự án !");
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data1) {
                        $("#divModalConfirm").html(data1);
                    }
                });
            }
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalKhoiTaoTTDA").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalKhoiTaoTTDALabel").html('Thêm mới khởi tạo thông tin dự án');
            }
            else {
                $("#modalKhoiTaoTTDALabel").html('Sửa khởi tạo thông tin dự án');
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
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalKhoiTaoTTDA").html(data);
            $("#modalKhoiTaoTTDALabel").html('Chi tiết khởi tạo thông tin dự án');
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

//Chi tiết khởi tạo thông tin dự án
function ChiTietKhoiTaoTTDA(iID_KhoiTao, sMaDonVi) {
    window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAn/Detail?iID_KhoiTao=" + iID_KhoiTao + "&sMaDonVi=" + sMaDonVi;
}

//Import
function ImportDonViBHXH() {
    window.location.href = "/BaoHiemXaHoi/DanhMucDonViBHXH/ImportDonViBHXH/";
}
