var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaChiPhi = "";
    var sTenVietTat = "";
    var sTenChiPhi = "";

    GetListData(sMaChiPhi, sTenVietTat, sTenChiPhi, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaChiPhi = $("#txtMaChiPhi").val();
    var sTenVietTat = $("#txtTenVietTat").val();
    var sTenChiPhi = $("#txtTenChiPhi").val();

    GetListData(sMaChiPhi, sTenVietTat, sTenChiPhi, iCurrentPage);
}

function GetListData(sMaChiPhi, sTenVietTat, sTenChiPhi, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMLoaiChiPhi/DanhMucLoaiChiPhiSearch",
        data: { _paging: _paging, sMaChiPhi: sMaChiPhi, sTenVietTat: sTenVietTat, sTenChiPhi: sTenChiPhi},
        success: function (data) {
            $("#lstDataView").html(data);
            
            $("#txtMaChiPhi").val(sMaChiPhi);
            $("#txtTenVietTat").val(sTenVietTat);
            $("#txtTenChiPhi").val(sTenChiPhi);
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMLoaiChiPhi/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalLoaiChiPhi").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalLoaiChiPhiLabel").html('Thêm mới loại chi phí');
            }
            else {
                $("#modalLoaiChiPhiLabel").html('Sửa thông tin loại chi phí');
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
        url: "/QLVonDauTu/QLDMLoaiChiPhi/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalLoaiChiPhi").html(data);
            $("#modalLoaiChiPhiLabel").html('Chi tiết loại chi phí');
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
    var Title = 'Xác nhận xóa loại chi phí';
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
        url: "/QLVonDauTu/QLDMLoaiChiPhi/LoaiChiPhiDelete",
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
    data.iID_ChiPhi = $("#iID_LoaiChiPhiModal").val();
    data.sMaChiPhi = $("#txtsMaChiPhiModal").val();
    data.sTenVietTat = $("#txtsTenVietTatModal").val();
    data.sTenChiPhi = $("#txtsTenChiPhiModal").val();
    data.iThuTu = $("#txtiThuTuModal").val()
    data.sMoTa = $("#txtsMoTaLoaiChiPhi").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDMLoaiChiPhi/LoaiChiPhiSave",
        data: { data: data},
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLDMLoaiChiPhi";
            } else {
                var Title = 'Lỗi lưu loại chi phí';
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
    var Title = 'Lỗi thêm mới/chỉnh sửa loại chi phí';
    var Messages = [];

    if (data.sMaChiPhi == null || data.sMaChiPhi == "") {
        Messages.push("Mã chi phí chưa nhập !");
    }

    if (data.sTenChiPhi == null || data.sTenChiPhi == "") {
        Messages.push("Tên chi phí chưa nhập !");
    }

    if (data.iThuTu == null || data.iThuTu == "") {
        Messages.push("Thứ tự chi phí chưa nhập !");
    }

    //if (CheckExistMaLoaiChiPhi(data.sMaChiPhi)) {
    //    Messages.push("Đã tồn tại mã chi phí !");
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

function CheckExistMaLoaiChiPhi(sMaChiPhi) {

}
